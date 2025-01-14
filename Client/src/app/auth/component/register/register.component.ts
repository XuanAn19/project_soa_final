import { Component } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';
import { NotificationService } from '../../../../environment/notification.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  userRegister = {
    fullname: '',
    email: '',
    password: ''
  };

  codeConfirm = {
    code: ''
  };

  sendCode: boolean = false;
  otpSent: boolean = false;

  constructor(
    private _auth: AuthService,
    private _notification: NotificationService,
    private router: Router,
    //private snack: MatSnackBar,
    private ngxService: NgxUiLoaderService
  ) {}

  ngOnInit(): void {}

  toSignup(){
    this.otpSent = !this.otpSent;
    window.location.reload();
  }
  confirmCodeEmail(): void {
    const confirmData = {
      Email: this.userRegister.email,
      Code: this.codeConfirm.code
    };
    this.ngxService.start();
    this._auth.codeConfirmEmail(confirmData).subscribe(
      response => {
        this.ngxService.stop();
        if (response.status === 'ok') {
          this.sendCode = true;
          this._notification.showNotification("Thông báo", "Xác minh code thành công. ", "success", 3000);

        } else {
          this._notification.showNotification("Thông báo", "Mã xác minh không đúng. Vui lòng thử lại. ", "error", 3000);
        }
      },
      error => {
        this._notification.showNotification("Thông báo", "Không có mã tồn tại. ", "error", 3000);
      }
    );
  }

  signupUser(): void {
    this.ngxService.start();
    this._auth.register(this.userRegister).subscribe(
      (response: any) => {
        this.ngxService.stop();

        if (response.status === "ok") {
          console.log('User registered successfully:', response);
          this._notification.showNotification("Thông báo", response.message, "success", 3000);
          this.otpSent = true;  // Hiển thị form xác nhận
        } else if (response.status === "no" && response.message === "Email này đã hoạt động.") {
          console.warn('Registration failed:', response);
          this._notification.showNotification("Thông báo", response.message, "error", 3000);
        }
      },
      error => {
        this.ngxService.stop();
        console.error('Error during registration:', error);
        this._notification.showNotification("Thông báo", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.", "error", 3000);
      }
    );
  }
}
