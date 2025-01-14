import { Injectable } from '@angular/core';
import { ApiBaseService } from './api-base.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = `api/v1/roles`;
  constructor(private _api: ApiBaseService) { }

  // Tạo quyền mới
  createRole(role: any): Observable<any> {
    return this._api.postTypeRequest(`${this.apiUrl}/create`, role);
  }
  // Lấy tất cả các vai trò
  getAllRoles(): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrl}`);
  }

  // Cập nhật vai trò
  updateRole(roleId: string, roleData: any): Observable<any> {
    return this._api.putTypeRequest(`${this.apiUrl}/update/${roleId}`, roleData);
  }

  // Xóa vai trò
  deleteRole(roleId: string): Observable<any> {
    return this._api.deleteTypeRequest(`${this.apiUrl}/delete/${roleId}`);
  }
}
