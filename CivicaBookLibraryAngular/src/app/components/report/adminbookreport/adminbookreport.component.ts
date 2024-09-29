import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { Book } from 'src/app/models/Book';
import { AdminBookReport } from 'src/app/models/admin-report-book.model';
import { AdminReportUser } from 'src/app/models/admin-report-user.model';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';
import { BookService } from 'src/app/services/book.service';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-adminbookreport',
  templateUrl: './adminbookreport.component.html',
  styleUrls: ['./adminbookreport.component.css']
})
export class AdminbookreportComponent {

  bookdetails: AdminBookReport[] = [];
  userdetails: AdminReportUser[] = [];
  books: Book[] = [];
  users: User[] = [];
  totalBooks!: number;
  totalUsers!: number;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  isBookSelected : boolean = false;

  userId: number | null = null;
  bookId: number | null = null;
  selectedDate: string | null = null;

  currentPage = 1;
  pageSize = 4;
  type: string = 'issue';
  totalPages: number[] = [];



  constructor(private authService: AuthService, private bookService: BookService, private reportService: ReportService, private router: Router, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges();   //it code trigger change detection automatically
    });

    this.loadBookCountWithDateOrStudent();
    // this.loadUserCount();
    this.loadAllIssueBooks(this.userId, this.selectedDate);
    this.loadUsers();
    this.loadBooks();

  }

  //
  calculateTotalPagesForBook() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalBooks / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }

  calculateTotalPagesForUser() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalUsers / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }
  onPageChange(page: number) {
    this.currentPage = page;
    if (this.bookdetails.length > 0) {
      this.loadBookCountWithDateOrStudent();
      this.loadAllIssueBooks(this.userId, this.selectedDate);
    }
    if (this.userdetails.length > 0) {
      this.loadUserCount();
      this.loadAllUser(this.bookId, this.type);
    }
  }
  onPageSizeChange() {
    this.currentPage = 1;
    if (this.bookdetails.length > 0) {
      this.loadBookCountWithDateOrStudent();
      this.loadAllIssueBooks(this.userId, this.selectedDate);
    }
    if (this.userdetails.length > 0) {
      this.loadUserCount();
      this.loadAllUser(this.bookId, this.type);
    }
  }
  calculateSrNo(index: number): number {
    return (this.currentPage - 1) * this.pageSize + index + 1;
  }

  isActive(pageNumber: number): boolean {
    return this.currentPage === pageNumber;
  }

  onPrevPage(search?: string) {
    if (this.currentPage > 1) {
      this.currentPage--;
      if (this.bookdetails.length > 0) {
        // this.loadBookCountWithDateOrStudent();
        this.loadAllIssueBooks(this.userId, this.selectedDate);
      }
      if (this.userdetails.length > 0) {
        // this.loadUserCount();
        this.loadAllUser(this.bookId, this.type);
      }
    }
  }

  onNextPage(search?: string) {
    if (this.currentPage < this.totalPages.length) {
      this.currentPage++;
      if (this.bookdetails.length > 0) {
        // this.loadBookCountWithDateOrStudent();
        this.loadAllIssueBooks(this.userId, this.selectedDate);
      }
      if (this.userdetails.length > 0) {
        // this.loadUserCount();
        this.loadAllUser(this.bookId, this.type);
      }
    }
  }
  //




  selectDate(): void {
    this.cdr.detectChanges();
    this.isBookSelected = false;
    this.bookdetails = [];
    this.userdetails = [];
    this.userId = null;
    this.bookId = null;
    this.loadBookCountWithDateOrStudent();
    this.loadAllIssueBooks(this.userId, this.selectedDate);

  }

  onUserChange(event: Event): void {
    this.cdr.detectChanges();
    this.isBookSelected = false;
    this.bookdetails = [];
    this.userdetails = [];
    const selectElement = event.target as HTMLSelectElement;
    this.userId = parseInt(selectElement.value, 10);
    this.selectedDate = null;
    this.bookId = null;
    this.loadBookCountWithDateOrStudent();
    this.loadAllIssueBooks(this.userId, this.selectedDate);
  }
  onBookChange(event: Event): void {
    this.cdr.detectChanges();
    this.isBookSelected = true;
    this.type = 'issue';
    this.bookdetails = [];
    this.userdetails = [];
    const selectElement = event.target as HTMLSelectElement;
    this.bookId = parseInt(selectElement.value, 10);
    this.selectedDate = null;
    this.userId = null;
    this.loadUserCount();
    this.loadAllUser(this.bookId, this.type);
  }
  onStatusChange(event: Event): void {
    this.cdr.detectChanges();
    this.isBookSelected = true;
    this.userdetails = [];
    this.bookId = this.bookId;
    this.selectedDate = null;
    this.userId = null;
    this.type = this.type
    this.loadUserCount();
    this.loadAllUser(this.bookId, this.type);
  }

  minDate(): string {
    // Get current date in YYYY-MM-DD format
    const today = new Date();
    const dd = String(today.getDate()).padStart(2, '0');
    const mm = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
    const yyyy = today.getFullYear();

    return `${yyyy}-${mm}-${dd}`;
  }


  loadBooks(): void {
    this.loading = true;
    this.bookService.getAllBooks().subscribe({
      next: (response: ApiResponse<Book[]>) => {
        if (response.success) {
          this.books = response.data;
        }
        else {
          this.books = [];
          console.error('Failed to fetch books', response.message);
        }
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.books = [];
        console.error(err.message);
        this.cdr.detectChanges();

      },
      complete: () => {
        this.loading = false;
        console.log("Completed");
      }
    });
  }



  loadUsers(): void {
    this.loading = true;
    this.authService.getAllUsers().subscribe({
      next: (response: ApiResponse<User[]>) => {
        if (response.success) {
          this.users = response.data;
          if (this.users.length > 0) {
            this.users.shift();
          }
        }
        else {
          this.users = [];
          console.error('Failed to fetch users', response.message);
        }
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.users = [];
        console.error(err.message);
        this.cdr.detectChanges();

      },
      complete: () => {
        this.loading = false;
        console.log("Completed");
      }
    });
  }

  loadBookCountWithDateOrStudent() {
    this.reportService.getBookCountWithDateOrStudent(this.userId, this.selectedDate)
      .subscribe({
        next: (response: ApiResponse<number>) => {
          if (response.success) {
            this.totalBooks = response.data;
            console.log(this.totalBooks);
            this.calculateTotalPagesForBook();

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

  loadAllIssueBooks(userId: number | null, selectedDate: string | null): void {
    this.loading = true;
    this.reportService.getIssueBookWithIssueDateOrUser(userId, selectedDate, this.currentPage, this.pageSize).subscribe({
      next: (response: ApiResponse<AdminBookReport[]>) => {
        if (response.success) {
          console.log(response.data);
          this.userdetails = [];
          this.bookdetails = response.data;
        }
        else {
          this.bookdetails = [];
          console.error('Failed to fetch books', response.message);
        }
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.bookdetails = [];
        console.error(err.message);
        this.cdr.detectChanges();

      },
      complete: () => {
        this.loading = false;
        console.log("Completed");
      }

    })
  }

  loadUserCount() {
    this.reportService.getUserCount(this.bookId)
      .subscribe({
        next: (response: ApiResponse<number>) => {
          if (response.success) {
            this.totalUsers = response.data;
            console.log(this.totalUsers);
            this.calculateTotalPagesForUser();

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

  loadAllUser(bookId: number | null, type: string): void {
    this.loading = true;
    this.reportService.getUserWithBook(bookId, type, this.currentPage, this.pageSize).subscribe({
      next: (response: ApiResponse<AdminReportUser[]>) => {
        if (response.success) {
          console.log(response.data);
          this.bookdetails = [];
          this.userdetails = response.data;
        }
        else {
          this.bookdetails = [];
          console.error('Failed to fetch books', response.message);
        }
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.userdetails = [];
        console.error(err.message);
        this.cdr.detectChanges();

      },
      complete: () => {
        this.loading = false;
        console.log("Completed");
      }

    })
  }


}
