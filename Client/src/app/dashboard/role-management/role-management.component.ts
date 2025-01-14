import { Component, OnInit } from '@angular/core';
import { RoleService } from '../../services/role.service';
import { Router } from 'express';
import { NotificationService } from '../../../environment/notification.service';

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrl: './role-management.component.css'
})
export class RoleManagementComponent  implements OnInit {
  roles: any[] = [];
  isLoading: boolean = false;
  roleName: string = '';  // Tên quyền
  roleDescription: string = '';  // Mô tả quyền
  isPopupVisible: boolean = false;  // Điều khiển việc hiển thị form thêm quyền
  editingRoleId: string | null = null;  // ID quyền khi đang chỉnh sửa
  editingRoleName: string = '';  // Tên quyền đang chỉnh sửa
  editingRoleDescription: string = '';  // Mô tả quyền đang chỉnh sửa
  constructor(
    private roleService: RoleService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.loadRoles();
  }

  // Lấy tất cả quyền
  loadRoles() {
    this.isLoading = true;
    this.roleService.getAllRoles().subscribe(
      response => {
        this.roles = response.data;
        this.isLoading = false;
      },
      error => {
        console.error('Error fetching roles', error);
        this.isLoading = false;
      }
    );
  }



openCreateRolePopup() {
  this.isPopupVisible = true; // Hiển thị form
  this.editingRoleId = null; // Đặt lại ID nếu là thêm mới
  this.editingRoleName = ''; // Đặt lại tên
  this.editingRoleDescription = ''; // Đặt lại mô tả
}

startEditing(role: any) {
  this.isPopupVisible = true; // Hiển thị form
  this.editingRoleId = role.id; // Gán ID để chỉnh sửa
  this.editingRoleName = role.name; // Gán tên
  this.editingRoleDescription = role.description; // Gán mô tả
}

  // Đóng form thêm quyền (popup)
  closeCreateRolePopup() {
    this.isPopupVisible = false;
  }

  // Lưu quyền (thêm mới hoặc chỉnh sửa)
  saveRole() {
    const role = {
      name: this.editingRoleName || this.roleName,
      description: this.editingRoleDescription || this.roleDescription
    };

    if (this.editingRoleId) {
      // Cập nhật quyền
      this.updateRole(role);
    } else {
      // Thêm quyền mới
      this.createRole(role);
    }
  }

  // Cập nhật quyền
  updateRole(role: any) {
    if (this.editingRoleId) {
      this.roleService.updateRole(this.editingRoleId, role).subscribe(
        response => {
          this.loadRoles();
          this.closeCreateRolePopup();
          this.notificationService.showNotification('Thành công', 'Quyền đã được cập nhật!', 'success');
        },
        error => {
          console.error('Error updating role', error);
          this.notificationService.showNotification('Lỗi', 'Không thể cập nhật quyền.', 'error');
        }
      );
    }
  }

  // Thêm quyền mới
  createRole(role: any) {
    this.roleService.createRole(role).subscribe(
      response => {
        this.loadRoles();
        this.closeCreateRolePopup();
        this.notificationService.showNotification('Thành công', 'Quyền đã được thêm!', 'success');
      },
      error => {
        console.error('Error creating role', error);
        this.notificationService.showNotification('Lỗi', 'Không thể tạo quyền.', 'error');
      }
    );
  }

  // Xóa quyền
  deleteRole(id: string) {
    this.notificationService.showConfirmation('Xác nhận', 'Bạn có chắc chắn muốn xóa quyền này?', 'warning', 'Xóa', 'Hủy')
      .then((confirmed) => {
        if (confirmed) {
          this.roleService.deleteRole(id).subscribe(
            response => {
              this.loadRoles();
              this.notificationService.showNotification('Thành công', 'Quyền đã được xóa!', 'success');
            },
            error => {
              console.error('Error deleting role', error);
              this.notificationService.showNotification('Lỗi', 'Không thể xóa quyền.', 'error');
            }
          );
        } else {
          this.notificationService.showNotification('Hủy bỏ', 'Hành động xóa quyền đã bị hủy!', 'info');
        }
      });
  }
}
