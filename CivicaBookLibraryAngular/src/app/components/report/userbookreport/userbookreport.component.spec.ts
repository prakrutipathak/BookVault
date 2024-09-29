import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserbookreportComponent } from './userbookreport.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { ChangeDetectorRef } from '@angular/core';
import { BookService } from 'src/app/services/book.service';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { UserBookReport } from 'src/app/models/user-report.model';

describe('UserbookreportComponent', () => {
  let component: UserbookreportComponent;
  let fixture: ComponentFixture<UserbookreportComponent>;
  let reportServiceSpy: jasmine.SpyObj<ReportService>;
  let bookServiceSpy: jasmine.SpyObj<BookService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;

  const mockBooks: UserBookReport[] = [

    {
      title: 'title 1',
      author: 'author 1',
      issueDate: '2024-07-09',
      returnDate: null,
      issueId: 1
    },
    {
      title: 'title 2',
      author: 'author 2',
      issueDate: '2024-07-10',
      returnDate: null,
      issueId: 2
    }

  ];

  beforeEach(() => {
    reportServiceSpy = jasmine.createSpyObj('ReportService', ['getBookCountForUser', 'issueBooksReport']);
    bookServiceSpy = jasmine.createSpyObj('BookService', ['returnBook']);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getUserId']);
    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    TestBed.configureTestingModule({
      declarations: [UserbookreportComponent],
      imports: [HttpClientTestingModule, RouterTestingModule, FormsModule],
      providers: [
        { provide: ReportService, useValue: reportServiceSpy },
        { provide: BookService, useValue: bookServiceSpy },
        { provide: ChangeDetectorRef, useValue: cdrSpy }
      ],
    });
    fixture = TestBed.createComponent(UserbookreportComponent);
    component = fixture.componentInstance;
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  //getBookCountForUser

  it('should calculate isseud book count', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: true, data: 2, message: '' };
    reportServiceSpy.getBookCountForUser.and.returnValue(of(mockResponse));
    spyOn(component, 'calculateTotalPages').and.stub();

    // Act
    component.loadBookCount();

    // Assert
    expect(reportServiceSpy.getBookCountForUser).toHaveBeenCalledWith(undefined, null,'issue');
    expect(component.totalBooks).toBe(mockResponse.data);
    expect(component.loading).toBeFalse();
  });

  it('should handle unsuccessful response when fetching isseud book  count', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = { success: false, data: 2, message: '' };
    reportServiceSpy.getBookCountForUser.and.returnValue(of(mockResponse));
    spyOn(console, 'error');

    // Act
    component.loadBookCount();

    // Assert
    expect(reportServiceSpy.getBookCountForUser).toHaveBeenCalledWith(undefined, null,'issue');
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books count', '');
    expect(component.loading).toBeFalse();
  });

  it('should handle error when fetching isseud book  count', () => {
    // Arrange
    const mockError = new Error('Network error');
    reportServiceSpy.getBookCountForUser.and.returnValue(throwError(mockError));
    spyOn(console, 'error');

    // Act
    component.loadBookCount();

    // Assert
    expect(reportServiceSpy.getBookCountForUser).toHaveBeenCalledWith(undefined, null,'issue');
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books count', mockError);
    expect(component.loading).toBeFalse();
  });


  // issueBooksReport
  it('should load all issue book with pagination', () => {
    // Arrange
    const mockResponse: ApiResponse<UserBookReport[]> = { success: true, data: mockBooks, message: '' };
    reportServiceSpy.issueBooksReport.and.returnValue(of(mockResponse));

    // Act
    component.loadBookDetail();

    // Assert
    expect(reportServiceSpy.issueBooksReport).toHaveBeenCalledWith(undefined,null,'issue',component.currentPage, component.pageSize);
    expect(component.bookdetails).toBe(mockResponse.data);
    expect(component.loading).toBeFalse();
  });

  it('should handle unsuccessful response when loading issue book with pagination', () => {
    // Arrange
    const mockResponse: ApiResponse<UserBookReport[]> = { success: false, data: [], message: '' };
    reportServiceSpy.issueBooksReport.and.returnValue(of(mockResponse));
    spyOn(console, 'error');

    // Act
    component.loadBookDetail();

    // Assert
    expect(reportServiceSpy.issueBooksReport).toHaveBeenCalledWith(undefined,null,'issue',component.currentPage, component.pageSize);
    expect(console.error).toHaveBeenCalledWith('Falied to fetch contact', '');
    expect(component.loading).toBeFalse();
  });

  it('should handle error when loading issue book with pagination', () => {
    // Arrange
    const mockError = new Error('Network error');
    reportServiceSpy.issueBooksReport.and.returnValue(throwError(mockError));
    spyOn(console, 'error');

    // Act
    component.loadBookDetail();

    // Assert
    expect(reportServiceSpy.issueBooksReport).toHaveBeenCalledWith(undefined,null,'issue',component.currentPage, component.pageSize);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books', mockError);
    expect(component.loading).toBeFalse();
  });




  //book return
  it('should call confirmReturn and set issuedId for updation', () => {
    // Arrange
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(component, 'bookReturn');

    // Act
    component.confirmReturn(1);

    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to return book?');
    expect(component.issuedId).toBe(1);
    expect(component.bookReturn).toHaveBeenCalled();
  });

  it('should not call bookReturn if confirm is cancelled', () => {
    // Arrange
    spyOn(window, 'confirm').and.returnValue(false);
    spyOn(component, 'bookReturn');

    // Act
    component.confirmReturn(1);

    // Assert
    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to return book?');
    expect(component.bookReturn).not.toHaveBeenCalled();
  });

  it('should return book and reload book details', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: true, data: "", message: 'Contact deleted successfully' };
    bookServiceSpy.returnBook.and.returnValue(of(mockDeleteResponse));
    spyOn(component, 'loadBookDetail');

    // Act
    component.issuedId = 1;
    component.bookReturn();

    // Assert
    expect(bookServiceSpy.returnBook).toHaveBeenCalledWith(1);
    expect(component.loadBookDetail).toHaveBeenCalled();
  });

  it('should alert error message if book return fails', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: false, data: "", message: 'Failed to delete contact' };
    bookServiceSpy.returnBook.and.returnValue(of(mockDeleteResponse));
    spyOn(window, 'alert');

    // Act
    component.issuedId = 1;
    component.bookReturn();

    // Assert
    expect(window.alert).toHaveBeenCalledWith('Failed to delete contact');
  });
  it('should alert error message if return book throws error', () => {
    // Arrange
    const mockError = { error: { message: 'Delete error' } };
    bookServiceSpy.returnBook.and.returnValue(throwError(() => mockError));
    spyOn(window, 'alert');

    // Act
    component.issuedId = 1;
    component.bookReturn();

    // Assert
    expect(window.alert).toHaveBeenCalledWith('Delete error');
  });
  it('should calculate total pages correctly', () => {
    // Arrange
    component.totalBooks = 10;
    component.pageSize = 4;

    // Act
    component.calculateTotalPages();

    // Assert
    expect(component.totalPages.length).toBe(3); 
  });


 //onPageChange
  it('should change current page and load book details on page change', () => {
    // Arrange
    const page = 2;
    spyOn(component, 'loadBookDetail');

    // Act
    component.onPageChange(page);

    // Assert
    expect(component.currentPage).toBe(page);
    expect(component.loadBookDetail).toHaveBeenCalled();
  });


  //onPageSizeChange
  it('should change current page, load book count, and load book details on page size change', () => {
    // Arrange
    spyOn(component, 'loadBookCount');
    spyOn(component, 'loadBookDetail');

    // Act
    component.onPageSizeChange();

    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadBookCount).toHaveBeenCalled();
    expect(component.loadBookDetail).toHaveBeenCalled();
  });



  //calculateSrNo
  it('should calculate serial number correctly', () => {
    // Arrange
    component.currentPage = 2;
    component.pageSize = 5;
    const index = 3; 

    // Act
    const serialNumber = component.calculateSrNo(index);

    // Assert
    expect(serialNumber).toBe((2 - 1) * 5 + 3 + 1); 
  });


  //isActive
  it('should return true if current page is active', () => {
    // Arrange
    component.currentPage = 3; 

    // Act
    const isActive = component.isActive(3);

    // Assert
    expect(isActive).toBe(true); 
  });



  //onPrevPage
  it('should decrease current page by 1 and load book details on previous page action', () => {
    // Arrange
    component.currentPage = 2; 

    spyOn(component, 'loadBookDetail'); 

    // Act
    component.onPrevPage();

    // Assert
    expect(component.currentPage).toBe(1); 
    expect(component.loadBookDetail).toHaveBeenCalled(); 
  });




  //onNextPage
  it('should increase current page by 1 and load book details on next page action', () => {
    // Arrange
    component.currentPage = 1; 
    component.totalPages = [1, 2, 3]; 

    spyOn(component, 'loadBookDetail'); 

    // Act
    component.onNextPage();

    // Assert
    expect(component.currentPage).toBe(2); 
    expect(component.loadBookDetail).toHaveBeenCalled(); 
  });


  it('should not increase current page if already on the last page on next page action', () => {
    // Arrange
    component.currentPage = 3; 
    component.totalPages = [1, 2, 3];

    spyOn(component, 'loadBookDetail'); 

    // Act
    component.onNextPage();

    // Assert
    expect(component.currentPage).toBe(3); 
    expect(component.loadBookDetail).not.toHaveBeenCalled();
  });



  //BookChange
  it('should load book details on date selection', () => {
    // Arrange
    spyOn(component, 'loadBookDetail'); 

    // Act
    component.selectDate();

    // Assert
    expect(component.loadBookDetail).toHaveBeenCalled(); 
  });

  it('should change type, reset current page if type is "issue", and load book count and details on book change', () => {
    // Arrange
    const event = {
      target: {
        value: 'issue'
      }
    } as unknown as Event;

    spyOn(component, 'loadBookCount'); 
    spyOn(component, 'loadBookDetail');

    // Act
    component.onBookChange(event);

    // Assert
    expect(component.type).toBe('issue'); 
    expect(component.currentPage).toBe(1);
    expect(component.loadBookCount).toHaveBeenCalled(); 
    expect(component.loadBookDetail).toHaveBeenCalled(); 
  });


 


});
