import { Injectable } from '@angular/core';
import { ApiBaseService } from './api-base.service';
import { catchError, finalize, Observable, throwError } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class GenerService {

  private apiUrl = `api/Genres`;
  private apiUrlHome = `api/Home`;
  constructor(private _api : ApiBaseService) { }

  getGenres() : Observable<any>{
    return this._api.getTypeRequest(this.apiUrl);
  }

  // Lấy danh sách sản phẩm theo danh mục
 // getBookByCategory(GenreID: number): Observable<any> {
 //   return this._api.getTypeRequest(`${this.apiUrlHome}/genre/${GenreID}`);
 // }
 getBookByCategory(GenreID: number): Observable<any> {
  return this._api.getTypeRequest(`${this.apiUrlHome}/genre/${GenreID}`).pipe(
    catchError((error) => {
      console.error('Error fetching categories:', error);
      return throwError(() => error);
    }),
    finalize(() => {
      console.log('API call completed or failed');
      // Đây là nơi bạn có thể dừng hoặc làm gì đó khi API call hoàn tất
    })
  );
}


}
