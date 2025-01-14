import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../../environment/notification.service';

@Component({
  selector: 'app-user-infomation',
  templateUrl: './user-infomation.component.html',
  styleUrls: ['./user-infomation.component.css'],
})
export class UserInfomationComponent implements OnInit {
  userInfo: any = {
    fullName: '',
    phoneNumber: '',
    address: {
      provice: '',
      district: '',
      ward: '',
      detail: '',
    },
  };
  isEditing: boolean = false;

  constructor(
    private userService: UserService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
  }

  // Lấy thông tin người dùng
  loadUserInfo(): void {
    this.userService.getUserInfo().subscribe({
      next: (response) => {
        if (response.success) {
          this.userInfo = {
            ...this.userInfo,
            ...response.data,

          };
          console.log('User Info:', this.userInfo); // Debug: Kiểm tra dữ liệu

        } else {
          this.notificationService.showNotification(
            'Thông báo',
            response.message || 'Không thể tải thông tin người dùng',
            'warning'
          );
        }
      },
      error: (err) => {
        console.error('Lỗi khi lấy thông tin người dùng:', err);
        this.notificationService.showNotification(
          'Lỗi',
          'Không thể tải thông tin người dùng. Vui lòng thử lại!',
          'error'
        );
      },
    });
  }

  // Bật/Tắt chế độ chỉnh sửa
  toggleEditMode(): void {
    this.isEditing = !this.isEditing;
  }

  // Cập nhật thông tin người dùng
  updateUserInfo(): void {
    this.notificationService
      .showConfirmation('Xác nhận', 'Bạn có chắc chắn muốn cập nhật thông tin cá nhân không?', 'warning')
      .then((confirmed) => {
        if (confirmed) {
          const updateData = {
            fullName: this.userInfo.fullName,
            phoneNumber: this.userInfo.phoneNumber,
            address: {
              provice: this.userInfo.address.provice,
              district: this.userInfo.address.district,
              ward: this.userInfo.address.ward,
              detail: this.userInfo.address.detail,
            },
          };

          this.userService.updateUserInfo(this.userInfo.email, updateData).subscribe({
            next: () => {
              this.notificationService.showNotification(
                'Thành công',
                'Thông tin cá nhân đã được cập nhật thành công!',
                'success'
              );
              this.isEditing = false; // Tắt chế độ chỉnh sửa sau khi cập nhật thành công
            },
            error: (err) => {
              console.error('Lỗi khi cập nhật thông tin người dùng:', err);
              this.notificationService.showNotification(
                'Lỗi',
                'Cập nhật thông tin thất bại. Vui lòng thử lại!',
                'error'
              );
            },
          });
        }
      });
  }
}
