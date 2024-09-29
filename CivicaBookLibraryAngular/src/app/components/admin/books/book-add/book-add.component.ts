import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Book } from 'src/app/models/Book';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-book-add',
  templateUrl: './book-add.component.html',
  styleUrls: ['./book-add.component.css']
})
export class BookAddComponent implements OnInit {
  loading: boolean = false;
  book: Book[] = [];
  bookForm!: FormGroup;
  
  constructor(
    private bookService:BookService,
    private router:Router,
    private fb:FormBuilder
  ) { }

  ngOnInit():void {
    this.bookForm = this.fb.group({
      title: ['', [Validators.required,Validators.minLength(2),Validators.maxLength(50)]],
      author: ['', [Validators.required,Validators.minLength(2), Validators.maxLength(50)]],
      totalQuantity: [0, [Validators.required, Validators.min(1)]],
      availableQuantity: [0, [Validators.required, Validators.min(0)]],
      issuedQuantity: [0, [Validators.required, Validators.min(0)]],
      pricePerBook: [0.01, [Validators.required, Validators.min(0.01)]]
    })
  }
  get formControl(){
    return this.bookForm.controls;
   }
   OnSubmit(){
    this.loading = true;

    if(this.bookForm.valid){
      console.log(this.bookForm.value);
      this.bookService.addBook(this.bookForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/bookList']);
          }
          else{
            alert(response.message)
          }
        },
        error:(err)=>{
          alert(err.error.message);
          this.loading = false;

        },
        complete:()=>{
          console.log("Completed");
          this.loading = false;

        }
      })
    }
  }
}
