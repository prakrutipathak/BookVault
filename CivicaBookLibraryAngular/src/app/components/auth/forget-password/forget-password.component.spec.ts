import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgetPasswordComponent } from './forget-password.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormControl, FormsModule, NgForm } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { SecurityQuestion } from 'src/app/models/securityQuestion.model';
import { SecurityquestionService } from 'src/app/services/securityquestion.service';
import { useAnimation } from '@angular/animations';

describe('ForgetPasswordComponent', () => {
  let component: ForgetPasswordComponent;
  let fixture: ComponentFixture<ForgetPasswordComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let questionServiceSpy: jasmine.SpyObj<SecurityquestionService>;
  let routerSpy: Router;

  const mockQuestions : SecurityQuestion[] = [
    {
      passwordHint: 1,
      question: 'Q1'
    },
    {
      passwordHint: 2,
      question: 'Q2'
    }
  ]

   beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['resetPassword']);
    questionServiceSpy = jasmine.createSpyObj('SecurityquestionService', ['getAllQuestions']);

    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, RouterTestingModule.withRoutes([])],
      declarations: [ForgetPasswordComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: SecurityquestionService, useValue: questionServiceSpy}
      ]
    });
    fixture = TestBed.createComponent(ForgetPasswordComponent);
    component = fixture.componentInstance;
    routerSpy = TestBed.inject(Router);
    questionServiceSpy = TestBed.inject(SecurityquestionService) as jasmine.SpyObj<SecurityquestionService>;
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load questions on init', () => {
    let mockQuestions: SecurityQuestion[] = [
      {
        passwordHint: 1,
        question: 'Question'
      }
    ];

    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    questionServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    fixture.detectChanges();

    expect(questionServiceSpy.getAllQuestions);
    expect(component.questions).toEqual(mockQuestions);
  });

  it('should set questions when questions are loaded successfully', () => {
    let mockQuestions: SecurityQuestion[] = [
      {
        passwordHint: 1,
        question: 'Question'
      }
    ];

    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    questionServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    component.loadQuestions()

    expect(questionServiceSpy.getAllQuestions).toHaveBeenCalled();
    expect(component.questions).toEqual(mockQuestions);
  });

  it('should set console error when loading questions fails', () => {
    let mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: [],
      success: false,
      message: 'Error fetching questions'
    };
    spyOn(console, 'error');

    questionServiceSpy.getAllQuestions.and.returnValue(of(mockApiResponse))
    component.loadQuestions()

    expect(questionServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch questions ', mockApiResponse.message)
  })

  it('should set console error when load questions returns error', () => {
    const mockError = { error: { message: 'HTTP error' } };
    authServiceSpy.resetPassword.and.returnValue(throwError(mockError));
    spyOn(console, 'error');

    questionServiceSpy.getAllQuestions.and.returnValue(throwError(mockError))
    component.loadQuestions()

    expect( questionServiceSpy.getAllQuestions).toHaveBeenCalledWith();
    expect(console.error).toHaveBeenCalledWith('Error fetching questions : ', mockError)
  })

  it('should invalidate password mismatch', () => {
    const form = <NgForm><unknown>{
      valid: true,
      value: {
          loginId: 'TestId',
          oldPassword: 'TestPassword@123',
          newPassword: 'TestPassword@1234',
          newConfirmPassword: 'TestPassword@12345',
      },
        controls: {
          loginId: { value: 'TestId' },
          oldPassword: { value: 'TestPassword@123' },
          newPassword: { value: 'TestPassword@1234' },
          newConfirmPassword: { value: 'TestPassword@12345' },
      },
    };
    form.controls['newPassword'] = new FormControl('password1');
    form.controls['confirmNewPassword'] = new FormControl('password2');

    component.checkPasswords(form);

    expect(form.controls['confirmNewPassword'].hasError('passwordMismatch')).toBeTrue();
  });

  it('should validate password', () => {
    const form = <NgForm><unknown>{
      valid: true,
      value: {
          loginId: 'TestId',
          oldPassword: 'TestPassword@123',
          newPassword: 'TestPassword@1234',
          newConfirmPassword: 'TestPassword@12345',
      },
        controls: {
          loginId: { value: 'TestId' },
          oldPassword: { value: 'TestPassword@123' },
          newPassword: { value: 'TestPassword@1234' },
          newConfirmPassword: { value: 'TestPassword@12345' },
      },
    };
    form.controls['newPassword'] = new FormControl('password1');
    form.controls['confirmNewPassword'] = new FormControl('password1');

    component.checkPasswords(form);

    expect(form.controls['confirmNewPassword'].errors).toBeNull();
  });

  it('should navigate to /signin on successful changing password', () => {
    spyOn(routerSpy, 'navigate');
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };
    authServiceSpy.resetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'Test',
        passwordHint: '1',
        passwordHintAnswer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        passwordHint: { value: '1' },
        passwordHintAnswer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
 
    component.onSubmit(form);
 
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/signin']);
  });

  it('should alert error message on unsuccessful password change', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error changing password.' };
    authServiceSpy.resetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'Test',
        passwordHint: '1',
        passwordHintAnswer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        passwordHint: { value: '1' },
        passwordHintAnswer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Error changing password.');
  });

  it('should alert error message on verification fails', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Verification failed!' };
    authServiceSpy.resetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'Test',
        passwordHint: '1',
        passwordHintAnswer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        passwordHint: { value: '1' },
        passwordHintAnswer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Verification failed!');
  });

  it('should alert error message on invaild username', () => {
    spyOn(window, 'alert');
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Invaild loginId.' };
    authServiceSpy.resetPassword.and.returnValue(of(mockResponse));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'Test',
        passwordHint: '1',
        passwordHintAnswer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        passwordHint: { value: '1' },
        passwordHintAnswer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('Invaild loginId.');
  });

  it('should alert error message on HTTP error', () => {
    spyOn(window, 'alert');
    const mockError = { error: { message: 'HTTP error' } };
    authServiceSpy.resetPassword.and.returnValue(throwError(mockError));
 
    const form = <NgForm><unknown>{
      valid: true,
      value: {
        loginId: 'Test',
        passwordHint: '1',
        passwordHintAnswer: 'TestAnswer',
        newPassword: 'Password@123',
        confirmNewPassword: 'Password@123'
      },
      controls: {
        loginId: { value: 'Test' },
        passwordHint: { value: '1' },
        passwordHintAnswer: { value: 'TestAnswer' },
        newPassword: { value: 'Password@123' },
        confirmNewPassword: { value: 'Password@123' }
      }
    };
 
    component.onSubmit(form);
 
    expect(window.alert).toHaveBeenCalledWith('HTTP error');
  });

  it('should load questions on init', () => {
    // Arrange
    const mockResponse: ApiResponse<SecurityQuestion[]> = { success: true, data: mockQuestions, message: '' };
    questionServiceSpy.getAllQuestions.and.returnValue(of(mockResponse));
 
    // Act
    fixture.detectChanges(); // ngOnInit is called here
 
    // Assert
    expect(questionServiceSpy.getAllQuestions);
  });

});
