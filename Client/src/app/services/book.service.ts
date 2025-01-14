import { Injectable } from '@angular/core';
import { environment } from '../../environment/environment';
import { ApiBaseService } from './api-base.service';
import { Observable } from 'rxjs';
import { Book } from '../Shared/models/Book.model';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = `api/BookApi`;
  private apiUrlHome = `api/Home`;

  constructor(private _api : ApiBaseService, private http : HttpClient) {}

  getBooks(): Observable<any> {
    return this._api.getTypeRequest(this.apiUrl);
  }

  getAllBooksHome(): Observable<any> {
    return this._api.getTypeRequest(this.apiUrlHome);
  }
  // Tìm kiếm sách theo tên
  searchBooks(keyword: string): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrlHome}/search?keyword=${encodeURIComponent(keyword)}`);
  }


  getBookById(id: number): Observable<any> {
    return this._api.getTypeRequest(`${this.apiUrl}/${id}`);
  }

  addBook(bookData: FormData): Observable<any> {
    return this._api.postTypeRequest(`${this.apiUrl}`, bookData);
  }

  updateBook(id: number, bookData: FormData): Observable<any> {
    return this._api.putTypeRequest(`${this.apiUrl}/${id}`, bookData);
  }

  deleteBook(id: number): Observable<any> {
    return this._api.deleteTypeRequest(`${this.apiUrl}/${id}`);
  }

}
