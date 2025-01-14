import { HTTP_INTERCEPTORS, HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, switchMap, throwError } from 'rxjs';
import { TokenStoreService } from './token-store.service';
import { AuthService } from './auth.service';
const TOKEN_HEADER_KEY = 'Authentication';
@Injectable({
  providedIn: 'root'
})
export class InterceptorService implements HttpInterceptor {

  constructor(private  _token : TokenStoreService,private _authService: AuthService ) {}
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    console.log('Original Request:', request.url);
    // Bỏ qua các API không yêu cầu token (VD: endpoint login)
    if (request.url.includes('auth/login')|| request.url.includes('api/Home')) {
      return next.handle(request);
    }

    // Lấy token từ sessionStorage
    const tokenRequest = sessionStorage.getItem('accessToken');
    const refreshToken = sessionStorage.getItem('refreshToken');

    let authReq = request;
    if (tokenRequest) {
      // Clone và gắn token vào header
      authReq = request.clone({
        setHeaders: { Authorization: `Bearer ${tokenRequest}` }
      });
      console.log(authReq);
    }

    return next.handle(authReq).pipe(
      // Xử lý khi token bị lỗi 401 (Unauthorized)
      catchError((error) => {
        if (error instanceof HttpErrorResponse && error.status === 401 && refreshToken) {
          // Sử dụng refresh token để tạo token mới
          return this._authService.createNewRefreshToken(refreshToken).pipe(
            switchMap((newToken) => {
              // Cập nhật token mới vào sessionStorage
              sessionStorage.setItem('accessToken', newToken.accessToken);
              const updatedRequest = request.clone({
                setHeaders: { Authorization: `Bearer ${newToken.accessToken}` }
              });
              // Gửi lại request với token mới
              return next.handle(updatedRequest);
            })
          );
        }
        return throwError(() => error.message);
      })
    );
  }
}

