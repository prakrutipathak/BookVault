import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { ChangePassword } from '../models/change-password.model';
import { ApiResponse } from '../models/ApiResponse{T}';
import { ResetPassword } from '../models/reset-password.model';
import { User } from '../models/user.model';
import { RegisterUser } from '../models/registeruser.model';


describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  
  const mockUsers: User[] = [
    {
      userId: 2,
      loginId: 'test1',
      salutation: 'Mr',
      name: 'test',
      dateOfBirth: '2002-02-02',
      age: 22,
      gender: 'M',
      email: 'test@user,com',
      phoneNumber: '76767676788',
      isAdmin:true
    },
    {
      userId: 3,
      loginId: 'test2',
      salutation: 'Ms',
      name: 'test2',
      dateOfBirth: '2002-02-02',
      age: 22,
      gender: 'F',
      email: '2@user,com',
      phoneNumber:'7373728292',
      isAdmin: false
    }
  ]

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule, FormsModule],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });
  
  afterEach(()=>{
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // changePassword

  it('should change password successfully',() => {
    // Arrnage
    const changePassword : ChangePassword = {
      loginId: 'test',
      oldPassword: 'Pass@1234',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123'
    };

    const mockSuccessResponse: ApiResponse<string>={
      data: '',
      success: true,
      message: 'Password changed successfully.'
    };

    //Act
    service.changePassword(changePassword).subscribe(
      response => {
        //Assert
        expect(response).toEqual(mockSuccessResponse);
    });
    const req=httpMock.expectOne('http://localhost:5159/api/User/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });

  it('should handle failed updation',()=>{
 // Arrnage
 const changePassword : ChangePassword = {
  loginId: 'test',
  oldPassword: 'Pass@1234',
  newPassword: 'Passowrd@123',
  confirmNewPassword: 'Password@123'
  };

  const mockErrorResponse: ApiResponse<string>={
    data: '',
    success: true,
    message: 'Password changed failed.'
  };

    //Act
    service.changePassword(changePassword).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockErrorResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/User/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while updating',()=>{
    //Arrange
    const changePassword : ChangePassword = {
      loginId: 'test',
      oldPassword: 'Pass@1234',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123'
      };

    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.changePassword(changePassword).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/User/ChangePassword');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });

  // resetPassword

  it('should reset password successfully',() => {
    // Arrnage
    const resetPassword : ResetPassword = {
      loginId: 'test',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123',
      passwordHint: 1,
      passwordHintAnswer: 'TestAnswer'
    };

    const mockSuccessResponse: ApiResponse<string>={
      data: '',
      success: true,
      message: 'Password changed successfully.'
    };

    //Act
    service.resetPassword(resetPassword).subscribe(
      response => {
        //Assert
        expect(response).toEqual(mockSuccessResponse);
    });
    const req=httpMock.expectOne('http://localhost:5159/api/User/ResetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });
  
  it('should handle failed updation',()=>{
    // Arrnage
    const resetPassword : ResetPassword = {
      loginId: 'test',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123',
      passwordHint: 1,
      passwordHintAnswer: 'TestAnswer'
    };
   
     const mockErrorResponse: ApiResponse<string>={
       data: '',
       success: true,
       message: 'Password changed failed.'
     };
   
       //Act
       service.resetPassword(resetPassword).subscribe(response=>{
         
         //Assert
         expect(response).toBe(mockErrorResponse);
       });
   
       //Api call
       const req=httpMock.expectOne('http://localhost:5159/api/User/ResetPassword');
       expect(req.request.method).toBe('PUT');
       req.flush(mockErrorResponse);
  });

  it('should handle HTTP error while updating',()=>{
    //Arrange
    const resetPassword : ResetPassword = {
      loginId: 'test',
      newPassword: 'Passowrd@123',
      confirmNewPassword: 'Password@123',
      passwordHint: 1,
      passwordHintAnswer: 'TestAnswer'
    };
    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.resetPassword(resetPassword).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/User/ResetPassword');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });

  // getAllUsers

  it('should handle an empty book list', () => {
    //Arrange
    const apiUrl='http://localhost:5159/api/User/GetAllUsers';

    const emptyResponse: ApiResponse<User[]>={
      success: true,
      data: [],
      message: ''
    }

    //Act
    service.getAllUsers().subscribe((response)=>{
      //Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });

  it('should handle HTTP error gracefully', ()=>{
    //Arrange
    const apiUrl='http://localhost:5159/api/User/GetAllUsers';
    const errorMessage = 'Failed to fetch books.';

    //Act
    service.getAllUsers().subscribe(
      ()=>fail('expected an error, not users'),
      (error) => {
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal server error');
      }
    );
    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');

    req.flush(errorMessage, {status: 500, statusText:'Internal server error'});

  });

  // deleteUser

  it('should delete a user by id successfully', ()=> {
    //Arrange
    const userId = 1;
    const mockSuccessResponse: ApiResponse<User>={
      success: true,
      data: {
        userId: 3,
        loginId: '',
        salutation: '',
        name: '',
        dateOfBirth: '',
        age: 0,
        gender: '',
        email: '',
        phoneNumber: '',
        isAdmin: false
      },
      message: "User deleted successfully"
    };

    //Act
    service.deleteUser(userId).subscribe(response => {
      //Assert
      expect(response).toEqual(mockSuccessResponse);
    });

    const req=httpMock.expectOne('http://localhost:5159/api/User/DeleteUser/'+userId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockSuccessResponse);
  });

  it('should handle failed user delete', ()=> {
    //Arrange
    const useId = 3;
    const mockErrorResponse: ApiResponse<User>={
      success: false,
      data: {
        userId: 3,
        loginId: '',
        salutation: '',
        name: '',
        dateOfBirth: '',
        age: 0,
        gender: '',
        email: '',
        phoneNumber: '',
        isAdmin: false
      },
      message: "Something went wrong"
    };

    //Act
    service.deleteUser(useId).subscribe(response => {
      //Assert
      expect(response.message).toEqual('Something went wrong');
    });

    const req=httpMock.expectOne('http://localhost:5159/api/User/DeleteUser/'+useId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockErrorResponse);
  });

  it('should handle HTTP errors', () => {
    //Arrange
    const userId = 3;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.deleteUser(userId).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne('http://localhost:5159/api/User/DeleteUser/'+userId);
    expect(req.request.method).toBe('DELETE');
    req.flush({},mockHttpError);
  });
  //Signup
  it('should signup successfully',()=>{
    //Arrange
    const register: RegisterUser={
      loginId: "login",
      salutation: "Mr",
      name: "name",
      dateOfBirth: "2003-06-25",
      gender: "F",
      email: "abc@gmail.com",
      phoneNumber: "1234567890",
      password: "Password@123",
      confirmPassword: "Password@123",
      passwordHint: 1,
      passwordHintAnswer: "blue"
    
     
    }

    const mockSuccessResponse: ApiResponse<string>={
      success: true,
      data: '',
      message: "Register Successfully"
    };

    //Act
    service.signUp(register).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockSuccessResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/User/Register');
    expect(req.request.method).toBe('POST');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed to signup',()=>{
    //Arrange
    const register: RegisterUser={
      loginId: "login",
      salutation: "Mr",
      name: "name",
      dateOfBirth: "2003-06-25",
      gender: "F",
      email: "abc@gmail.com",
      phoneNumber: "1234567890",
      password: "Password@123",
      confirmPassword: "Password@123",
      passwordHint: 1,
      passwordHintAnswer: "blue"
    
     
    }

    const mockErrorResponse: ApiResponse<string>={
      success: false,
      data: '',
      message: "User already exists"
    };

    //Act
    service.signUp(register).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockErrorResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/User/Register');
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse);
  });


  it('should handle HTTP error while signup',()=>{
    //Arrange
    const register: RegisterUser={
      loginId: "login",
      salutation: "Mr",
      name: "name",
      dateOfBirth: "2003-06-25",
      gender: "F",
      email: "abc@gmail.com",
      phoneNumber: "1234567890",
      password: "Password@123",
      confirmPassword: "Password@123",
      passwordHint: 1,
      passwordHintAnswer: "blue"
    
     
    }
    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.signUp(register).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/User/Register');
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });
  //getUsersCount
  it('should retrieve users count without search parameter', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 10,
      message: 'Users count retrieved successfully'
    };

    // Act
    service.getUsersCount().subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne('http://localhost:5159/api/User/GetUsersCount');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });



  it('should retrieve Users count with search parameter', () => {
    // Arrange
    const search = 'query';
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 5,
      message: 'Users count retrieved successfully'
    };

    // Act
    service.getUsersCount(search).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/User/GetUsersCount?search=${search}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should handle HTTP errors while users count', () => {
    //Arrange
    const search = 'query';
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getUsersCount(search).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne(`http://localhost:5159/api/User/GetUsersCount?search=${search}`);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });
   //getAllUsersByPagination
   it('should retrieve users without search parameter', () => {
    // Arrange
    const page=1;
    const pageSize=2;
    const sortOrder="asc";
    const mockResponse: ApiResponse<User[]> = {
      success: true,
      data: [
        {"userId": 2,
        "loginId": "test",
        "salutation": "Miss",
        "name": "test",
        "age": 21,
        "dateOfBirth": "2003-06-10T00:00:00",
        "gender": "F",
        "email": "test@example.com",
        "phoneNumber": "0987654321",
        "isAdmin": false,
      },
      {
        "userId": 2,
        "loginId": "test",
        "salutation": "Miss",
        "name": "test",
        "age": 21,
        "dateOfBirth": "2003-06-10T00:00:00",
        "gender": "F",
        "email": "test@example.com",
        "phoneNumber": "0987654321",
        "isAdmin": false,
      }],
      message: 'Users retrieved successfully'
    };

    // Act
    service.getAllUsersByPagination(page,pageSize,sortOrder).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/User/GetAllUsersByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });



  it('should retrieve Users count with default parameter', () => {
    // Arrange
    
    const page=1;
    const pageSize=2;
    const sortOrder="default";
    const mockResponse: ApiResponse<User[]> = {
      success: true,
      data: [
        {"userId": 2,
        "loginId": "test",
        "salutation": "Miss",
        "name": "test",
        "age": 21,
        "dateOfBirth": "2003-06-10T00:00:00",
        "gender": "F",
        "email": "test@example.com",
        "phoneNumber": "0987654321",
        "isAdmin": false,
      },
      {
        "userId": 2,
        "loginId": "test",
        "salutation": "Miss",
        "name": "test",
        "age": 21,
        "dateOfBirth": "2003-06-10T00:00:00",
        "gender": "F",
        "email": "test@example.com",
        "phoneNumber": "0987654321",
        "isAdmin": false,
      }],
      message: 'Users  retrieved successfully'
    };

    // Act
    service.getAllUsersByPagination(page,pageSize,sortOrder).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/User/GetAllUsersByPagination?page=${page}&pageSize=${pageSize}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should handle HTTP errors while users pagination', () => {
    //Arrange
    const page=1;
    const pageSize=2;
    const sortOrder="asc"

    const search = 'query';
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getAllUsersByPagination(1,2,"asc",search).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne(`http://localhost:5159/api/User/GetAllUsersByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });
  //getUserById
  it('should fetch a user by id successfully', ()=>{
    //Arrange
    const userId = 1;
    const mockSuccessResponse: ApiResponse<User>={
      success: true,
      data: {
        "userId": 2,
    "loginId": "test",
    "salutation": "Miss",
    "name": "test",
    "age": 21,
    "dateOfBirth": "2003-06-10T00:00:00",
    "gender": "F",
    "email": "test@example.com",
    "phoneNumber": "0987654321",
    "isAdmin": false,
    
      
      },
      message:''
    };

    //Act
    service.getUserById(userId).subscribe(response => {
      //Assert
      expect(response.success).toBeTrue();
      expect(response.message).toBe('');
      expect(response.data).toEqual(mockSuccessResponse.data);
    });

    const req=httpMock.expectOne('http://localhost:5159/api/User/GetUserByLoginId/'+userId);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed user retrieval', () => {
    //Arrange
    const userId=1;
    const mockErrorResponse: ApiResponse<User>={
      success: false,
      data: {
        "userId": 2,
    "loginId": "test",
    "salutation": "Miss",
    "name": "test",
    "age": 21,
    "dateOfBirth": "2003-06-10T00:00:00",
    "gender": "F",
    "email": "test@example.com",
    "phoneNumber": "0987654321",
    "isAdmin": false,
    
      
      },
      message: 'No record found!'
    };

    //Act
    service.getUserById(userId).subscribe(response => {
      //Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.message).toEqual("No record found!");
      expect(response.success).toBeFalse();
    });
    
    const req=httpMock.expectOne('http://localhost:5159/api/User/GetUserByLoginId/'+userId);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);

  });


  it('should handle HTTP errors while getUserById', () => {
    //Arrange
    const userId = 3;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getUserById(userId).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne('http://localhost:5159/api/User/GetUserByLoginId/'+userId);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });
});
