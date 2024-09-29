import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FooterComponent } from './components/shared/footer/footer.component';
import { NavbarComponent } from './components/shared/navbar/navbar.component';
import { SigninComponent } from './components/auth/signin/signin.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AuthService } from './services/auth.service';
import { AuthInterceptor } from './interceptor/auth.interceptor';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { BooksListComponent } from './components/admin/books/books-list/books-list.component';
import { BookissueComponent } from './components/client/bookissue/bookissue.component';
import { ForgetPasswordComponent } from './components/auth/forget-password/forget-password.component';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { BookDetailsComponent } from './components/admin/books/book-details/book-details.component';
import { BookAddComponent } from './components/admin/books/book-add/book-add.component';
import { BookModifyComponent } from './components/admin/books/book-modify/book-modify.component';
import { AdminbookreportComponent } from './components/report/adminbookreport/adminbookreport.component';
import { UserbookreportComponent } from './components/report/userbookreport/userbookreport.component';
import { UserListComponent } from './components/admin/users/user-list/user-list.component';
import { DatePipe } from '@angular/common';
import { CapitalizePipe } from './pipe/capitalize.pipe';
import { SignupsuccessComponent } from './components/auth/signupsuccess/signupsuccess.component';

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    NavbarComponent,
    SigninComponent,
    HomeComponent,
    PrivacyComponent,
    SignupComponent,
    ForgetPasswordComponent,
    ChangePasswordComponent,
    BooksListComponent,
    BookissueComponent,
    BookDetailsComponent,
    BookAddComponent,
    UserListComponent,
    BookModifyComponent,
    AdminbookreportComponent,
    UserbookreportComponent,
    CapitalizePipe,
    SignupsuccessComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    NgbModule
  ],
  providers: [DatePipe,AuthService,{provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
