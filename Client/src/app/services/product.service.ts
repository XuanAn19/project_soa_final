import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { HttpClient, HttpContext } from '@angular/common/http';
import { environment } from '../../environment/environment';
import { Observable } from 'rxjs';
import { Product } from '../Shared/models/product.model';
import { ApiBaseService } from './api-base.service';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private url = environment.apiUrl ;
  constructor(private _api : ApiBaseService, private http : HttpClient) { }

  getAllProduct(): Observable<Product>{
    return this.http.get<Product>(this.url + 'api/products');
  }
  getProductById(id: Number): Observable<any> {//được sử dụng để gửi yêu cầu tới API để lấy thông tin về một sản phẩm cụ thể dựa trên id được truyền vào và trả về một đối tượng Observable để lắng nghe kết quả trả về từ API.
    return this._api.getTypeRequest('api/products/' + id);
  }

  addProduct(productData: any): Observable<any>{
    const data ={
      "name" : productData.name,
      "description" : productData.description,
      "quantity": productData.quantity,
      "price" : productData.price,
      "created_day": productData.created_day,
      "updated_day" : productData.updated_day
    }
    return this._api.postTypeRequest(this.url +'api/products', data);
  }
  updateProduct(productId: any, productData: any): Observable<any> {
    const data = {
      "name" : productData.name,
      "description" : productData.description,
      "quantity": productData.quantity,
      "price" : productData.price,
      "created_day": productData.created_day,
      "updated_day" : productData.updated_day
    }
    alert(productData.category);
    return this.http.put(this.url + 'api/products/' + productId, data);
  }

  deleteProduct(productId: any): Observable<any> {
    return this.http.delete(this.url + 'api/products/' + productId);
  }
}
