import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LocalstorageService } from './helpers/localstorage.service';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiResponse } from '../models/ApiResponse{T}';
import { LocalStorageKeys } from './helpers/localstoragekeys';
import { RegisterUser } from '../models/registeruser.model';
import { ResetPassword } from '../models/reset-password.model';
import { ChangePassword } from '../models/change-password.model';
import { User } from '../models/user.model';
import { Router } from '@angular/router';



@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5159/api/User/';

  constructor(private http: HttpClient, private localStorageHelper: LocalstorageService, private router: Router) { }

  private authState = new BehaviorSubject<boolean>(this.localStorageHelper.hasItem(LocalStorageKeys.TokenName));

  private usernameSubject = new BehaviorSubject<string | null | undefined>(this.localStorageHelper.getItem(LocalStorageKeys.LoginId));
  private userIdSubject = new BehaviorSubject<string | null | undefined>(this.localStorageHelper.getItem(LocalStorageKeys.UserId));

  signUp(user: RegisterUser): Observable<ApiResponse<string>> {
    const body = user;
    return this.http.post<ApiResponse<string>>(this.apiUrl + "Register", body)
  }

  signIn(username: string, password: string): Observable<ApiResponse<string>> {
    const body={username, password};
    return this.http.post<ApiResponse<string>>(this.apiUrl+"Login", body).pipe(
      tap(response => {
        if(response.success){

          const token = response.data;
 
          const payload = token.split('.')[1];
          const decodedPayload = JSON.parse(atob(payload));
          const userid = decodedPayload.UserId;

          this.localStorageHelper.setItem(LocalStorageKeys.TokenName, token);
          this.localStorageHelper.setItem(LocalStorageKeys.LoginId, username);
          this.localStorageHelper.setItem(LocalStorageKeys.UserId, userid);

          //this.authState.next(true);
          
          
          this.authState.next(this.localStorageHelper.hasItem(LocalStorageKeys.TokenName));
          this.usernameSubject.next(username);
          this.userIdSubject.next(userid);
        }
      }),
    );
  }

  //service for signout
  signOut(){
    this.localStorageHelper.removeItem(LocalStorageKeys.TokenName);
    this.localStorageHelper.removeItem(LocalStorageKeys.LoginId);
    this.localStorageHelper.removeItem(LocalStorageKeys.UserId);
    this.router.navigate(['/home']);
    this.authState.next(false);
    this.usernameSubject.next(null);
    this.userIdSubject.next(null);
  }

  getUsername(): Observable< string | null | undefined> {
    return this.usernameSubject.asObservable();
  }
  getUserId(): Observable< string | null | undefined> {
    return this.userIdSubject.asObservable();
  }

  isAuthenticated(){
    return this.authState.asObservable();
  }

  resetPassword(resetPassword: ResetPassword): Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(this.apiUrl+ 'ResetPassword',resetPassword)
  }

  changePassword(changePassword: ChangePassword): Observable<ApiResponse<string>>{
    return this.http.put<ApiResponse<string>>(this.apiUrl+ 'ChangePassword',changePassword)
  }

  getAllUsersByPagination(page: number, pageSize: number, sortOrder: string, search?: string): Observable<ApiResponse<User[]>> {
    if (search != null) {
      return this.http.get<ApiResponse<User[]>>(this.apiUrl + `GetAllUsersByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(sortOrder == 'default') {
      return this.http.get<ApiResponse<User[]>>(this.apiUrl + `GetAllUsersByPagination?page=${page}&pageSize=${pageSize}`);
    }
    else{
      return this.http.get<ApiResponse<User[]>>(this.apiUrl + `GetAllUsersByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
  }
  getUsersCount(search?:string){
    if(search != null){
      return this.http.get<ApiResponse<number>>(this.apiUrl + `GetUsersCount?search=${search}`);
    }
    else{
      return this.http.get<ApiResponse<number>>(this.apiUrl + `GetUsersCount`);
    }
  }
  getAllUsers(): Observable<ApiResponse<User[]>> {
    return this.http.get<ApiResponse<User[]>>(this.apiUrl + `GetAllUsers`);
  }
  deleteUser(userId: number): Observable<ApiResponse<User>> {
    return this.http.delete<ApiResponse<User>>(`${this.apiUrl}DeleteUser/${userId}`)

  }

  getUserById(userId: number | undefined): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(this.apiUrl + 'GetUserByLoginId/' + userId);
  }


}
