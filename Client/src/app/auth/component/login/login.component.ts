import { Component } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { NotificationService } from '../../../../environment/notification.service';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private _auth: AuthService, private _router: Router,private notificationService: NotificationService) { }

  ngOnInit(): void { }

  onSubmit(): void {
    this.loading = true;
    this.error = '';
    if (!this.canSubmit()) {
      this.notificationService.showNotification("Thông báo", "Vui lòng nhập thông tin đăng nhập.", 'warning', 2000);
    }
    else {
      this._auth.login({Email: this.email, Password: this.password}).subscribe(
        (res) =>{
          console.log(res)
          this.loading = false;
          this.notificationService.showNotification("Thông báo", "Đăng nhập thành công.", "success" , 2000);
          //this._router.navigate(['/admin/product-management']);
          this._router.navigate(['/']);

        },
        (err) => {
          console.log(err);
          this.loading = false;
         this.notificationService.showNotification("Thông báo", err, "error" , 3000);
        }
      )
    }
  }

  canSubmit(): boolean {
    return this.email.length > 0 && this.password.length > 0;
  }
}
