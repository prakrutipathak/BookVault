import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SigninComponent } from './components/auth/signin/signin.component';
import { NavbarComponent } from './components/shared/navbar/navbar.component';
import { FooterComponent } from './components/shared/footer/footer.component';
import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { BooksListComponent } from './components/admin/books/books-list/books-list.component';
import { BookissueComponent } from './components/client/bookissue/bookissue.component';
import { authGuard } from './guards/auth.guard';
import { ForgetPasswordComponent } from './components/auth/forget-password/forget-password.component';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { BookDetailsComponent } from './components/admin/books/book-details/book-details.component';
import { BookAddComponent } from './components/admin/books/book-add/book-add.component';
import { BookModifyComponent } from './components/admin/books/book-modify/book-modify.component';
import { AdminbookreportComponent } from './components/report/adminbookreport/adminbookreport.component';
import { UserbookreportComponent } from './components/report/userbookreport/userbookreport.component';
import { UserListComponent } from './components/admin/users/user-list/user-list.component';
import { adminGuard } from './guards/admin.guard';
import { SignupsuccessComponent } from './components/auth/signupsuccess/signupsuccess.component';
import { userGuard } from './guards/user.guard';

const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path:'signin', component: SigninComponent},
  {path:'signup', component: SignupComponent},
  {path:'signupsuccess', component: SignupsuccessComponent},
  {path: 'navbar', component: NavbarComponent},
  {path: 'footer', component: FooterComponent},
  {path: 'home', component: HomeComponent},
  {path: 'privacy', component: PrivacyComponent},
  {path: 'bookList', component: BooksListComponent, canActivate: [authGuard, adminGuard]},
  {path: 'bookissue', component: BookissueComponent,canActivate:[authGuard]},
  {path: 'forgetPassword', component: ForgetPasswordComponent},
  {path: 'changepassword', component: ChangePasswordComponent,canActivate:[authGuard]},
  {path: 'bookDetail/:BookId', component: BookDetailsComponent, canActivate: [authGuard, adminGuard]},
  {path: 'addBook', component: BookAddComponent, canActivate: [authGuard, adminGuard]},
  {path: 'modifyBook/:BookId', component: BookModifyComponent, canActivate: [authGuard, adminGuard]},
  {path: 'admin-book-report', component: AdminbookreportComponent, canActivate: [authGuard, adminGuard]},
  {path: 'user-book-report', component: UserbookreportComponent,canActivate:[authGuard,userGuard]},
  {path: 'userlist', component: UserListComponent, canActivate: [authGuard, adminGuard]},

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
