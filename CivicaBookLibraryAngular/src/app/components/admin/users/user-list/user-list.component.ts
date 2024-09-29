import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  sortedUsers: User[] = [];
  username: string | null | undefined;
  totalUsers!: number;
  pageSize = 4;
  currentPage = 1;
  loading: boolean = false;
  isAuthenticated: boolean = false;
  totalPages: number[] = [];
  sortOrder: string = 'default';
  search: string = '';

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef, private route: Router) { }

  ngOnInit(): void {
    this.searchUsers();
    this.loadAllUsers();
    this.authService.isAuthenticated().subscribe((authState: boolean) => {
      this.isAuthenticated = authState;
      this.cdr.detectChanges();
    });
  }
  totalUserCount(search?: string) {
    this.authService.getUsersCount(search)
      .subscribe({
        next: (response: ApiResponse<number>) => {
          if (response.success) {
            this.totalUsers = response.data;
            console.log(this.totalUsers);
            this.calculateTotalPages();

          }
          else {
            console.error('Failed to fetch users', response.message);
          }
        },
        error: (error => {
          console.error('Failed to fetch users', error);
          this.loading = false;
        })
      });
  }

  loadAllUsers(): void {
    this.loading = true;
    this.authService.getAllUsers().subscribe({
      next: (response: ApiResponse<User[]>) => {
        if (response.success) {
          console.log(response.data);
          this.sortedUsers = response.data;
        }
        else {
          console.error('Failed to fetch users.', response.message);
          alert('Failed to fetch users.');
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error fetching users.', error);
        this.loading = false;
      }
    });
  }

  loadPaginatedUsers(search?: string,) {
    this.loading = true;
    this.authService.getAllUsersByPagination(this.currentPage, this.pageSize, this.sortOrder, search)
      .subscribe({
        next: (response: ApiResponse<User[]>) => {
          if (response.success) {
            this.users = response.data;
            console.log(response.data);
          }
          else {
            console.error('Failed to fetch users', response.message);
          }
          this.loading = false;

        },
        error: (error => {
          console.error('Failed to fetch users', error);
          this.loading = false;
        })
      });
  }
  calculateTotalPages() {
    this.totalPages = [];
    const pages = Math.ceil(this.totalUsers / this.pageSize);
    for (let i = 1; i <= pages; i++) {
      this.totalPages.push(i);
    }
  }

  sortAsc() {
    this.sortOrder = 'asc';
    this.currentPage = this.currentPage;
    this.totalUserCount(this.search);
    this.loadPaginatedUsers(this.search);
    this.sortOrder = 'desc';
  }

  sortDesc() {
    this.sortOrder = 'desc';
    this.currentPage = this.currentPage;
    this.totalUserCount(this.search);
    this.loadPaginatedUsers(this.search);
    this.sortOrder = 'asc';
  }
  searchUsers() {
    //this.currentPage = 1;
    // this.loadPaginatedUsers(this.search);
    // this.totalUserCount(this.search);

    if (this.search != ''  && this.search.length > 2) {
      this.currentPage = 1;
      this.totalUserCount(this.search);
      this.loadPaginatedUsers(this.search);
    }
    else {
      this.currentPage = 1;
      this.totalUserCount();
      this.loadPaginatedUsers('');
    }
  }
  onPageChange(page: number, search?: string) {
    this.currentPage = page;
    this.loadPaginatedUsers(search);
  }
  onPageSizeChange(search?: string) {
    this.currentPage = 1;
    this.loadPaginatedUsers(search);
    this.totalUserCount(search);
  }
  calculateSrNo(index: number): number {
    return (this.currentPage - 1) * this.pageSize + index + 1;
  }
  isActive(pageNumber: number): boolean {
    return this.currentPage === pageNumber;
  }
  deleteUser(userId: number) {
    if (confirm('Are you sure you want to delete this user?')) {
      this.authService.deleteUser(userId).subscribe(() => {
        this.totalUsers--;
        const pages = Math.ceil(this.totalUsers / this.pageSize);
        if (this.currentPage > pages) {
          this.currentPage = pages;
        }
        this.loadPaginatedUsers(this.search);
        this.totalUserCount(this.search);
      });
    }
  }
  onPrevPage(search?: string) {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadPaginatedUsers(search);
    }
  }

  onNextPage(search?: string) {
    if (this.currentPage < this.totalPages.length) {
      this.currentPage++;
      this.loadPaginatedUsers(search);
    }
  }
}