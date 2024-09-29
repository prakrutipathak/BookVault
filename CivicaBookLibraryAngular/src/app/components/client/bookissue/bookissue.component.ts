import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Book } from 'src/app/models/Book';
import { BookIssue } from 'src/app/models/bookIssue.model';
import { AuthService } from 'src/app/services/auth.service';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-bookissue',
  templateUrl: './bookissue.component.html',
  styleUrls: ['./bookissue.component.css']
})
export class BookissueComponent implements OnInit {
  loading: boolean = false;
  userId: string | null | undefined;
  books: Book[] = [];
  bookIssueForm!: FormGroup;
  constructor(private authService: AuthService, private cdr: ChangeDetectorRef, private fb: FormBuilder, private router: Router, private bookService: BookService) { }
  ngOnInit(): void {
    this.authService.getUserId().subscribe((userId:string |null|undefined)=>{
      this.userId=userId
          });
    this.bookIssueForm=this.fb.group({
      bookId : [0, [Validators.required, this.bookValidator]],
      returnDate: null,
    });
    this.loadBooks();
  }
  get formControl() {
    return this.bookIssueForm.controls;
  }
  bookValidator(controls: any) {
    return controls.value == '' ? { invalidBook: true } : null;
  }
 
   loadBooks():void{
    this.loading = true; 
    this.bookService.getAllBooks().subscribe({
      next: (response: ApiResponse<Book[]>) => {
        if (response.success) {
          this.books = response.data;
        } else {
          console.error('Failed to fetch books ', response.message);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching books: ', error);
        this.loading = false;
      }
    });
  }
  onSubmit() {
    this.loading = true;
    if (this.bookIssueForm.valid) {
      const bookIssueData: BookIssue = {
        issueDate: this.bookIssueForm.value.issueDate,
        returnDate: this.bookIssueForm.value.returnDate,
        userId: Number(this.userId),
        bookId: this.bookIssueForm.value.bookId
      };

      console.log(this.bookIssueForm.value);
      console.log(bookIssueData);
      this.bookService.bookIssue(bookIssueData)
        .subscribe({
          next: (response) => {
            if (response.success) {
              console.log("Book Issued", response);
              this.router.navigate(['/user-book-report']);
            }
            else {
              alert(response.message);
            }
            this.loading = false;
          },
          error: (err) => {
            this.loading = false;
            alert(err.error.message);
          },
          complete: () => {
            this.loading = false;
            console.log('Completed');
          }
        });
    }
  }

}
