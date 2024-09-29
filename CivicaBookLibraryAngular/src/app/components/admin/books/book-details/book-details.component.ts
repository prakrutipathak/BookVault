import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EditBook } from 'src/app/models/EditBook';
import { AuthService } from 'src/app/services/auth.service';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-book-details',
  templateUrl: './book-details.component.html',
  styleUrls: ['./book-details.component.css']
})
export class BookDetailsComponent implements OnInit {
  bookId:number|undefined;
  isAuthenticated: boolean = false;
  username:string |null|undefined;

  book:EditBook={
    bookId: 0,
    title: '',
    author: '',
    totalQuantity: 0,
    availableQuantity: 0,
    issuedQuantity: 0,
    pricePerBook: 0
  }
  constructor(private bookService:BookService,private route:ActivatedRoute,private router:Router,private authService:AuthService,private cdr:ChangeDetectorRef) { }

  ngOnInit():void{
    this.authService.isAuthenticated().subscribe((authState:boolean)=>{
      this.isAuthenticated=authState;
      this.cdr.detectChanges();
     });
     this.authService.getUsername().subscribe((username:string |null|undefined)=>{
      this.username=username;
      this.cdr.detectChanges();
     });
     const bookId = Number(this.route.snapshot.paramMap.get('BookId'));
     this.bookService.getBookById(bookId).subscribe({
       next: (response) => {
         if (response.success) {
           this.book = response.data;
         } else {
           console.error('Failed to fetch book.', response.message);
         }
       },
       error: (error) => {
         console.error('Failed to fetch book.', error);
       },
     });
  }
  deleteBook(bookId: number) {
    if (confirm('Are you sure you want to delete this book?')) {
      this.bookService.deleteBook(bookId).subscribe(() => {
        this.router.navigate(['/bookList']);
      });
    }
  }

}
