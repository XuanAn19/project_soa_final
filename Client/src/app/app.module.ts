import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/component/login/login.component';
import { RegisterComponent } from './auth/component/register/register.component';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { HTTP_INTERCEPTORS, HttpClientModule, provideHttpClient, withFetch } from '@angular/common/http';
import { AuthGuardService } from './guards/auth-guard.service';
import { NgxUiLoaderModule, NgxUiLoaderService } from 'ngx-ui-loader';
import { ManagementComponent } from './dashboard/management/management.component';
import { CommonModule } from '@angular/common';
import { LoadingSpinnerComponent } from './dashboard/loading-spinner/loading-spinner.component';
import { BookListComponent } from './dashboard/book-list/book-list.component';
import { BookDetailComponent } from './book-detail/book-detail.component';
import { InterceptorService } from './services/interceptor.service';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { DashboardComponent } from './dashboard/dashboard/dashboard.component';
import { UserManagementComponent } from './dashboard/user-management/user-management.component';
import { UserInfomationComponent } from './component/user-infomation/user-infomation.component';
import { RoleManagementComponent } from './dashboard/role-management/role-management.component';
import { BorrowComponent } from './borrow/borrow.component';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    ManagementComponent,
    LoadingSpinnerComponent,
    BookListComponent,
    BookDetailComponent,
    DashboardComponent,
    UserManagementComponent,
    UserInfomationComponent,
    RoleManagementComponent,
    BorrowComponent,

  ],
  imports: [
    HttpClientModule,
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    BrowserAnimationsModule,
    NzAlertModule,
    NzButtonModule,
    NgxUiLoaderModule,
    CommonModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: InterceptorService,
      multi: true,
    },
    AuthGuardService,
    provideHttpClient(withFetch()),
    provideAnimationsAsync(),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule { }
