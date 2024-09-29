import { TestBed } from '@angular/core/testing';

import { SecurityquestionService } from './securityquestion.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiResponse } from '../models/ApiResponse{T}';
import { SecurityQuestion } from '../models/securityQuestion.model';

describe('SecurityquestionService', () => {
  let service: SecurityquestionService;
  let httpMock: HttpTestingController;

  const mockApiResponse: ApiResponse<SecurityQuestion[]> = {
    success: true,
    data:[
      {
        passwordHint: 1,
        question: 'TestQuestion1'
      },
      {
        passwordHint: 2,
        question: 'TestQuestion2'
      }
    ],
    message : ''
  };
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule],
      providers: [SecurityquestionService]
    });

    service = TestBed.inject(SecurityquestionService);
    httpMock = TestBed.inject(HttpTestingController);

  });
  afterEach(()=>{
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all questions successfully',()=>{
    // Arrange
    const apiUrl ='http://localhost:5159/api/SecurityQuestion/';

    //Act
    service.getAllQuestions().subscribe((response) => {
      //Assert
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl+"GetAllSecurityQuestions");
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });

  it('should handle an empty categories list', () => {
    // Arrange
    const apiUrl = "http://localhost:5159/api/SecurityQuestion/"
 
    const emptyResponse: ApiResponse<SecurityQuestion[]> = {
      success: true,
      data: [],
      message: ''
    }
 
    // Act
    service.getAllQuestions().subscribe((response) => {
      // Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });
 
    const req = httpMock.expectOne(apiUrl+"GetAllSecurityQuestions");
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle HTTP error gracefully',()=>{
    // Arrange
    const apiUrl = "http://localhost:5159/api/SecurityQuestion/"
    const errorMessage = 'Faild to load categories';
    
    //Act 
    service.getAllQuestions().subscribe(()=>
      fail('expect an error, not questions'),
      (error) =>{
        // Asssertttttt
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    );
    const req = httpMock.expectOne(apiUrl+"GetAllSecurityQuestions");
    expect(req.request.method).toBe('GET');

    // respond with error
    req.flush(errorMessage,{status:500,statusText:'Internal Server Error'});
  });
});
