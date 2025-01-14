import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { TokenStoreService } from '../services/token-store.service';//Quản lý thông tin token.

@Injectable({
  providedIn: 'root',
})
export class AuthGuardService implements CanActivate {
  constructor(private _route: Router, private _token: TokenStoreService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const currentUser = this._token.getUser();
    // Kiểm tra xem người dùng có đăng nhập và có quyền admin không
    if (currentUser) {
      const path = route.routeConfig?.path; // Lấy đường dẫn của route

      if (path && path.startsWith('admin')) { // Chỉ kiểm tra quyền admin đối với các route có 'admin'
        if (currentUser.role === 'admin') {
          return true; // Người dùng là admin thì cho phép truy cập
        } else {
          this._route.navigate(['/']); // Nếu không phải admin, chuyển hướng về trang chủ hoặc trang khác
          return false;
        }
      }
      return true; // Các route không phải 'admin' thì luôn cho phép truy cập
    }
    this._route.navigate(['/login']);
    return false;
  }
}
