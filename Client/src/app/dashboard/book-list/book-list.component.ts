import { Component, OnInit } from '@angular/core';
import { Book, Genre } from '../../Shared/models/Book.model';
import { BookService } from '../../services/book.service';
import { GenerService } from '../../services/gener.service';
import { NotificationService } from '../../../environment/notification.service';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  genres: Genre[] = [];
  isLoading = true;
  isFormVisible = false;
  isEditMode = false;
  formData: Book = {
    id: 0,
    title: '',
    author: '',
    publishedDate: new Date(),
    price: 0,
    availableCopies: 0,
    genreID: 0,
    genre: null,
    image: '',
    imageFile: null,
    isAvailable: true
  };

  constructor(private bookService: BookService, private gener: GenerService, private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.loadBooks();
    this.loadGenres();
  }

  loadBooks(): void {
    this.bookService.getBooks().subscribe({
      next: (books) => {
        this.books = books;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load books:', err);
        this.isLoading = false;
      },
    });
  }

  loadGenres(): void {
    this.gener.getGenres().subscribe(genres => this.genres = genres);
  }

  openForm(book?: Book): void {
    this.isFormVisible = true;
    if (book) {
      this.isEditMode = true;
      this.formData = { ...book };
      if (typeof this.formData.publishedDate === 'string') {
        this.formData.publishedDate = new Date(this.formData.publishedDate);
      }
    } else {
      this.isEditMode = false;
      this.formData = {
        id: 0,
        title: '',
        author: '',
        publishedDate: new Date(),
        price: 0,
        availableCopies: 0,
        genreID: 0,
        genre: null,
        image: '',
        imageFile: null,
        isAvailable: true
      };
    }
  }

  closeForm(): void {
    this.isFormVisible = false;
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.formData.imageFile = file;
      // Hiển thị ảnh trước khi upload
      const reader = new FileReader();
      reader.onload = () => {
        this.formData.image = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (this.isEditMode) {
      this.updateBook(); // Nếu là chế độ chỉnh sửa, gọi phương thức cập nhật
    } else {
      this.addBook(); // Nếu không, gọi phương thức thêm sách
    }
  }

  addBook(): void {
    // Hiển thị thông báo xác nhận trước khi thêm sách
    this.notificationService
      .showConfirmation('Xác nhận', 'Bạn có chắc chắn muốn thêm sách này không?', 'warning')
      .then((confirmed) => {
        if (confirmed) {
          // Nếu người dùng nhấn "Đồng ý", thực hiện thêm sách
          const formData = new FormData();

          // Thêm dữ liệu vào FormData
          formData.append('title', this.formData.title);
          formData.append('author', this.formData.author);
          formData.append('publishedDate', new Date(this.formData.publishedDate).toISOString());
          formData.append('price', this.formData.price.toString());
          formData.append('availableCopies', this.formData.availableCopies.toString());
          formData.append('genreID', this.formData.genreID.toString());
          formData.append('isAvailable', this.formData.isAvailable.toString());

          // Thêm tệp ảnh nếu có
          if (this.formData.imageFile) {
            formData.append('imageFile', this.formData.imageFile, this.formData.imageFile.name);
          }

          // Gửi yêu cầu thêm sách lên server
          this.bookService.addBook(formData).subscribe({
            next: (response) => {
              this.books.push(response); // Thêm sách mới vào danh sách
              this.closeForm(); // Đóng form
              this.notificationService.showNotification('Thành công', 'Sách đã được thêm thành công!', 'success');
            },
            error: (err) => {
              console.error('Thêm sách thất bại:', err);
              this.notificationService.showNotification('Lỗi', 'Thêm sách thất bại. Vui lòng thử lại!', 'error');
            },
          });
        } else {
          // Nếu người dùng nhấn "Hủy", hiển thị thông báo hủy
          this.notificationService.showNotification('Đã hủy', 'Thao tác thêm sách đã bị hủy.', 'info');
        }
      });
  }

  updateBook(): void {
    // Hiển thị thông báo xác nhận trước khi cập nhật sách
    this.notificationService
      .showConfirmation('Xác nhận', 'Bạn có chắc chắn muốn cập nhật sách này không?', 'warning')
      .then((confirmed) => {
        if (confirmed) {
          // Nếu người dùng nhấn "Đồng ý", thực hiện cập nhật sách
          const formData = new FormData();

          // Thêm dữ liệu vào FormData
          formData.append('title', this.formData.title);
          formData.append('author', this.formData.author);
          formData.append('publishedDate', new Date(this.formData.publishedDate).toISOString());
          formData.append('price', this.formData.price.toString());
          formData.append('availableCopies', this.formData.availableCopies.toString());
          formData.append('genreID', this.formData.genreID.toString());
          formData.append('isAvailable', this.formData.isAvailable.toString());

          // Thêm tệp ảnh nếu có
          if (this.formData.imageFile) {
            formData.append('imageFile', this.formData.imageFile, this.formData.imageFile.name);
          }

          // Gửi yêu cầu cập nhật sách lên server
          this.bookService.updateBook(this.formData.id, formData).subscribe({
            next: (response) => {
              // Cập nhật sách trong danh sách
              const index = this.books.findIndex(book => book.id === this.formData.id);
              if (index !== -1) {
                this.books[index] = response;
              }
              this.closeForm(); // Đóng form
              this.notificationService.showNotification('Thành công', 'Sách đã được cập nhật thành công!', 'success');
            },
            error: (err) => {
              console.error('Cập nhật sách thất bại:', err);
              this.notificationService.showNotification('Lỗi', 'Cập nhật sách thất bại. Vui lòng thử lại!', 'error');
            },
          });
        } else {
          // Nếu người dùng nhấn "Hủy", hiển thị thông báo hủy
          this.notificationService.showNotification('Đã hủy', 'Thao tác cập nhật sách đã bị hủy.', 'info');
        }
      });
  }

  deleteBook(id: number): void {
    // Hiển thị thông báo xác nhận trước khi xóa sách
    this.notificationService
      .showConfirmation('Xác nhận', 'Bạn có chắc chắn muốn xóa sách này không?', 'warning')
      .then((confirmed) => {
        if (confirmed) {
          // Nếu người dùng nhấn "Đồng ý", thực hiện xóa sách
          this.bookService.deleteBook(id).subscribe({
            next: () => {
              // Xóa sách khỏi danh sách
              this.books = this.books.filter(book => book.id !== id);
              this.notificationService.showNotification('Thành công', 'Sách đã được xóa thành công!', 'success');
            },
            error: (err) => {
              console.error('Xóa sách thất bại:', err);
              this.notificationService.showNotification('Lỗi', 'Xóa sách thất bại. Vui lòng thử lại!', 'error');
            },
          });
        } else {
          // Nếu người dùng nhấn "Hủy", hiển thị thông báo hủy
          this.notificationService.showNotification('Đã hủy', 'Thao tác xóa sách đã bị hủy.', 'info');
        }
      });
  }
}
