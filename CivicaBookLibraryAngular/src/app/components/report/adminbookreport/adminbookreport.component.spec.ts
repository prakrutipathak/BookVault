import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminbookreportComponent } from './adminbookreport.component';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ChangeDetectorRef, Component } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';
import { AuthService } from 'src/app/services/auth.service';
import { BookService } from 'src/app/services/book.service';
import { Router } from '@angular/router';
import { AdminReportUser } from 'src/app/models/admin-report-user.model';
import { Book } from 'src/app/models/Book';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { User } from 'src/app/models/user.model';
import { ReportUser } from 'src/app/models/reportuser.model';
import { AdminBookReport } from 'src/app/models/admin-report-book.model';

describe('AdminbookreportComponent', () => {
  let component: AdminbookreportComponent;
  let fixture: ComponentFixture<AdminbookreportComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let bookService: jasmine.SpyObj<BookService>;
  let reportService: jasmine.SpyObj<ReportService>;
  let router: Router;
  let cdr: jasmine.SpyObj<ChangeDetectorRef>;

  const mockBookList :Book[] = [
    {
      bookId: 1,
      title: 'Title 1',
      author: 'Author 1',
      totalQuantity: 10,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 10.2
    },
    {
      bookId: 2,
      title: 'Title 2',
      author: 'Author 2',
      totalQuantity: 20,
      availableQuantity: 18,
      issuedQuantity: 2,
      pricePerBook: 89.2
    },
    {
      bookId: 3,
      title: 'Title 3',
      author: 'Author 3',
      totalQuantity: 10,
      availableQuantity: 18,
      issuedQuantity: 1,
      pricePerBook: 9.2
    }
  ];
  const reportUser :ReportUser={
    userId: 1,
    name: 'name 1'
  }
  const mockAdminReportUserList :AdminReportUser[] = [
    {
      userId: 1,
      issueDate: '17/09/2002',
      user: reportUser
    },
    {
      userId: 2,
      issueDate: '10/09/2002',
      user: reportUser
    },
    
  ];
  const mockAdminReportBookList :AdminBookReport[] = [
    {
      userId: 1,
      issueDate: '17/09/2002',
      user: reportUser,
      title: 'title 1',
      author: 'author 1'
    },
    {
      userId: 2,
      issueDate: '10/09/2002',
      user: reportUser,
      title: 'title 2',
      author: 'author 2'
    },
    
  ];
  const mockUserList :User[] = [
    {
      userId: 1,
      loginId: 'Login Id 1',
      salutation: 'Mr.',
      name: 'Login',
      dateOfBirth: '17-08-2002',
      age: 12,
      gender: 'M',
      email: 'login@gmail.com',
      phoneNumber: '9090909090',
      isAdmin: false
    },
    {
      userId: 2,
      loginId: 'Login Id 2',
      salutation: 'Mrs.',
      name: 'Login',
      dateOfBirth: '17-08-2002',
      age: 12,
      gender: 'M',
      email: 'login@gmail.com',
      phoneNumber: '9090909090',
      isAdmin: false
    },
    
  ];
  const mockEmptyAdminReportUserList : AdminReportUser[] =[];
  const mockEmptyAdminReportBookList : AdminBookReport[] =[];
  const mockEmptyUserList : User[] =[];
  const mockEmptyBookList : Book[] =[];
  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated','getAllUsers','getUsersCount']);
    const bookServiceSpy = jasmine.createSpyObj('BookService', ['loadAllIssueBooks', 'loadBooks','getAllBooks']);
    const reportServiceSpy = jasmine.createSpyObj('ReportService', ['loadBookCountWithDateOrStudent', 'loadUsers','getBookCountWithDateOrStudent','getIssueBookWithIssueDateOrUser','getUserCount','getUserWithBook']);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule, FormsModule, HttpClientTestingModule],
      declarations: [AdminbookreportComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: BookService, useValue: bookServiceSpy },
        { provide: ReportService, useValue: reportServiceSpy },
        { provide: Router, useValue: jasmine.createSpyObj('Router', ['navigate']) },
        { provide: ChangeDetectorRef, useValue: jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']) }
      ]
    }).compileComponents();
    
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    bookService = TestBed.inject(BookService) as jasmine.SpyObj<BookService>;
    reportService = TestBed.inject(ReportService) as jasmine.SpyObj<ReportService>;
    router = TestBed.inject(Router);
    cdr = TestBed.inject(ChangeDetectorRef) as jasmine.SpyObj<ChangeDetectorRef>; 
    
    fixture = TestBed.createComponent(AdminbookreportComponent);
    component = fixture.componentInstance;
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should calculate total pages for books', () => {
    component.totalBooks = 10;
    component.pageSize = 4;

    component.calculateTotalPagesForBook();

    expect(component.totalPages.length).toBe(Math.ceil(component.totalBooks / component.pageSize));
  });


  it('should calculate total pages for users', () => {
    component.totalUsers = 8;
    component.pageSize = 3;

    component.calculateTotalPagesForUser();

    expect(component.totalPages.length).toBe(Math.ceil(component.totalUsers / component.pageSize));
  });


  //calculateSrNo
  describe('UserbookreportComponent', () => {
    
    it('should calculate serial number based on currentPage and pageSize', () => {
      // Arrange
      component.currentPage = 2;
      component.pageSize = 5;
      const index = 3; // Mock index
  
      // Act
      const result = component.calculateSrNo(index);
  
      // Assert
      expect(result).toBe((2 - 1) * 5 + 3 + 1); 
    });
  
  });

  
  //onPageSizeChange
  describe('UserbookreportComponent', () => {
    it('should call loadUserCount and loadAllUser when userdetails.length > 0', () => {
      // Arrange
      const mockUserDetails: AdminReportUser[] = [{
        userId: 0,
        issueDate: '',
        user: {
          userId: 0,
          name: ''
        }
      }];
      component.userdetails = mockUserDetails;
      spyOn(component, 'loadUserCount');
      spyOn(component, 'loadAllUser');
  
      // Act
      component.onPageSizeChange();
  
      // Assert
      expect(component.currentPage).toBe(1); 
      expect(component.loadUserCount).toHaveBeenCalled();
      expect(component.loadAllUser).toHaveBeenCalledWith(component.bookId,component.type);
    });
  
  });
  


  it('should call loadBookCountWithDateOrStudent and loadAllIssueBooks when bookdetails.length > 0', () => {
    // Arrange
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onPageChange(2);
  
    // Assert
    expect(component.currentPage).toBe(2);
    expect(component.loadBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(component.userId, component.selectedDate);
  });


  it('should not call loadBookCountWithDateOrStudent and loadAllIssueBooks when bookdetails.length is 0', () => {
    // Arrange
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onPageChange(2);
  
    // Assert
    expect(component.currentPage).toBe(2);
    expect(component.loadBookCountWithDateOrStudent).not.toHaveBeenCalled();
    expect(component.loadAllIssueBooks).not.toHaveBeenCalled();
  });


  it('should call loadBookCountWithDateOrStudent and loadAllIssueBooks when bookdetails.length > 0 onPageSizeChange', () => {
    // Arrange
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onPageSizeChange();
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(component.userId, component.selectedDate);
  });
  


  it('should not call loadBookCountWithDateOrStudent and loadAllIssueBooks when bookdetails.length is 0 onPageSizeChange', () => {
    // Arrange
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onPageSizeChange();
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadBookCountWithDateOrStudent).not.toHaveBeenCalled();
    expect(component.loadAllIssueBooks).not.toHaveBeenCalled();
  });
  

  it('should call loadUserCount and loadAllUser when userdetails.length > 0 onPageSizeChange', () => {
    // Arrange
    const page = 1;
    component.userdetails = [{
      userId: 0,
      issueDate: '',
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadUserCount');
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onPageChange(page);
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadUserCount).toHaveBeenCalled();
    expect(component.loadAllUser).toHaveBeenCalledOnceWith(component.bookId,component.type);
  });
  


  it('should not call loadUserCount and loadAllUser when userdetails.length is 0 onPageSizeChange', () => {
    // Arrange
    const page = 1;
    spyOn(component, 'loadUserCount');
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onPageChange(page);
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadUserCount).not.toHaveBeenCalled();
    expect(component.loadAllUser).not.toHaveBeenCalled();
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
  it('should decrease currentPage and call loadAllIssueBooks when currentPage > 1', () => {
    // Arrange
    component.currentPage = 2;
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }];
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onPrevPage();
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(component.userId, component.selectedDate);
  });
  
  it('should not call loadAllIssueBooks when currentPage is 1', () => {
    // Arrange
    component.currentPage = 1;
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onPrevPage();
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadAllIssueBooks).not.toHaveBeenCalled();
  });
  
  it('should call loadAllUser when currentPage > 1 and userdetails.length > 0', () => {
    // Arrange
    component.currentPage = 2;
    component.userdetails = [{
      userId: 0,
      issueDate: '',
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onPrevPage();
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadAllUser).toHaveBeenCalled();
  });
  
  it('should not call loadAllUser when currentPage is 1', () => {
    // Arrange
    component.currentPage = 1;
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onPrevPage();
  
    // Assert
    expect(component.currentPage).toBe(1);
    expect(component.loadAllUser).not.toHaveBeenCalledWith(component.bookId,component.type);
  });
  


  //onNextPage
  it('should increase currentPage and call loadAllIssueBooks when currentPage < totalPages.length', () => {
    // Arrange
    component.currentPage = 1;
    component.totalPages = [1, 2]; 
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onNextPage();
  
    // Assert
    expect(component.currentPage).toBe(2);
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(component.userId, component.selectedDate);
  });
  
  it('should not call loadAllIssueBooks when currentPage === totalPages.length', () => {
    // Arrange
    component.currentPage = 2;
    component.totalPages = [1, 2]; 
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onNextPage();
  
    // Assert
    expect(component.currentPage).toBe(2);
    expect(component.loadAllIssueBooks).not.toHaveBeenCalled();
  });
  
  it('should call loadAllUser when currentPage < totalPages.length and userdetails.length > 0', () => {
    // Arrange
    component.currentPage = 1;
    component.totalPages = [1, 2]; 
    component.userdetails = [{
      userId: 0,
      issueDate: '',
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onNextPage();
  
    // Assert
    expect(component.currentPage).toBe(2);
    expect(component.loadAllUser).toHaveBeenCalledWith(component.bookId,component.type);
  });
  

  it('should not call loadAllUser when currentPage === totalPages.length', () => {
    // Arrange
    component.currentPage = 2;
    component.totalPages = [1, 2]; 
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onNextPage();
  
    // Assert
    expect(component.currentPage).toBe(2);
    expect(component.loadAllUser).not.toHaveBeenCalled();
  });
  


  //selectedDate
  it('should reset variables and call load methods when date is selected', () => {
    // Arrange
    component.userId = 1;
    component.bookId = 1;
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }];
    component.userdetails = [{
      userId: 0,
      issueDate: '',
      user: {
        userId: 0,
        name: ''
      }
    }];
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.selectDate();
  
    // Assert
    expect(component.userId).toBeNull();
    expect(component.bookId).toBeNull();
    expect(component.bookdetails.length).toBe(0);
    expect(component.userdetails.length).toBe(0);
    expect(component.loadBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(null, component.selectedDate);
  });
  

  it('should not call load methods when date is selected and bookdetails or userdetails are empty', () => {
    // Arrange
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.selectDate();
  
    // Assert
    expect(component.loadBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(null, component.selectedDate);
  });
  


  //onUserChange
  it('should update userId and call load methods when user is selected', () => {
    // Arrange
    const mockEvent = {
      target: {
        value: '1'
      } as HTMLSelectElement
    };
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    component.userdetails = [{
      userId: 0,
      issueDate: '',
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onUserChange(mockEvent as unknown as Event);
  
    // Assert
    expect(component.userId).toBe(1);
    expect(component.selectedDate).toBeNull();
    expect(component.bookId).toBeNull();
    expect(component.bookdetails.length).toBe(0);
    expect(component.userdetails.length).toBe(0);
    expect(component.loadBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(1, null);
  });
  
  it('should not call load methods when user is selected and bookdetails or userdetails are empty', () => {
    // Arrange
    const mockEvent = {
      target: {
        value: '1'
      } as HTMLSelectElement
    };
    spyOn(component, 'loadBookCountWithDateOrStudent');
    spyOn(component, 'loadAllIssueBooks');
  
    // Act
    component.onUserChange(mockEvent as unknown as Event);
  
    // Assert
    expect(component.loadBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(component.loadAllIssueBooks).toHaveBeenCalledWith(1, null);
  });
  


  //onBookChange
  it('should update bookId and call load methods when book is selected', () => {
    // Arrange
    const mockEvent = {
      target: {
        value: '1'
      } as HTMLSelectElement
    };
    component.bookdetails = [{
      title: '',
      author: '',
      issueDate: '',
      userId: 0,
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    component.userdetails = [{
      userId: 0,
      issueDate: '',
      user: {
        userId: 0,
        name: ''
      }
    }]; 
    spyOn(component, 'loadUserCount');
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onBookChange(mockEvent as unknown as Event);
  
    // Assert
    expect(component.bookId).toBe(1);
    expect(component.selectedDate).toBeNull();
    expect(component.userId).toBeNull();
    expect(component.bookdetails.length).toBe(0);
    expect(component.userdetails.length).toBe(0);
    expect(component.loadUserCount).toHaveBeenCalled();
    expect(component.loadAllUser).toHaveBeenCalledWith(1,component.type);
  });
  

  
  it('should not call load methods when book is selected and bookdetails or userdetails are empty', () => {
    // Arrange
    const mockEvent = {
      target: {
        value: '1'
      } as HTMLSelectElement
    };
    spyOn(component, 'loadUserCount');
    spyOn(component, 'loadAllUser');
  
    // Act
    component.onBookChange(mockEvent as unknown as Event);
  
    // Assert
    expect(component.loadUserCount).toHaveBeenCalled();
    expect(component.loadAllUser).toHaveBeenCalledWith(1,component.type);
  });
  
  it('should load books successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    bookService.getAllBooks.and.returnValue(of(mockResponse));
    //Act
    component.loadBooks();

    //Assert
    expect(bookService.getAllBooks).toHaveBeenCalled();
    expect(component.books).toEqual(mockBookList);
    expect(component.loading).toBe(false);
  })
  it('should fail to load books ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: false, data:mockEmptyBookList , message: 'Failed to fetch books.',
    };
    bookService.getAllBooks.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadBooks();

    //Assert
    expect(bookService.getAllBooks).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    bookService.getAllBooks.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadBooks();

    //Assert
    expect(bookService.getAllBooks).toHaveBeenCalled();

  })
  it('should load Users successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authService.getAllUsers.and.returnValue(of(mockResponse));
    //Act
    component.loadUsers();

    //Assert
    expect(authService.getAllUsers).toHaveBeenCalled();
    expect(component.users).toEqual(mockUserList);
    expect(component.loading).toBe(false);
  })
  it('should fail to load users ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: false, data:mockEmptyUserList , message: 'Failed to fetch users.',
    };
    authService.getAllUsers.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadUsers();

    //Assert
    expect(authService.getAllUsers).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    authService.getAllUsers.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadUsers();

    //Assert
    expect(authService.getAllUsers).toHaveBeenCalled();
  })
  it('should calaulate total user count',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: true, data: 3, message: ''
    };
    reportService.getUserCount.and.returnValue(of(mockResponse));

    //Act
    component.loadUserCount();

    //Assert
    expect(reportService.getUserCount).toHaveBeenCalled();

  })
  it('should fail to calaulate total User count ',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: false, data: 0, message: 'Failed to fetch books'
    };
    reportService.getUserCount.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadUserCount();

    //Assert
    expect(reportService.getUserCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockResponse.message);

  })
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    reportService.getUserCount.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadUserCount();

    //Assert
    expect(reportService.getUserCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockError);

  })
  it('should calaulate total book count with date or student',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: true, data: 3, message: ''
    };
    reportService.getBookCountWithDateOrStudent.and.returnValue(of(mockResponse));

    //Act
    component.loadBookCountWithDateOrStudent();

    //Assert
    expect(reportService.getBookCountWithDateOrStudent).toHaveBeenCalled();

  })
  it('should fail to calaulate total books count with date or student',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: false, data: 0, message: 'Failed to fetch books'
    };
    reportService.getBookCountWithDateOrStudent.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadBookCountWithDateOrStudent();

    //Assert
    expect(reportService.getBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockResponse.message);

  })
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    reportService.getBookCountWithDateOrStudent.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadBookCountWithDateOrStudent();

    //Assert
    expect(reportService.getBookCountWithDateOrStudent).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockError);

  })
  it('should load All Users successfully',()=>{
    //Arrange
    const mockResponse :ApiResponse<AdminReportUser[]> ={
      success: true, data: mockAdminReportUserList, message: '',
    };
    reportService.getUserWithBook.and.returnValue(of(mockResponse));
    //Act
    component.loadAllUser(1,'admin');

    //Assert
    expect(reportService.getUserWithBook).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })
  
  it('should fail to load users ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<AdminReportUser[]> ={
      success: false, data: mockAdminReportUserList , message: 'Failed to fetch users.',
    };
    reportService.getUserWithBook.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadAllUser(1,"");

    //Assert
    expect(reportService.getUserWithBook).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    reportService.getUserWithBook.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadAllUser(1,"");

    //Assert
    expect(reportService.getUserWithBook).toHaveBeenCalled();
  })
  it('should load All Issue Books successfully',()=>{
    //Arrange
    const mockResponse :ApiResponse<AdminBookReport[]> ={
      success: true, data: mockAdminReportBookList, message: '',
    };
    reportService.getIssueBookWithIssueDateOrUser.and.returnValue(of(mockResponse));
    //Act
    component.loadAllIssueBooks(1,"");

    //Assert
    expect(reportService.getIssueBookWithIssueDateOrUser).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })
  
  it('should fail to load users ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<AdminBookReport[]> ={
      success: false, data: mockAdminReportBookList , message: 'Failed to fetch users.',
    };
    reportService.getIssueBookWithIssueDateOrUser.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadAllIssueBooks(1,"");

    //Assert
    expect(reportService.getIssueBookWithIssueDateOrUser).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    reportService.getIssueBookWithIssueDateOrUser.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadAllIssueBooks(1,"");

    //Assert
    expect(reportService.getIssueBookWithIssueDateOrUser).toHaveBeenCalled();
  })
