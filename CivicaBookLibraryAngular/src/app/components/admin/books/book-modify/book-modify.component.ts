import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from 'src/app/models/Book';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-book-modify',
  templateUrl: './book-modify.component.html',
  styleUrls: ['./book-modify.component.css']
})
export class BookModifyComponent implements OnInit {
  loading: boolean = false;
  book: Book[] = [];
  bookForm!: FormGroup;
  books:any;
  bookId!: number;
  constructor(
    private bookService:BookService,
    public router:Router,
    private fb:FormBuilder,
    private route:ActivatedRoute
  ) { }

  ngOnInit():void {
    this.route.params.subscribe((params) =>{
      this.bookId = params['BookId'];
      this.getBook(this.bookId);
    });
    this.bookForm = this.fb.group({
      bookId:[0],
      title: ['', [Validators.required,Validators.minLength(2),Validators.maxLength(50)]],
      author: ['', [Validators.required,Validators.minLength(2), Validators.maxLength(50)]],
      totalQuantity: [0, [Validators.required, Validators.min(1)]],
      availableQuantity: [0, [Validators.required, Validators.min(0)]],
      issuedQuantity: [0, [Validators.required, Validators.min(0)]],
      pricePerBook: [0.01, [Validators.required, Validators.min(0.01)]]
    })
  }

  get formControl() {
    return this.bookForm.controls;
  }
  getBook(bookId:number): void {
    // this.bookId = Number(this.route.snapshot.paramMap.get('BookId'));
    this.bookService.getBookById(bookId).subscribe({
      next: (response) => {
        if (response.success) {
       
          this.bookForm.patchValue({
            bookId: response.data.bookId,
            title: response.data.title,
            author: response.data.author,
            totalQuantity: response.data.totalQuantity,
            availableQuantity: response.data.availableQuantity,
            issuedQuantity: response.data.issuedQuantity,
            pricePerBook: response.data.pricePerBook,
          })
        } else {
          console.error('Failed to fetch books', response.message);
        }
      },
      error: (error) => {
        alert(error.error.message);
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
  OnSubmit() {
    this.loading = true;

    if (this.bookForm.valid) {
      console.log(this.bookForm.value);
      this.bookService.modifyBook(this.bookForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/bookList']);
          }
          else {
            alert(response.message)
          }
        },
        error: (err) => {
          alert(err.error.message);
          console.log(err);
          this.loading = false;

        },
        complete: () => {
          console.log("Completed");
          this.loading = false;

        }
      })
    }
  }
}
