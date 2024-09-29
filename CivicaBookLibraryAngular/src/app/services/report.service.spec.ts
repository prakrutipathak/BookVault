import { TestBed } from '@angular/core/testing';

import { ReportService } from './report.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ReportUser } from '../models/reportuser.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { UserBookReport } from '../models/user-report.model';
import { AdminReportUser } from '../models/admin-report-user.model';
import { AdminBookReport } from '../models/admin-report-book.model';

describe('ReportService', () => {
  let service: ReportService;
  let httpMock: HttpTestingController;

  const mockApiResponse: ApiResponse<ReportUser[]>={
    success: true,
    data: [
      { 
        userId: 1,
        name: "report 1"
      },
      {
        userId: 2,
        name: "report 2"
      }
    ],
    message: ''
  };


  const mockSuccessResponse: ApiResponse<string>={
    success: true,
    data: "",
    message: "Book Saved Successfully"
  };

  const mockErrorResponse: ApiResponse<string>={
    success: false,
    data: "",
    message: "Book Already Exists"
  };

  const mockHttpError={
    status: 500,
    statusText: "Internal Server Error."
  };


  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, RouterTestingModule]
    });
    service = TestBed.inject(ReportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });


  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  //getBookCountForUser
  it('should fetch book count without selected date', () => {
    // Arrange
    const userId = 1;
    const type = 'issue';
    const mockResponse: ApiResponse<number> = { success: true, data: 5, message: 'Book count fetched successfully' };

    // Act
    service.getBookCountForUser(userId, null, type)
      .subscribe(response => {
        // Assert
        expect(response.data).toBe(5);
        expect(response.success).toBe(true);
      });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetBookCountForUser?userId=${userId}&type=${type}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch book count with selected date', () => {
    // Arrange
    const userId = 1;
    const selectedDate = '2024-06-30';
    const type = 'issue';
    const mockResponse: ApiResponse<number> = { success: true, data: 3, message: 'Book count fetched successfully' };

    // Act
    service.getBookCountForUser(userId, selectedDate, type)
      .subscribe(response => {
        // Assert
        expect(response.data).toBe(3);
        expect(response.success).toBe(true);
      });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetBookCountForUser?userId=${userId}&selectedDate=${selectedDate}&type=${type}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should handle HTTP errors', () => {
    //Arrange
    const userId = 1;
    const selectedDate = "10";
    const type = 'issue';

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getBookCountForUser(userId, selectedDate, type).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne(`http://localhost:5159/api/Report/GetBookCountForUser?userId=${userId}&selectedDate=${selectedDate}&type=${type}`);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });




  //getBookCountWithDateOrStudent
  it('should fetch book count without userId and issueDate', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: true, data: 10, message: 'Book count fetched successfully' };

    // Act
    service.getBookCountWithDateOrStudent(null, null)
      .subscribe(response => {
        // Assert
        expect(response.data).toBe(10);
        expect(response.success).toBe(true);
      });

    // Assert
    const apiUrl = 'http://localhost:5159/api/Report/GetBookCountWithDateOrStudent';
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch book count with issueDate and no userId', () => {
    // Arrange
    const issueDate = '2024-06-30';
    const mockResponse: ApiResponse<number> = { success: true, data: 5, message: 'Book count fetched successfully' };

    // Act
    service.getBookCountWithDateOrStudent(null, issueDate)
      .subscribe(response => {
        // Assert
        expect(response.data).toBe(5);
        expect(response.success).toBe(true);
      });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetBookCountWithDateOrStudent?issuedate=${issueDate}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch book count with userId and no issueDate', () => {
    // Arrange
    const userId = 1;
    const mockResponse: ApiResponse<number> = { success: true, data: 3, message: 'Book count fetched successfully' };

    // Act
    service.getBookCountWithDateOrStudent(userId, null)
      .subscribe(response => {
        // Assert
        expect(response.data).toBe(3);
        expect(response.success).toBe(true);
      });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetBookCountWithDateOrStudent?userId=${userId}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });




  //getUserWithBook
  it('should fetch users with book by bookId, page, and pageSize', () => {
    // Arrange
    const bookId = 1;
    const page = 1;
    const pageSize = 5;
    const type="issue";
    const mockResponse: ApiResponse<AdminReportUser[]> = {
      success: true,
      data: [
        { userId: 1, issueDate: '2024-06-30', user: { userId: 1, name: 'User A' } as ReportUser },
        { userId: 2, issueDate: '2024-07-01', user: { userId: 2, name: 'User B' } as ReportUser }
      ],
      message: 'Users with book fetched successfully'
    };

    // Act
    service.getUserWithBook(bookId,type, page, pageSize)
      .subscribe(response => {
        // Assert
        expect(response.success).toBe(true);
        expect(response.data.length).toBe(2);
        expect(response.data[0].user.name).toBe('User A');
        expect(response.data[1].user.name).toBe('User B');
      });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/AdminUserReport?bookId=${bookId}&type=${type}&page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch users with book without bookId when null', () => {
    // Arrange
    const bookId = null;
    const page = 1;
    const pageSize = 5;
    const type="issue";
    const mockResponse: ApiResponse<AdminReportUser[]> = {
      success: true,
      data: [
        { userId: 1, issueDate: '2024-06-30', user: { userId: 1, name: 'User A' } as ReportUser },
        { userId: 2, issueDate: '2024-07-01', user: { userId: 2, name: 'User B' } as ReportUser }
      ],
      message: 'Users with book fetched successfully'
    };

    // Act
    service.getUserWithBook(bookId,type, page, pageSize)
      .subscribe(response => {
        // Assert
        expect(response.success).toBe(true);
        expect(response.data.length).toBe(2);
        expect(response.data[0].user.name).toBe('User A');
        expect(response.data[1].user.name).toBe('User B');
      });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/AdminUserReport?bookId=${bookId}&type=${type}&page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should handle HTTP errors', () => {
    //Arrange
    const bookId = null;
    const page = 1;
    const pageSize = 5;
    const type="issue";

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getUserWithBook(bookId,type, page, pageSize).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne(`http://localhost:5159/api/Report/AdminUserReport?bookId=${bookId}&type=${type}&page=${page}&pageSize=${pageSize}`);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });




  //getUserCount
  it('should fetch user count by bookId', () => {
    // Arrange
    const bookId = 1;
    const type="issue";
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 5, 
      message: 'User count fetched successfully'
    };

    // Act
    service.getUserCount(bookId).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data).toBe(5); 
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetUserCount?bookId=${bookId}&type=${type}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should handle null bookId gracefully', () => {
    // Arrange
    const bookId = null;
    const type="issue";
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 0, 
      message: 'User count fetched successfully'
    };

    // Act
    service.getUserCount(bookId).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data).toBe(0); 
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetUserCount?bookId=${bookId}&type=${type}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should handle server error', () => {
    // Arrange
    const bookId = 1;
    const type="issue";
    const mockErrorResponse: ApiResponse<number> = {
      success: false,
      data: 0,
      message: 'Failed to fetch user count'
    };

    // Act
    service.getUserCount(bookId).subscribe(
      () => fail('expected an error, not user count'),
      (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal server error');
      }
    );

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/GetUserCount?bookId=${bookId}&type=${type}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush('Server error', { status: 500, statusText: 'Internal server error' });
  });




  //getIssueBookWithIssueDateOrUser
  it('should fetch issue books without userId and issueDate', () => {
    // Arrange
    const page = 1;
    const pageSize = 2;
    const mockResponse: ApiResponse<AdminBookReport[]> = {
      success: true,
      data: [
        { title: 'Book 1', author: 'Author 1', issueDate: '2024-06-28', userId: 1, user: { userId: 1, name: 'User 1' } },
        { title: 'Book 2', author: 'Author 2', issueDate: '2024-06-27', userId: 2, user: { userId: 2, name: 'User 2' } }
      ],
      message: 'Issue books fetched successfully'
    };

    // Act
    service.getIssueBookWithIssueDateOrUser(null, null, page, pageSize).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data.length).toBe(2); 
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/AdminBookReport?page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch issue books with issueDate and default page settings', () => {
    // Arrange
    const issueDate = '2024-06-27';
    const mockResponse: ApiResponse<AdminBookReport[]> = {
      success: true,
      data: [
        { title: 'Book 1', author: 'Author 1', issueDate: '2024-06-27', userId: 1, user: { userId: 1, name: 'User 1' } },
        { title: 'Book 2', author: 'Author 2', issueDate: '2024-06-27', userId: 2, user: { userId: 2, name: 'User 2' } }
      ],
      message: 'Issue books fetched successfully'
    };

    // Act
    service.getIssueBookWithIssueDateOrUser(null, issueDate).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data.length).toBe(2); // Check against the mock data length
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/AdminBookReport?issuedate=${issueDate}&page=1&pageSize=2`; // Default page settings
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch issue books with userId and custom page settings', () => {
    // Arrange
    const userId = 1;
    const page = 2;
    const pageSize = 3;
    const mockResponse: ApiResponse<AdminBookReport[]> = {
      success: true,
      data: [
        { title: 'Book 1', author: 'Author 1', issueDate: '2024-06-28', userId: 1, user: { userId: 1, name: 'User 1' } },
        { title: 'Book 2', author: 'Author 2', issueDate: '2024-06-27', userId: 1, user: { userId: 1, name: 'User 1' } }
      ],
      message: 'Issue books fetched successfully'
    };

    // Act
    service.getIssueBookWithIssueDateOrUser(userId, null, page, pageSize).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data.length).toBe(2); 
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/AdminBookReport?userId=${userId}&page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should handle server error', () => {
    // Arrange
    const userId = 1;
    const page = 1;
    const pageSize = 2;
    const mockErrorResponse: ApiResponse<AdminBookReport[]> = {
      success: false,
      data: [],
      message: 'Failed to fetch issue books'
    };

    // Act
    service.getIssueBookWithIssueDateOrUser(userId, null).subscribe(
      () => fail('expected an error, not issue books'),
      (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal server error');
      }
    );

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/AdminBookReport?userId=${userId}&page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush('Server error', { status: 500, statusText: 'Internal server error' });
  });





  //issueBooksReport
  it('should fetch issue books without selectedDate', () => {
    // Arrange
    const userId = 1;
    const type = 'issue';
    const page = 1;
    const pageSize = 2;
    const mockResponse: ApiResponse<UserBookReport[]> = {
      success: true,
      data: [
        { title: 'Book 1', author: 'Author 1', issueDate: '2024-06-28', returnDate: null, issueId: 1 },
        { title: 'Book 2', author: 'Author 2', issueDate: '2024-06-27', returnDate: '2024-07-05', issueId: 2 }
      ],
      message: 'Issue books fetched successfully'
    };

    // Act
    service.issueBooksReport(userId, null, type, page, pageSize).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data.length).toBe(2); 
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/UserBookReport?userId=${userId}&type=${type}&page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });


  it('should fetch issue books with selectedDate', () => {
    // Arrange
    const userId = 1;
    const selectedDate = '2024-06-27';
    const type = 'return';
    const page = 1;
    const pageSize = 2;
    const mockResponse: ApiResponse<UserBookReport[]> = {
      success: true,
      data: [
        { title: 'Book 1', author: 'Author 1', issueDate: '2024-06-27', returnDate: '2024-07-05', issueId: 1 },
        { title: 'Book 2', author: 'Author 2', issueDate: '2024-06-27', returnDate: '2024-07-04', issueId: 2 }
      ],
      message: 'Issue books fetched successfully'
    };

    // Act
    service.issueBooksReport(userId, selectedDate, type, page, pageSize).subscribe(response => {
      // Assert
      expect(response.success).toBe(true);
      expect(response.data.length).toBe(2); 
    });

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/UserBookReport?userId=${userId}&selectedDate=${selectedDate}&type=${type}&page=${page}&pageSize=${pageSize}`;
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush(mockResponse);
  });

  
  it('should handle server error', () => {
    // Arrange
    const userId = 1;
    const mockErrorResponse: ApiResponse<UserBookReport[]> = {
      success: false,
      data: [],
      message: 'Failed to fetch issue books'
    };

    // Act
    service.issueBooksReport(userId, null).subscribe(
      () => fail('expected an error, not issue books'),
      (error) => {
        // Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal server error');
      }
    );

    // Assert
    const apiUrl = `http://localhost:5159/api/Report/UserBookReport?userId=${userId}&type=issue&page=1&pageSize=2`; 
    const mockRequest = httpMock.expectOne(apiUrl);
    expect(mockRequest.request.method).toBe('GET');
    mockRequest.flush('Server error', { status: 500, statusText: 'Internal server error' });
  });

});
