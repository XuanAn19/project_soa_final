import { Injectable } from '@angular/core';
import { ApiBaseService } from './api-base.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `api/v1/user`;
  constructor(private _api: ApiBaseService) {}

  // Lấy thông tin người dùng
  getUserInfo(): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrl}/infomation`);
  }

  // Cập nhật thông tin người dùng
  updateUserInfo(email: string, updateUserDTO: any): Observable<any> {
    return this._api.putTypeRequest(`${this.apiUrl}/update/${email}`, updateUserDTO);
  }

  getAllUsers(): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrl}/all`);
  }

  updateUserRole(userId: string, roleId: string): Observable<any> {
    return this._api.putTypeRequest(`${this.apiUrl}/role/${userId}`, { roleId });
  }

  deleteUser(userId: string): Observable<any> {
    return this._api.deleteTypeRequest(`${this.apiUrl}/${userId}`);
  }

  getAllRoles(): Observable<any> {
    return this._api.getTypeRequest(`api/v1/roles`);
  }

  getRoleById(roleId : string): Observable<any> {
    return this._api.getTypeRequest(`api/v1/roles/${roleId}`);
  }
}
