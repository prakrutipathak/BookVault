import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { UserBookReport } from 'src/app/models/user-report.model';
import { AuthService } from 'src/app/services/auth.service';
import { BookService } from 'src/app/services/book.service';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-userbookreport',
  templateUrl: './userbookreport.component.html',
  styleUrls: ['./userbookreport.component.css']
})

export class UserbookreportComponent implements OnInit {

  bookdetails: UserBookReport[] = [];
  totalBooks!: number;
  bookId: number | undefined;
  loading: boolean = false;
  issuedId: number | undefined;
  userIdString: string | undefined | null;
  userId: number | undefined;
  selectedDate: string | null = null;
  type: string = 'issue';

  currentPage = 1;
  pageSize = 4;
  totalPages: number[] = [];


  constructor(private authService: AuthService, private reportService: ReportService, private bookService: BookService, private router: Router, private route: ActivatedRoute, private cdr: ChangeDetectorRef) { }




  ngOnInit(): void {


    this.authService.getUserId().subscribe((userIdString: string | null | undefined) => {
      this.userIdString = userIdString;
      if (userIdString != null && userIdString != undefined) {
        this.userId = Number(userIdString);
        this.loadBookCount();
        this.loadBookDetail();
        this.cdr.detectChanges();
      }
      this.cdr.detectChanges();
    });

  }

  //
  calculateTotalPages() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalBooks / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }
  onPageChange(page: number) {
    this.currentPage = page;
    this.loadBookDetail();
  }
  onPageSizeChange() {
    this.currentPage = 1;
    this.loadBookCount();
    this.loadBookDetail();
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
      this.loadBookDetail();
    }
  }

  onNextPage(search?: string) {
    if (this.currentPage < this.totalPages.length) {
      this.currentPage++;
      this.loadBookDetail();
    }
  }

  //

  selectDate(): void {
    this.loadBookDetail();

  }

  onBookChange(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.type = selectElement.value;
    if(this.type == 'issue'){
      this.currentPage = 1;
    }
    this.loadBookCount();
    this.loadBookDetail();
  }

  minDate(): string {
    // Get current date in YYYY-MM-DD format
    const today = new Date();
    const dd = String(today.getDate()).padStart(2, '0');
    const mm = String(today.getMonth() + 1).padStart(2, '0'); // January is 0!
    const yyyy = today.getFullYear();

    return `${yyyy}-${mm}-${dd}`;
  }

  loadBookCount() {
    this.reportService.getBookCountForUser(this.userId, this.selectedDate,this.type)
      .subscribe({
        next: (response: ApiResponse<number>) => {
          if (response.success) {
            this.totalBooks = response.data;
            console.log(this.totalBooks);
            this.calculateTotalPages();

          }
          else {
            console.error('Failed to fetch books count', response.message);
          }
        },
        error: (error => {
          console.error('Failed to fetch books count', error);
          this.loading = false;
        })
      });
  }

  loadBookDetail(): void {
    this.loading = true;
    this.reportService.issueBooksReport(this.userId, this.selectedDate, this.type, this.currentPage, this.pageSize).subscribe({
      next: (response: ApiResponse<UserBookReport[]>) => {
        if (response.success) {
          console.log("book data", response.data);
          this.bookdetails = response.data;
        } else {
          this.bookdetails = [];
          console.error("Falied to fetch contact", response.message);
        }
        this.cdr.detectChanges();

      },
      error: (err) => {
        console.error('Failed to fetch books', err);
        this.bookdetails = [];
        this.cdr.detectChanges();
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
        console.log("Completed");
      }
    })
  }
  confirmReturn(id: number): void {
    if (confirm('Are you sure you want to return book?')) {
      this.issuedId = id;
      this.bookReturn();
    }
  }

  bookReturn(): void {
    this.bookService.returnBook(this.issuedId).subscribe({
      next: (response) => {
        if (response.success) {

          this.loadBookDetail();
        } else {
          alert(response.message);
        }
      },
      error: (err) => {
        alert(err.error.message);
      },
      complete: () => {
        console.log('Completed');
      }
    })
  }


}
