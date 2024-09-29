/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ChangeDetectorRef, DebugElement } from '@angular/core';
import { UserListComponent } from './user-list.component';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { User } from 'src/app/models/user.model';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let router: Router;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;

  const mockUser:User = {
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
  }
  const mockEmptyUserList : User[] =[];
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

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated','getUsername','isAuthenticated','getAllUsersByPagination','getUsersCount','getAllUsers','deleteUser']);

    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,RouterTestingModule],
      declarations: [UserListComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ChangeDetectorRef, useValue: cdrSpy }
      ],
    });
 
    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;

    // fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should calaulate total user count without search',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: true, data: 3, message: ''
    };
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));

    //Act
    component.totalUserCount();

    //Assert
    expect(authServiceSpy.getUsersCount).toHaveBeenCalled();

  })
  it('should fail to calaulate total User count ',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: false, data: 0, message: 'Failed to fetch users'
    };
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.totalUserCount();

    //Assert
    expect(authServiceSpy.getUsersCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users',mockResponse.message);

  })
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    authServiceSpy.getUsersCount.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.totalUserCount();

    //Assert
    expect(authServiceSpy.getUsersCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users',mockError);

  })
  it('should load Users successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authServiceSpy.getAllUsers.and.returnValue(of(mockResponse));
    //Act
    component.loadAllUsers();

    //Assert
    expect(authServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.sortedUsers).toEqual(mockUserList);
    expect(component.loading).toBe(false);
  })
  it('should fail to load users ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: false, data:mockEmptyUserList , message: 'Failed to fetch users.',
    };
    authServiceSpy.getAllUsers.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadAllUsers();

    //Assert
    expect(authServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    authServiceSpy.getAllUsers.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadAllUsers();

    //Assert
    expect(authServiceSpy.getAllUsers).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Error fetching users.',mockError);

  })
  it('should load paginated users successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedUsers();

    //Assert
    expect(authServiceSpy.getAllUsersByPagination).toHaveBeenCalled();
    expect(component.users).toEqual(mockUserList);
    expect(component.loading).toBe(false);
  })
  it('should fail to load paginated users ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: false, data:mockUserList , message: 'Failed to fetch users',
    };
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadPaginatedUsers();

    //Assert
    expect(authServiceSpy.getAllUsersByPagination).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    authServiceSpy.getAllUsersByPagination.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadPaginatedUsers();

    //Assert
    expect(authServiceSpy.getAllUsersByPagination).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch users',mockError);

  })
  it('should load paginated users successfully with search',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedUsers("Title 1");

    //Assert
    expect(authServiceSpy.getAllUsersByPagination).toHaveBeenCalled();
    expect(component.users).toEqual(mockUserList);
    expect(component.loading).toBe(false);
  })
  it('should load paginated users successfully with sortBy and Search',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedUsers("Title 1");

    //Assert
    expect(authServiceSpy.getAllUsersByPagination).toHaveBeenCalled();
    expect(component.users).toEqual(mockUserList);
    expect(component.loading).toBe(false);
  })
  it('should search User', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    const initialColumn = 'title';
    
    component.currentPage=1;
   

    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.searchUsers();

    expect(component.sortOrder).toBe('default');
    expect(authServiceSpy.getUsersCount).toHaveBeenCalledWith(undefined);

  });
  it('should search book if string empty', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    const initialColumn = 'title';
    component.currentPage=1;
    component.search ='ert'
   if(component.search != ''  && component.search.length > 2){
    component.currentPage=1;
   }
    authServiceSpy.getUsersCount('');
    authServiceSpy.getAllUsersByPagination(1,2,'');
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.searchUsers();

    expect(component.sortOrder).toBe('default');
    expect(authServiceSpy.getUsersCount).toHaveBeenCalledWith(component.search);

  });
  it('should load paginated page on pageChange()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    const initialColumn = 'title';
    let page = 2;
    
    component.currentPage=page;
   
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.onPageChange(page,initialColumn);

    expect(component.sortOrder).toBe('default');

  });
  it('should load paginated page on pagesizeChange()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    const initialColumn = 'title';
    
    component.currentPage=1;
   let search = 'title'

    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.onPageSizeChange(search);

    expect(component.sortOrder).toBe('default');
    expect(authServiceSpy.getUsersCount).toHaveBeenCalledWith(search);
  });
  it('should calculate serial number correctly', () => {
    const index = 0; 
    const expectedSerialNumber = (component.currentPage - 1) * component.pageSize + index + 1;
    const actualSerialNumber = component.calculateSrNo(index);
    component.calculateSrNo(index);
    expect(actualSerialNumber).toBe(expectedSerialNumber);
  });
  it('should show active button differently', () => {
    const pageNumber = 1; 
    component.currentPage === pageNumber;
    const expectedPageNumber =component.currentPage === pageNumber;;
    const actualPageNumber = component.isActive(pageNumber);
    component.isActive(pageNumber);
    expect(actualPageNumber).toBe(expectedPageNumber);
  });
  it('should load previous paginated page on onPrevPage()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    const initialColumn = 'title';
    
    component.currentPage=2;
    let search = 'title'

    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.onPrevPage(search);

    expect(component.sortOrder).toBe('default');
  });
  it('should load next paginated page on onNextPage()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    const initialColumn = 'title';
    
    component.currentPage=3;
    let search = 'title'
    component.totalPages.length = 6;
    
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.onNextPage(search);

    expect(component.sortOrder).toBe('default');
  });

  it('should not delete User when user cancels deletion', () => {
    const UserId = 2;
    spyOn(window, 'confirm').and.returnValue(false); 

    component.deleteUser(UserId);

    expect(window.confirm).toHaveBeenCalled();
    expect(authServiceSpy.deleteUser).not.toHaveBeenCalled(); 

  });
  it('should delete Users successfully',()=>{
    //Arrange
   let UserId = 1;
   const mockResponse :ApiResponse<number> ={
    success: true, data: 10, message: '',
  };
  const mockResponse1 :ApiResponse<User[]> ={
    success: true, data: mockUserList, message: '',
  };
  const mockResponse2 :ApiResponse<User> ={
    success: true, data: mockUser, message: '',
  };
  const initialColumn = 'title';
  let page = 3
  component.currentPage=4;
  if(component.currentPage>page)
    {
      component.currentPage=page
    }

 
  spyOn(window, 'confirm').and.returnValue(true); 
 
    authServiceSpy.deleteUser.and.returnValue(of(mockResponse2));
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));
  
    //Act
    component.deleteUser(UserId);

    //Assert
    expect(authServiceSpy.deleteUser).toHaveBeenCalled();
  })
  it('should set asc correctly when sorting by a column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.sortAsc();
  
    expect(component.sortOrder).toBe('desc');
  });
  it('should set desc correctly when sorting by a column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<User[]> ={
      success: true, data: mockUserList, message: '',
    };
    authServiceSpy.getUsersCount.and.returnValue(of(mockResponse));
    authServiceSpy.getAllUsersByPagination.and.returnValue(of(mockResponse1));

    component.sortDesc();
  
    expect(component.sortOrder).toBe('asc');
  });
});
