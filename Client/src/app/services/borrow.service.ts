import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { url } from 'inspector';
import { Observable } from 'rxjs';
import { ApiBaseService } from './api-base.service';
import { BookDTO, BorrowModel } from '../Shared/models/borrow.model';

@Injectable({
  providedIn: 'root'
})
export class BorrowService {
  private apiUrl = 'api/borrow'; // Địa chỉ API backend

  constructor(private http: HttpClient, private _api: ApiBaseService) { }

  getAllBorrows(): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrl}/getAll`);
  }
  // Lấy danh sách bản mượn của người dùng theo idUser
  getBorrowsByUser(): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrl}/getBorrowByIdUser`);
  }
  borrowBook(bookId: number, borrowCount: number): Observable<any> {
    const payload = {
      IdBook : bookId,
      BorrowCount : borrowCount
    };
    console.log('Payload:', payload);
    return this._api.postTypeRequest(`${this.apiUrl}/create`, payload );
  }

  returnBook(borrowId: number): Observable<any> {
    return this._api.putTypeRequest(`${this.apiUrl}/return/${borrowId}`, borrowId);
  }

  deleteBorrowRecord(borrowId: number): Observable<any> {
    return this._api.deleteTypeRequest(`${this.apiUrl}/borrow/${borrowId}`);
  }

  getBookById(bookId: number): Observable<BookDTO> {
    return this.http.get<BookDTO>(`/api/BookApi/${bookId}`);
  }
}