//onBookChange
it('should update bookId and call load methods when status is selected', () => {
  // Arrange
  const mockEvent = {
    target: {
      value: '1'
    } as HTMLSelectElement
  };
  component.bookdetails = [{
    title: '',
    author: '',
    issueDate: '',
    userId: 0,
    user: {
      userId: 0,
      name: ''
    }
  }]; 
  component.userdetails = [{
    userId: 0,
    issueDate: '',
    user: {
      userId: 0,
      name: ''
    }
  }]; 
  spyOn(component, 'loadUserCount');
  spyOn(component, 'loadAllUser');

  // Act
  component.onStatusChange(mockEvent as unknown as Event);

  // Assert
  expect(component.bookId).toBeNull();
  expect(component.selectedDate).toBeNull();
  expect(component.userId).toBeNull();
  expect(component.bookdetails.length).toBe(1);
  expect(component.userdetails.length).toBe(0);
  expect(component.loadUserCount).toHaveBeenCalled();
  expect(component.loadAllUser).toHaveBeenCalledWith(null,component.type);
});



it('should not call load methods when book is selected and bookdetails or userdetails are empty on status change', () => {
  // Arrange
  const mockEvent = {
    target: {
      value: '1'
    } as HTMLSelectElement
  };
  spyOn(component, 'loadUserCount');
  spyOn(component, 'loadAllUser');

  // Act
  component.onStatusChange(mockEvent as unknown as Event);

  // Assert
  expect(component.loadUserCount).toHaveBeenCalled();
  expect(component.loadAllUser).toHaveBeenCalledWith(null,component.type);
});
});
