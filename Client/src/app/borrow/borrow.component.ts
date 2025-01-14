import { Component, OnInit } from '@angular/core';
import { BorrowModel } from '../Shared/models/borrow.model';
import { BorrowService } from '../services/borrow.service';
import { NotificationService } from '../../environment/notification.service';

@Component({
  selector: 'app-borrow',
  templateUrl: './borrow.component.html',
  styleUrl: './borrow.component.css'
})
export class BorrowComponent implements OnInit {
  borrows: BorrowModel[] = [];
  isLoading: boolean = false; // Biến kiểm soát trạng thái loading

  constructor(
    private borrowService: BorrowService,
    private notificationService: NotificationService // Inject NotificationService
  ) { }

  ngOnInit(): void {
    this.loadBorrows();
  }

  // Tải danh sách mượn sách
  loadBorrows(): void {
    this.isLoading = true; // Bắt đầu loading
    this.borrowService.getAllBorrows().subscribe(
      (data) => {
        this.borrows = data;
        this.isLoading = false; // Kết thúc loading
      },
      (error) => {
        console.error('Error loading borrows', error);
        this.isLoading = false; // Kết thúc loading nếu có lỗi
        this.notificationService.showNotification('Lỗi', 'Không thể tải danh sách mượn sách', 'error');
      }
    );
  }

  // Mượn sách
  borrowBook(bookId: number, borrowCount: number): void {
    this.isLoading = true; // Bắt đầu loading
    this.borrowService.borrowBook(bookId, borrowCount).subscribe(
      () => {
        this.notificationService.showNotification('Thành công', 'Mượn sách thành công', 'success');
        this.loadBorrows(); // Tải lại danh sách sau khi mượn sách
      },
      (error) => {
        console.error('Error borrowing book', error);
        this.isLoading = false; // Kết thúc loading nếu có lỗi
        this.notificationService.showNotification('Lỗi', 'Mượn sách thất bại', 'error');
      }
    );
  }

  // Trả sách
  returnBook(borrowId: number): void {
    this.notificationService.showConfirmation(
      'Xác nhận',
      'Bạn có chắc chắn muốn trả sách này không?',
      'warning'
    ).then((isConfirmed) => {
      if (isConfirmed) {
        this.isLoading = true; // Bắt đầu loading
        this.borrowService.returnBook(borrowId).subscribe(
          () => {
            this.notificationService.showNotification('Thành công', 'Trả sách thành công', 'success');
            this.loadBorrows(); // Tải lại danh sách sau khi trả sách
          },
          (error) => {
            console.error('Error returning book', error);
            this.isLoading = false; // Kết thúc loading nếu có lỗi
            this.notificationService.showNotification('Lỗi', 'Trả sách thất bại', 'error');
          }
        );
      }
    });
  }

  // Xóa bản ghi mượn sách
  deleteBorrowRecord(borrowId: number): void {
    this.notificationService.showConfirmation(
      'Xác nhận',
      'Bạn có chắc chắn muốn xóa bản ghi này không?',
      'warning'
    ).then((isConfirmed) => {
      if (isConfirmed) {
        this.isLoading = true; // Bắt đầu loading
        this.borrowService.deleteBorrowRecord(borrowId).subscribe(
          () => {
            this.notificationService.showNotification('Thành công', 'Xóa bản ghi thành công', 'success');
            this.loadBorrows(); // Tải lại danh sách sau khi xóa
          },
          (error) => {
            console.error('Error deleting borrow record', error);
            this.isLoading = false; // Kết thúc loading nếu có lỗi
            this.notificationService.showNotification('Lỗi', 'Xóa bản ghi thất bại', 'error');
          }
        );
      }
    });
  }
}
