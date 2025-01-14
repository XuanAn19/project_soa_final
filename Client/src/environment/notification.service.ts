import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor() {}

  /**
   * Hiển thị thông báo thông thường
   * @param title Tiêu đề thông báo
   * @param text Nội dung thông báo
   * @param icon Loại icon (success, error, warning, info)
   * @param timer Thời gian tự động đóng thông báo (ms)
   */
  showNotification(
    title: string,
    text: string,
    icon: 'success' | 'error' | 'warning' | 'info' = 'info',
    timer: number = 3000
  ) {
    Swal.fire({
      title: title,
      text: text,
      icon: icon,
      confirmButtonText: 'OK',
      timer: timer,
    });
  }

  /**
   * Hiển thị thông báo xác nhận
   * @param title Tiêu đề thông báo
   * @param text Nội dung thông báo
   * @param icon Loại icon (success, error, warning, info)
   * @param confirmButtonText Văn bản nút xác nhận
   * @param cancelButtonText Văn bản nút hủy
   * @returns Promise<boolean> Trả về true nếu người dùng xác nhận, false nếu hủy
   */
  showConfirmation(
    title: string,
    text: string,
    icon: 'success' | 'error' | 'warning' | 'info' = 'warning',
    confirmButtonText: string = 'Đồng ý',
    cancelButtonText: string = 'Hủy'
  ): Promise<boolean> {
    return Swal.fire({
      title: title,
      text: text,
      icon: icon,
      showCancelButton: true,
      confirmButtonText: confirmButtonText,
      cancelButtonText: cancelButtonText,
      reverseButtons: true, // Đảo vị trí các nút
    }).then((result) => {
      return result.isConfirmed; // Trả về true nếu nhấn "Đồng ý", false nếu nhấn "Hủy"
    });
  }
}
