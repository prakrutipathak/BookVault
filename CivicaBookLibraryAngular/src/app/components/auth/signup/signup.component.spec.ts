import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { SignupComponent } from './signup.component';
import { AuthService } from 'src/app/services/auth.service';
import { SecurityquestionService } from 'src/app/services/securityquestion.service';
import { of, throwError } from 'rxjs';
import { SecurityQuestion } from 'src/app/models/securityQuestion.model';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { SignupsuccessComponent } from '../signupsuccess/signupsuccess.component';

describe('SignupComponent', () => {
  let component: SignupComponent;
  let fixture: ComponentFixture<SignupComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let questionService: SecurityquestionService;
  let router: Router;

  
  beforeEach(waitForAsync(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['signUp']);
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([{path:'signupsuccess',component : SignupsuccessComponent}]),
        FormsModule,
        ReactiveFormsModule,
        HttpClientTestingModule
      ],
      declarations: [SignupComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        SecurityquestionService
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SignupComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    questionService = TestBed.inject(SecurityquestionService);
    router = TestBed.inject(Router);

  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load security questions successfully', () => {
    const mockQuestions: SecurityQuestion[] = [
      { passwordHint: 1, question: 'question 1' },
      { passwordHint: 2, question: 'question 2' }
    ];

    const mockApiResponse: ApiResponse<SecurityQuestion[]> = {
      data: mockQuestions,
      success: true,
      message: ''
    };

    spyOn(questionService, 'getAllQuestions').and.returnValue(of(mockApiResponse));

    component.loadQuestions();

    expect(questionService.getAllQuestions).toHaveBeenCalled();
    expect(component.questions).toEqual(mockQuestions);
    expect(component.loading).toBeFalse();
  });

  it('should handle error when loading security questions fails', () => {
    const mockError = { error: { message: 'Failed to load questions' } };

    spyOn(console, 'error');
    spyOn(questionService, 'getAllQuestions').and.returnValue(throwError(mockError));

    component.loadQuestions();

    expect(questionService.getAllQuestions).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Error fetching questions : ', mockError);
    expect(component.loading).toBeFalse();
  });

});
