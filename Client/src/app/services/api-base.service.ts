import { Injectable } from '@angular/core';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiBaseService {
  private baseUrl = environment.apiUrl;

  constructor(private _http: HttpClient) {}

  getTypeRequest(url: string) {// phuong thức này gửi yeu cầu GEt đên URL được chỉ định
    return this._http.get(`${this.baseUrl}${url}`);
  }
  postTypeRequest(url: string, payload: any) {
    return this._http.post(`${this.baseUrl}${url}`, payload).pipe(
      map((res) => {
        console.log('API Response:', res); // Xem chi tiết phản hồi từ API
        return res;
      })
    );
  }
  putTypeRequest(url: string, payload: any) {
    return this._http.put(`${this.baseUrl}${url}`, payload).pipe(
      map((res) => {
        console.log('API Response:', res); // Xem chi tiết phản hồi từ API
        return res;
      })
    );
  }

  // Phương thức DELETE
  deleteTypeRequest(url: string) {
    return this._http.delete(`${this.baseUrl}${url}`).pipe(
      map((res) => {
        console.log('API Response:', res); // Xem chi tiết phản hồi từ API
        return res;
      })
    );
  }
}
