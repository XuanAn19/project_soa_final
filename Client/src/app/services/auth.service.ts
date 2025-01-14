import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { ApiBaseService } from './api-base.service';
import { TokenStoreService } from './token-store.service';
import { emit } from 'process';
import { LoginModel } from '../Shared/models/LoginModel.model';
import { TokenModel } from '../Shared/models/TokenModel.model';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject: BehaviorSubject<any>;
  public user: Observable<any>;
  private  baseUrl = `${environment.apiUrl}`;
  constructor(private _api: ApiBaseService, private _token: TokenStoreService, private _http: HttpClient) {
    this.userSubject = new BehaviorSubject<any>(this._token.getUser());
    this.user = this.userSubject.asObservable();

  }
  getUser() {
    console.log(this.userSubject);
    console.log(this.userSubject.value);
    return this.userSubject.value;
  }
  login(login : LoginModel): Observable<TokenModel> {
    const urlApi = `api/v1/auth/login`;
    return this._api.postTypeRequest(urlApi, login).pipe(
      map((res : any)=>{
         this._token.setToken(res.accessToken);
         this._token.setRefreshToken(res.refreshToken);
        this._token.setUser({ email: res.email , role: res.role});

        const user: TokenModel = {
          accessToken: res.accessToken,
          refreshToken: res.refreshToken,
          fullName: res.fullname,
          Email: login.Email,
          role : res.role
        }

        console.log(res);
        this.userSubject.next(user);
        return user;

      })
    );
  }
  //Tạo mới token
  createNewRefreshToken(refreshToken: string){
    const urlApi = `${this.baseUrl}api/v1/auth/refresh-token`;

    return this._http.post<any>(urlApi,{refreshToken:refreshToken})
  }


  register(userRegister: any): Observable<any> {
    return this._api.postTypeRequest(`api/v1/auth/register`, userRegister);
  }

  codeConfirmEmail(sendCodeEmail : any): Observable<any>{
    const confirmData = {
      Email : sendCodeEmail.Email,
      Code : sendCodeEmail.Code
    };
    return this._api.putTypeRequest('api/v1/auth/confirm', confirmData);
  }

  logout() {
    this._token.clearStorage();
    this.userSubject.next(null);

  }
  
}


