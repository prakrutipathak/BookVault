import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { ResetPassword } from 'src/app/models/reset-password.model';
import { SecurityQuestion } from 'src/app/models/securityQuestion.model';
import { AuthService } from 'src/app/services/auth.service';
import { SecurityquestionService } from 'src/app/services/securityquestion.service';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.css']
})
export class ForgetPasswordComponent implements OnInit{
  username: string | null | undefined;
  questions: SecurityQuestion[] | undefined;
  loading : boolean = false;
  resetPassword: ResetPassword = {
    loginId: '',
    passwordHint: 0,
    passwordHintAnswer: '',
    newPassword: '',
    confirmNewPassword: ''
  }

  constructor(
    private authService: AuthService,
    private router : Router,
    private questionService: SecurityquestionService
  ){}

  ngOnInit(): void {
    this.loadQuestions();
  }

  loadQuestions(): void{
    this.loading = true;
    this.questionService.getAllQuestions().subscribe({
      next:(response: ApiResponse<SecurityQuestion[]>) =>{
        if(response.success){
          this.questions = response.data;
        }
        else{
          console.error('Failed to fetch questions ', response.message);
        }
        this.loading = false;
      },error:(error)=>{
        console.error('Error fetching questions : ',error);
        this.loading = false;
      }
    });
  };

  checkPasswords(form: NgForm):void {
    const password = form.controls['newPassword'];
    const confirmPassword = form.controls['confirmNewPassword'];
 
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword.setErrors(null);
    }
  };

  onSubmit(resetPasswordForm: NgForm): void {
    if(resetPasswordForm.valid){
      this.authService.resetPassword(this.resetPassword).subscribe({
        next:(response) => {
          if(response.success){
            alert('Password reset successfull.')
            this.router.navigate(['/signin'])
          }
          else {
            alert(response.message);
          }
        },
        error:(err) => {
          alert(err.error.message);
        }
      });
    }
  }

}
