import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BookissueComponent } from './bookissue.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { BookService } from 'src/app/services/book.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { BookIssue } from 'src/app/models/bookIssue.model';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';

describe('BookissueComponent', () => {
  let component: BookissueComponent;
  let fixture: ComponentFixture<BookissueComponent>;
  let bookSpy : jasmine.SpyObj<BookService>;
  let authSpy : jasmine.SpyObj<AuthService>;
  let router : Router;

  beforeEach(() => {
    bookSpy = jasmine.createSpyObj('BookService',['bookIssue','getAllBooks']);
    authSpy = jasmine.createSpyObj('AuthService',['getUserId']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule, ReactiveFormsModule],
      declarations: [BookissueComponent],
      providers : [
        {
          provide : BookService,useValue : bookSpy,
        },
        { provide: AuthService, useValue: authSpy },
       
      ]
    });
    fixture = TestBed.createComponent(BookissueComponent);
    component = fixture.componentInstance;
    //fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should issue book successfully and navigate to userbookrepost',()=>{
    //Arrange
    const mockform={
      returnDate: null,
      bookId: 1,

    }
    spyOn(router, 'navigate');

    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };
    authSpy.getUserId.and.returnValue(of('testUserId')); // Mock getUserId
    bookSpy.getAllBooks.and.returnValue(of({ success: true, data: [],message:'' })); // Mock getAllBooks
    fixture.detectChanges();

    //Act
    component.bookIssueForm.setValue(mockform); // Set form value
    bookSpy.bookIssue.and.returnValue(of(mockResponse));
    component.onSubmit();

    
   // Assert
  expect(bookSpy.bookIssue).toHaveBeenCalled();
  expect(router.navigate).toHaveBeenCalled();


  })
  it('should issue book error',()=>{
    //Arrange
    const mockError = { error: { message: 'Error' } };
    bookSpy.bookIssue.and.returnValue(throwError(mockError));
  
    spyOn(window, 'alert').and.stub();
    const mockform={
      returnDate: null,
      bookId: 1,

    }

    authSpy.getUserId.and.returnValue(of('testUserId')); // Mock getUserId
    bookSpy.getAllBooks.and.returnValue(of({ success: true, data: [],message:'' })); // Mock getAllBooks
    fixture.detectChanges();

    //Act
    component.bookIssueForm.setValue(mockform); // Set form value
    component.onSubmit();

    
   // Assert
  expect(bookSpy.bookIssue).toHaveBeenCalled();
  expect(window.alert).toHaveBeenCalledWith(mockError.error.message); // Verify alert message
  


  })
  it('should handle error when issue book fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockform={
      returnDate: null,
      bookId: 1,

    }
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    
    authSpy.getUserId.and.returnValue(of('testUserId')); // Mock getUserId
    bookSpy.getAllBooks.and.returnValue(of({ success: true, data: [],message:'' })); // Mock getAllBooks
    fixture.detectChanges(); // Trigger change detection to initialize component

    // Act
    component.bookIssueForm.setValue(mockform); // Set form value
    bookSpy.bookIssue.and.returnValue(of(mockResponse));
    component.onSubmit(); // Call onSubmit method

    // Assert
    expect(bookSpy.bookIssue).toHaveBeenCalled(); // Check if bookIssue was called with correct data
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message); // Should alert error message
  });
  
 
  
});
