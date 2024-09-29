import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Book } from 'src/app/models/Book';
import { AuthService } from 'src/app/services/auth.service';
import { BookService } from 'src/app/services/book.service';

@Component({
  selector: 'app-books-list',
  templateUrl: './books-list.component.html',
  styleUrls: ['./books-list.component.css']
})
export class BooksListComponent implements OnInit {

  books: Book[] = [];
  sortedBooks: Book[] = [];
  username: string | null | undefined;
  totalBooks!: number;
  pageSize = 4;
  currentPage = 1;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  totalPages: number[] = [];
  sortOrder: string = 'asc';
  search: string = '';
  sortBy = '';

  constructor(private authService: AuthService, private bookService: BookService, private cdr: ChangeDetectorRef, private route: Router) { }

  ngOnInit(): void {
    this.searchBooks();
    this.loadAllBooks();
    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges();
    });
  }
  totalBooksCount(search?: string) {
    this.bookService.getBooksCount(search)
      .subscribe({
        next: (response: ApiResponse<number>) => {
          if (response.success) {
            this.totalBooks = response.data;
            console.log(this.totalBooks);
            this.calculateTotalPages();

          }
          else {
            console.error('Failed to fetch books', response.message);
          }
        },
        error: (error => {
          console.error('Failed to fetch books', error);
          this.loading = false;
        })
      });
  }

  loadAllBooks(): void {
    this.loading = true;
    this.bookService.getAllBooks().subscribe({
      next: (response: ApiResponse<Book[]>) => {
        if (response.success) {
          console.log(response.data);
          this.sortedBooks = response.data;
        }
        else {
          console.error('Failed to fetch books.', response.message);
          alert('Failed to fetch books.');
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching books.', error);
        this.loading = false;
      }
    });
  }

  loadPaginatedBooks(search?: string, sortBy?: string) {
    this.loading = true;
    this.bookService.getAllBooksByPagination(this.currentPage, this.pageSize, this.sortOrder, search, sortBy)
      .subscribe({
        next: (response: ApiResponse<Book[]>) => {
          if (response.success) {
            this.books = response.data;
            console.log(response.data);
          }
          else {
            console.error('Failed to fetch books', response.message);
          }
          this.loading = false;

        },
        error: (error => {
          console.error('Failed to fetch books', error);
          this.loading = false;
        })
      });
  }
 
  calculateTotalPages() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalBooks / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }

  sortColumn(columnName: string) {
    if (this.sortBy === columnName ) {
      
      if(this.sortOrder=='desc')
        {
          this.currentPage = this.currentPage;
          this.totalBooksCount(this.search);
          this.loadPaginatedBooks(this.search,columnName);
          this.sortOrder='asc';
        }
        else{
          this.currentPage = this.currentPage;
          this.totalBooksCount(this.search);
          this.loadPaginatedBooks(this.search,columnName);
          this.sortOrder='desc';
        }
    } 
    else {
      this.sortBy = columnName;
      this.sortOrder = 'default';
      this.totalBooksCount(this.search);
      this.loadPaginatedBooks(this.search,columnName);
      this.sortOrder = 'desc'
    }
  }

  searchBooks() {
    // this.currentPage = 1;
    // this.loadPaginatedBooks(this.search,this.sortBy);
    // this.totalBooksCount(this.search);

    if (this.search != ''  && this.search.length > 2) {
      this.currentPage = 1;
      this.totalBooksCount(this.search);
      this.loadPaginatedBooks(this.search);
    }
    else {
      this.currentPage = 1;
      this.totalBooksCount();
      this.loadPaginatedBooks('');
    }
}
onPageChange(page: number,search?:string) {
  this.currentPage = page;
  this.loadPaginatedBooks(search,this.sortBy);
}
onPageSizeChange(search?: string) {
  this.currentPage = 1; 
  this.loadPaginatedBooks(search, this.sortBy);
  this.totalBooksCount(search);
}
calculateSrNo(index: number): number {
  return (this.currentPage - 1) * this.pageSize + index + 1;
}
isActive(pageNumber: number): boolean {
  return this.currentPage === pageNumber;
}
deleteBook(bookId: number) {
  if (confirm('Are you sure you want to delete this book?')) {
    this.bookService.deleteBook(bookId).subscribe(() => {
      this.totalBooks --;
      const pages = Math.ceil(this.totalBooks / this.pageSize);
      if(this.currentPage>pages){
        this.currentPage=pages;
      }
      this.loadPaginatedBooks(this.search,this.sortBy); 
      this.totalBooksCount(this.search);     
    });
  }
}

  onPrevPage(search?: string) {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadPaginatedBooks(search, this.sortBy);
    }
  }

  onNextPage(search?: string) {
    if (this.currentPage < this.totalPages.length) {
      this.currentPage++;
      this.loadPaginatedBooks(search, this.sortBy);
    }
  }
}

