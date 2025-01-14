import { Component, OnInit } from '@angular/core';

import { BookService } from '../services/book.service';
import { GenerService } from '../services/gener.service';
import { Router } from '@angular/router';
import { LoadingService } from '../services/loading.service';
import { TokenStoreService } from '../services/token-store.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  categories: any[] = []; // Lưu trữ danh mục
  books: any[] = []; // Khởi tạo mảng sách
  selectedCategoryID: number | null = null;
  searchKeyword: string = ''; // Biến chứa từ khóa tìm kiếm

  constructor(
    private bookService: BookService,
    private categoryService: GenerService,
    public loadingService: LoadingService, // Inject LoadingService
    private router: Router, // Thêm Router để điều hướng
    private _token: TokenStoreService,
    private _auth: AuthService
  ) {}

  ngOnInit(): void {
    this.getBooks();
    this.getCategories();
  }
  // Kiểm tra nếu người dùng đã đăng nhập
  isLoggedIn(): boolean {
    return this._token.getToken() !== null;
  }
  getBooks(): void {
    this.loadingService.startLoading(); // Bật trạng thái loading
    this.bookService.getBooks().subscribe(
      (data) => {
        this.books = data; // Gán dữ liệu vào mảng books
        this.loadingService.stopLoading(); // Tắt trạng thái loading
        console.log('Books:', this.books); // Kiểm tra dữ liệu trả về
      },
      (error) => {
        console.error('Error fetching books:', error); // Kiểm tra lỗi nếu có
        this.loadingService.stopLoading(); // Tắt trạng thái loading nếu có lỗi
      }
    );
  }

  // Lấy danh mục từ API
  getCategories(): void {
    this.loadingService.startLoading(); // Bật trạng thái loading
    this.categoryService.getGenres().subscribe(
      (data) => {
        this.categories = data;
        this.loadingService.stopLoading(); // Tắt trạng thái loading
      },
      (error) => {
        console.error('Error fetching categories:', error);
        this.loadingService.stopLoading(); // Tắt trạng thái loading nếu có lỗi
      }
    );
  }

  // Lấy sản phẩm theo danh mục
  onCategorySelect(category: any): void {
    this.selectedCategoryID = category.genreID;

    if (this.selectedCategoryID !== null) {
      this.loadingService.startLoading(); // Bật trạng thái loading
      this.categoryService.getBookByCategory(this.selectedCategoryID).subscribe(
        (data) => {
          this.books = data;
          this.loadingService.stopLoading(); // Tắt trạng thái loading
        },
        (error) => {
          console.error('Error fetching products:', error);
          this.loadingService.stopLoading(); // Tắt trạng thái loading nếu có lỗi
        }
      );
    } else {
      console.error('Category ID is null');
    }
  }

  // Hàm xử lý khi nhấn vào "Home" để quay về tất cả sách
  goToHome(): void {
    this.selectedCategoryID = null; // Đặt lại bộ lọc thể loại
    this.router.navigate(['/']); // Điều hướng về trang chủ (tất cả sách)
    this.getBooks(); // Gọi lại hàm lấy tất cả sách
  }

  // Hàm tìm kiếm sách
  searchBooks(): void {
    if (this.searchKeyword.trim() !== '') {
      this.loadingService.startLoading(); // Bật trạng thái loading
      this.bookService.searchBooks(this.searchKeyword).subscribe(
        (data) => {
          this.books = data; // Gán kết quả tìm kiếm vào mảng books
          this.loadingService.stopLoading(); // Tắt trạng thái loading
        },
        (error) => {
          console.error('Error searching books:', error);
          this.loadingService.stopLoading(); // Tắt trạng thái loading nếu có lỗi
        }
      );
    } else {
      alert('Vui lòng nhập từ khóa tìm kiếm!');
    }
  }
  infomation(): void {
    this.router.navigate(['/user/infomation']);
  }
  // Đăng xuất
  logout(): void {
    this._auth.logout();
    this.router.navigate(['/login']);
  }
}
