import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-management',
  templateUrl: './management.component.html',
  styleUrl: './management.component.css'

})

export class ManagementComponent {
  constructor(private authService: AuthService, private router: Router) {}

  // Hàm gọi khi người dùng nhấn vào nút Đăng xuất
  onLogout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
