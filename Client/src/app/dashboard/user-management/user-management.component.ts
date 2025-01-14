import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../../environment/notification.service';
import { Role } from '../../Shared/models/Role.model';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  users: any[] = [];
  isLoading = true;
  roles: Role[] = [];
  constructor(
    private userService: UserService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.fetchUsers();
    this.fetchRoles(); // Lấy danh sách roles khi component khởi tạo
  }

  // Lấy danh sách người dùng
  fetchUsers(): void {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (response) => {
        this.users = response.data;
        
        this.isLoading = false;
      },
      error: () => {
        this.notificationService.showNotification(
          'Lỗi',
          'Không thể tải danh sách người dùng',
          'error'
        );
        this.isLoading = false;
      }
    });
  }

  // Lấy danh sách quyền
  fetchRoles(): void {
    this.userService.getAllRoles().subscribe({
      next: (response) => {
        this.roles = response.data; // Gán dữ liệu vào danh sách role
      },
      error: () => {
        this.notificationService.showNotification(
          'Lỗi',
          'Không thể tải danh sách quyền',
          'error'
        );
      }
    });
  }

  // Cập nhật vai trò cho người dùng
  updateUserRole(userId: string, roleId: string): void {
    this.userService.updateUserRole(userId, roleId).subscribe({
      next: () => {
        this.notificationService.showNotification(
          'Thành công',
          'Vai trò người dùng đã được cập nhật!',
          'success'
        );
        this.fetchUsers();
      },
      error: () => {
        this.notificationService.showNotification(
          'Lỗi',
          'Không thể cập nhật vai trò người dùng',
          'error'
        );
      }
    });
  }

  // Xóa người dùng
  deleteUser(userId: string): void {
    if (confirm('Bạn có chắc chắn muốn xóa người dùng này không?')) {
      this.userService.deleteUser(userId).subscribe({
        next: () => {
          this.notificationService.showNotification(
            'Thành công',
            'Người dùng đã được xóa!',
            'success'
          );
          this.fetchUsers();
        },
        error: () => {
          this.notificationService.showNotification(
            'Lỗi',
            'Không thể xóa người dùng',
            'error'
          );
        }
      });
    }
  }
}
