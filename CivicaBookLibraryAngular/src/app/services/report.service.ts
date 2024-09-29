import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Observable } from 'rxjs';
import { AdminBookReport } from '../models/admin-report-book.model';
import { AdminReportUser } from '../models/admin-report-user.model';
import { UserBookReport } from '../models/user-report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = 'http://localhost:5159/api/Report';

  constructor(private http: HttpClient) { }



  issueBooksReport(userId: number | undefined, selectedDate?: string | null, type: string = 'issue', page: number = 1, pageSize: number = 2): Observable<ApiResponse<UserBookReport[]>> {
    if (selectedDate == null) {
      return this.http.get<ApiResponse<UserBookReport[]>>(this.apiUrl + `/UserBookReport?userId=${userId}&type=${type}&page=${page}&pageSize=${pageSize}`);
    } else {

      return this.http.get<ApiResponse<UserBookReport[]>>(this.apiUrl + `/UserBookReport?userId=${userId}&selectedDate=${selectedDate}&type=${type}&page=${page}&pageSize=${pageSize}`);
    }

  }

  getBookCountForUser(userId: number | undefined, selectedDate?: string | null, type: string = 'issue'): Observable<ApiResponse<number>> {
    if (selectedDate == null) {
      return this.http.get<ApiResponse<number>>(this.apiUrl + `/GetBookCountForUser?userId=${userId}&type=${type}`);
    } else {

      return this.http.get<ApiResponse<number>>(this.apiUrl + `/GetBookCountForUser?userId=${userId}&selectedDate=${selectedDate}&type=${type}`);
    }
  }

  getIssueBookWithIssueDateOrUser(userId?: number | null, issueDate?: string | null, page: number = 1, pageSize: number = 2): Observable<ApiResponse<AdminBookReport[]>> {
    if (userId == null && issueDate == null) {
      return this.http.get<ApiResponse<AdminBookReport[]>>(this.apiUrl + `/AdminBookReport?page=${page}&pageSize=${pageSize}`);
    }
    else if (userId == null) {
      return this.http.get<ApiResponse<AdminBookReport[]>>(this.apiUrl + `/AdminBookReport?issuedate=${issueDate}&page=${page}&pageSize=${pageSize}`);
    }

    else {
      return this.http.get<ApiResponse<AdminBookReport[]>>(this.apiUrl + `/AdminBookReport?userId=${userId}&page=${page}&pageSize=${pageSize}`);
    }

  }

  getBookCountWithDateOrStudent(userId?: number | null, issueDate?: string | null): Observable<ApiResponse<number>> {
    if (userId == null && issueDate == null) {
      return this.http.get<ApiResponse<number>>(this.apiUrl + `/GetBookCountWithDateOrStudent`);
    }
    else if (userId == null) {
      return this.http.get<ApiResponse<number>>(this.apiUrl + `/GetBookCountWithDateOrStudent?issuedate=${issueDate}`);
    }

    else {
      return this.http.get<ApiResponse<number>>(this.apiUrl + `/GetBookCountWithDateOrStudent?userId=${userId}`);
    }
  }

  getUserWithBook(bookId: number | null,type: string = 'issue', page: number, pageSize: number): Observable<ApiResponse<AdminReportUser[]>> {
    return this.http.get<ApiResponse<AdminReportUser[]>>(this.apiUrl + `/AdminUserReport?bookId=${bookId}&type=${type}&page=${page}&pageSize=${pageSize}`);

  }

  getUserCount(bookId: number | null,type: string = 'issue'): Observable<ApiResponse<number>> {
    return this.http.get<ApiResponse<number>>(this.apiUrl + `/GetUserCount?bookId=${bookId}&type=${type}`);

  }


}
