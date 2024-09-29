import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { RegisterUser } from 'src/app/models/registeruser.model';
import { SecurityQuestion } from 'src/app/models/securityQuestion.model';
import { AuthService } from 'src/app/services/auth.service';
import { SecurityquestionService } from 'src/app/services/securityquestion.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  user: RegisterUser = {
    loginId: '',
    salutation: '',
    name: '',
    dateOfBirth: '',
    gender: '',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
    passwordHint: 0,
    passwordHintAnswer: ''
  }

  loading: boolean = false;
  questions: SecurityQuestion[] = [];
  age: number | null = null;


  constructor(private authService: AuthService, private router: Router, private questionService: SecurityquestionService) { }

  ngOnInit(): void {
    this.loadQuestions();
  }

  checkPasswords(form: NgForm): void {
    const password = form.controls['password'];
    const confirmPassword = form.controls['confirmPassword'];

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword.setErrors(null);
    }
  }

  calculateAge(): void {
    if (this.user.dateOfBirth) {
      const today = new Date();
      const birthDate = new Date(this.user.dateOfBirth);
      let age = today.getFullYear() - birthDate.getFullYear();
      const monthDifference = today.getMonth() - birthDate.getMonth();
      if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < birthDate.getDate())) {
        age--;
      }
      this.age = age;
    }
  }

  loadQuestions(): void {
    this.loading = true;
    this.questionService.getAllQuestions().subscribe({
      next: (response: ApiResponse<SecurityQuestion[]>) => {
        if (response.success) {
          this.questions = response.data;
        }
        else {
          console.error('Failed to fetch questions ', response.message);
        }
        this.loading = false;
      }, error: (error) => {
        console.error('Error fetching questions : ', error);
        this.loading = false;
      }
    });
  };

  //   //   }
  onSubmit(signUpForm: NgForm): void {
    if (signUpForm.valid) {
      this.loading = true;

      console.log(signUpForm.value);

      this.authService.signUp(this.user).subscribe({
        next: (response) => {
          if (response.success) {
            console.log('User added successfully:', response);
            this.router.navigate(['/signupsuccess']);
          } else {
            alert(response.message);
          }

          this.loading = false;
        },
        error: (err) => {
          console.error(err.error.message);
          this.loading = false;
          alert(err.error.message);
        },
        complete: () => {
          this.loading = false;
          console.log("completed");
        }
      });
    }
  }
}

