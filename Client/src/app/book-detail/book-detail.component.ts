import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService } from '../services/book.service';
import { BorrowService } from '../services/borrow.service';
import { NotificationService } from '../../environment/notification.service';
import { AuthService } from '../services/auth.service';
import { TokenStoreService } from '../services/token-store.service';

@Component({
  selector: 'app-book-detail',
  templateUrl: './book-detail.component.html',
  styleUrl: './book-detail.component.css'
})
export class BookDetailComponent implements OnInit {
  book: any;  // Lưu trữ dữ liệu chi tiết sách
  Id: number | undefined;  // ID của cuốn sách
  borrowError: string | null = null; // Thông báo lỗi mượn sách

  constructor(
    private route: ActivatedRoute,  // Để lấy ID từ URL
    private bookService: BookService,  // Để gọi API lấy chi tiết sách
    private borrow: BorrowService,
    private notificationService: NotificationService,
    private _token: TokenStoreService,
    private router : Router,
    private _auth: AuthService
  ) { }

  ngOnInit(): void {
    // Lấy ID từ URL
    this.Id = +this.route.snapshot.paramMap.get('id')!;

    // Gọi API để lấy chi tiết sách
    this.bookService.getBookById(this.Id).subscribe(
      (data) => {
        this.book = data;  // Lưu thông tin chi tiết sách vào biến book
        console.log(this.book);
      },
      (error) => {
        console.error('Lỗi khi lấy chi tiết sách:', error);
      }
    );
  }

  // Kiểm tra số lượng sách đã mượn trước khi mượn thêm
  checkBorrowLimitAndBorrow() {
    if (!this._token.getToken()) {
      this.router.navigate(['/login']);}
      else{
    // Lấy danh sách bản mượn của người dùng
    this.borrow.getBorrowsByUser().subscribe(
      (borrows) => {
        // Kiểm tra số lượng bản mượn của người dùng
        const borrowCount = borrows.length;

        if (borrowCount >= 2) {
          // Nếu đã mượn 2 sách, yêu cầu trả sách trước khi mượn thêm
          this.notificationService.showNotification(
            'Không thể mượn thêm sách!',
            'Bạn cần trả sách trước khi mượn thêm.',
            'warning'
          );
        } else {
          // Nếu chưa mượn đủ 2 sách, cho phép mượn
          this.borrowBook();
        }
      },
      (error) => {
        console.error('Lỗi khi lấy thông tin bản mượn:', error);
        this.notificationService.showNotification(
          'Lỗi khi lấy dữ liệu!',
          'Không thể lấy thông tin mượn sách.',
          'error'
        );
      }
    );}
  }

// Xử lý mượn sách
borrowBook(): void {
  if (this.book.availableCopies > 0) {
    this.notificationService
      .showConfirmation(
        'Xác nhận mượn sách',
        `Bạn có chắc chắn muốn mượn sách "${this.book.title}" không?`,
        'warning',
        'Có',
        'Hủy'
      )
      .then((confirmed) => {
        if (confirmed) {
          this.borrow.borrowBook(this.book.id, 1).subscribe(
            (response) => {
              this.borrowError = null; // Xóa thông báo lỗi (nếu có)
              this.book.availableCopies--; // Cập nhật số lượng sách
              this.notificationService.showNotification(
                'Thành công',
                'Mượn sách thành công!',
                'success'
              );
            },
            (error) => {
              console.error('Lỗi khi mượn sách:', error);
              this.notificationService.showNotification(
                'Lỗi',
                'Có lỗi xảy ra khi mượn sách.',
                'error'
              );
            }
          );
        }
      });
  } else {
    this.notificationService.showNotification(
      'Không thể mượn',
      'Không thể mượn sách. Số lượng đã hết.',
      'warning'
    );
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
// Kiểm tra nếu người dùng đã đăng nhập
isLoggedIn(): boolean {
  return this._token.getToken() !== null;
}
}
