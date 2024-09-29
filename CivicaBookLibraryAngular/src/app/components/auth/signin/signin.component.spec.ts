import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SigninComponent } from './signin.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';

describe('SigninComponent', () => {
  let component: SigninComponent;
  let fixture: ComponentFixture<SigninComponent>;
  let authServiceSpy:jasmine.SpyObj<AuthService>;
  let router:Router;
  

  beforeEach(() => {
    const authSpy = jasmine.createSpyObj('AuthService',['signIn']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule.withRoutes([]), FormsModule],
      declarations: [SigninComponent],
      providers : [
        {
          provide : AuthService,useValue : authSpy
        },
        
      ]
    });
    fixture = TestBed.createComponent(SigninComponent);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    component = fixture.componentInstance;
    fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should login successfully and navigate to home',()=>{
    //Arrange
   
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };

    authServiceSpy.signIn.and.returnValue(of(mockResponse));
    
    spyOn(router, 'navigate').and.stub();
     // Act
     component.username = 'testUser';
     component.password = 'testPassword';
     component.login();

    //Assert
    expect(authServiceSpy.signIn).toHaveBeenCalledWith('testUser', 'testPassword');
    expect(router.navigate).toHaveBeenCalledWith(['/home']);


  });
  it('should handle unsuccessful login and alert message', () => {
    // Arrange
    
    const mockErrorResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    authServiceSpy.signIn.and.returnValue(of(mockErrorResponse));
  
    spyOn(window, 'alert'); // Mock window.alert
  
    // Act
    component.username = 'invalidUser';
    component.password = 'invalidPassword';
    component.login();
  
    // Assert
    expect(authServiceSpy.signIn).toHaveBeenCalledWith('invalidUser', 'invalidPassword');
    expect(window.alert).toHaveBeenCalledWith(mockErrorResponse.message); // Verify alert message
  });
 
   it('should handle login error and alert error message', () => {
    // Arrange
    const mockError = { error: { message: 'Error' } };
    authServiceSpy.signIn.and.returnValue(throwError(mockError));
  
    spyOn(window, 'alert').and.stub(); // Mock window.alert
  
    // Act
    component.username = 'validUser';
    component.password = 'validPassword';
    component.login();
  
    // Assert
    expect(authServiceSpy.signIn).toHaveBeenCalledWith('validUser', 'validPassword');
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message); // Verify alert message
  });
  
 });
