import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenStoreService {
  TOKEN_KEY = 'accessToken';
  REFRESH_TOKEN ='refreshToken';
  USER_KEY = 'auth-user';
  constructor() { }
  public getToken() {
    return sessionStorage.getItem(this.TOKEN_KEY);
  }
  setToken(token: string): void {
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.setItem(this.TOKEN_KEY, token);
  }

  setRefreshToken(refreshToken: string): void {
    sessionStorage.removeItem(this.REFRESH_TOKEN);
    sessionStorage.setItem(this.REFRESH_TOKEN, refreshToken);
  }

  getUser(): any {
    const user = sessionStorage.getItem(this.USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  setUser(user: any): void {
    sessionStorage.removeItem(this.USER_KEY);
    sessionStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }
  clearStorage(): void {
    sessionStorage.clear();
  }
}
