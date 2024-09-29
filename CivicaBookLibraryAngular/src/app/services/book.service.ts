import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Book } from '../models/Book';
import { Observable } from 'rxjs';
import { BookIssue } from '../models/bookIssue.model';
import { AddBook } from '../models/AddBook';
import { EditBook } from '../models/EditBook';

@Injectable({
  providedIn: 'root'
})
export class BookService {

  private apiUrl = 'http://localhost:5159/api/Book/';

  constructor(private http: HttpClient) { }

  getAllBooks(): Observable<ApiResponse<Book[]>> {
    return this.http.get<ApiResponse<Book[]>>(this.apiUrl + `GetAllBooks`);
  }

  getAllBooksByPagination(page: number, pageSize: number, sortOrder: string, search?: string, sortBy?: string): Observable<ApiResponse<Book[]>> {
    if (search != null && sortBy != null) {
      return this.http.get<ApiResponse<Book[]>>(this.apiUrl + `GetAllBooksByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search == null && sortBy != null){
      return this.http.get<ApiResponse<Book[]>>(this.apiUrl + `GetAllBooksByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else if(search != null && sortBy == null){
      return this.http.get<ApiResponse<Book[]>>(this.apiUrl + `GetAllBooksByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
    else {
      return this.http.get<ApiResponse<Book[]>>(this.apiUrl + `GetAllBooksByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    }
  }

  getBooksCount(search?:string){
    if(search != null){
      return this.http.get<ApiResponse<number>>(this.apiUrl + `GetBooksCount?search=${search}`);
    }
    else{
      return this.http.get<ApiResponse<number>>(this.apiUrl + `GetBooksCount`);
    }
  }
  addBook(addBook: AddBook): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}InsertBook`, addBook);
  }

  getBookById(bookId: number): Observable<ApiResponse<EditBook>> {
    return this.http.get<ApiResponse<EditBook>>(`${this.apiUrl}GetBookById?id=${bookId}`)
  }

  modifyBook(editBook: EditBook): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}ModifyBook`, editBook);
  }

  deleteBook(bookId: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}RemoveBook/${bookId}`)

  }
  bookIssue(bookIssue: BookIssue): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(this.apiUrl+'BookIssue', bookIssue);
  }
  returnBook(issuedId: number|undefined): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(this.apiUrl+'BookReturn/'+issuedId,issuedId);
    
  }
}
