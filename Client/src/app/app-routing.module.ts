import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './auth/component/login/login.component';
import { RegisterComponent } from './auth/component/register/register.component';
import { AuthGuardService } from './guards/auth-guard.service';
import { ManagementComponent } from './dashboard/management/management.component';
import { BookListComponent } from './dashboard/book-list/book-list.component';
import { UserManagementComponent } from './dashboard/user-management/user-management.component';
import { RoleManagementComponent } from './dashboard/role-management/role-management.component';
import { UserInfomationComponent } from './component/user-infomation/user-infomation.component';
import { BorrowComponent } from './borrow/borrow.component';
import { BookDetailComponent } from './book-detail/book-detail.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent  },
  { path: 'register', component: RegisterComponent },
  { path: 'book/:id', component: BookDetailComponent },
  { path: 'user/infomation', component: UserInfomationComponent },

  {
    path: 'admin/dashboard',
    component: ManagementComponent,
    canActivate: [AuthGuardService],
    data: { role: "admin" },
    children: [
      { path: 'books', component: BookListComponent },
      { path: 'users', component: UserManagementComponent },
      { path: 'roles', component: RoleManagementComponent },
      { path: 'profiles', component: UserInfomationComponent },
      { path: 'borrow', component: BorrowComponent },
    ],
   }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
