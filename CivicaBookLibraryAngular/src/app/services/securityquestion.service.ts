import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/ApiResponse{T}';
import { SecurityQuestion } from '../models/securityQuestion.model';

@Injectable({
  providedIn: 'root'
})
export class SecurityquestionService {

  private apiUrl = "http://localhost:5159/api/SecurityQuestion/";
  constructor(private http: HttpClient) { }
 
  getAllQuestions():Observable<ApiResponse<SecurityQuestion[]>>{
    return this.http.get<ApiResponse<SecurityQuestion[]>>(this.apiUrl+'GetAllSecurityQuestions')
  }}
