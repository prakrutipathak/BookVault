import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BooksListComponent } from './books-list.component';
import { BookService } from 'src/app/services/book.service';
import { ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { Book } from 'src/app/models/Book';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

describe('BooksListComponent', () => {
  let component: BooksListComponent;
  let fixture: ComponentFixture<BooksListComponent>;
  let bookServiceSpy: jasmine.SpyObj<BookService>;
  let router: Router;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let cdrSpy: jasmine.SpyObj<ChangeDetectorRef>;
  let mockTotalBooksCount = jasmine.createSpy('totalBooksCount');
  let mockLoadPaginatedBooks = jasmine.createSpy('loadPaginatedBooks');
  let mockLoadsearchBooks = jasmine.createSpy('searchBooks');
  let mockLoadloadAllBooks = jasmine.createSpy('loadAllBooks');

  const mockBook:Book = {
    bookId: 0,
    title: '',
    author: '',
    totalQuantity: 0,
    availableQuantity: 0,
    issuedQuantity: 0,
    pricePerBook: 0
  }
  const mockEmptyBookList : Book[] =[];
  const mockBookList :Book[] = [
    {
      bookId: 1,
      title: 'Title 1',
      author: 'Author 1',
      totalQuantity: 10,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 10.2
    },
    {
      bookId: 2,
      title: 'Title 2',
      author: 'Author 2',
      totalQuantity: 20,
      availableQuantity: 18,
      issuedQuantity: 2,
      pricePerBook: 89.2
    },
    {
      bookId: 3,
      title: 'Title 3',
      author: 'Author 3',
      totalQuantity: 10,
      availableQuantity: 18,
      issuedQuantity: 1,
      pricePerBook: 9.2
    }
  ];

    


  beforeEach(() => {
    bookServiceSpy=jasmine.createSpyObj('BookService',['getAllBooks','getAllBooksByPagination','getBooksCount','addBook','getBookById','modifyBook','deleteBook'])
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated','getUsername','isAuthenticated']);

    cdrSpy = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,RouterTestingModule],
      declarations: [BooksListComponent],
      providers: [
        { provide: BookService, useValue: bookServiceSpy },
        { provide: ChangeDetectorRef, useValue: cdrSpy }
      ],
    });
 
    fixture = TestBed.createComponent(BooksListComponent);
    component = fixture.componentInstance;
    
    // fixture.detectChanges();
    router = TestBed.inject(Router);
  });
  beforeEach(() => {
    mockTotalBooksCount = spyOn(component, 'totalBooksCount').and.callThrough();
    mockLoadPaginatedBooks = spyOn(component, 'loadPaginatedBooks').and.callThrough();
    mockLoadsearchBooks = spyOn(component, 'searchBooks').and.callThrough();
    mockLoadloadAllBooks = spyOn(component, 'loadAllBooks').and.callThrough();
    router = TestBed.inject(Router);
  });
  afterEach(() => {
    // Clear spies and reset state if needed
    bookServiceSpy.getAllBooks.calls.reset();
    bookServiceSpy.getAllBooksByPagination.calls.reset();
    bookServiceSpy.getBooksCount.calls.reset();
    bookServiceSpy.addBook.calls.reset();
    bookServiceSpy.getBookById.calls.reset();
    bookServiceSpy.modifyBook.calls.reset();
    bookServiceSpy.deleteBook.calls.reset();
    authServiceSpy.isAuthenticated.calls.reset();
    mockTotalBooksCount.calls.reset();
    mockLoadPaginatedBooks.calls.reset();
    mockLoadsearchBooks.calls.reset();
    mockLoadloadAllBooks.calls.reset();
    cdrSpy.detectChanges.calls.reset();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should calaulate total book count without search',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: true, data: 3, message: ''
    };
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));

    //Act
    component.totalBooksCount();

    //Assert
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalled();

  })

  it('should calaulate total book count with search',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: true, data: 3, message: ''
    };
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));

    //Act
    component.totalBooksCount('search');

    //Assert
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalled();

  })
  it('should fail to calaulate total book count ',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: false, data: 0, message: 'Failed to fetch books'
    };
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.totalBooksCount();

    //Assert
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockResponse.message);

  })
  it('should fail to calaulate total book count ',()=>{
    //Arrange
    const mockResponse :ApiResponse<number> ={
      success: false, data: 3, message: 'Failed to fetch books'
    };
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.totalBooksCount();

    //Assert
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockResponse.message);

  })
  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    bookServiceSpy.getBooksCount.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.totalBooksCount();

    //Assert
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockError);

  })
  it('should load books successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    bookServiceSpy.getAllBooks.and.returnValue(of(mockResponse));
    //Act
    component.loadAllBooks();

    //Assert
    expect(bookServiceSpy.getAllBooks).toHaveBeenCalled();
    expect(component.sortedBooks).toEqual(mockBookList);
    expect(component.loading).toBe(false);
  })
  it('should fail to load books ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: false, data:mockEmptyBookList , message: 'Failed to fetch books.',
    };
    bookServiceSpy.getAllBooks.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadAllBooks();

    //Assert
    expect(bookServiceSpy.getAllBooks).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    bookServiceSpy.getAllBooks.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadAllBooks();

    //Assert
    expect(bookServiceSpy.getAllBooks).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Error fetching books.',mockError);

  })
  it('should load paginated books successfully',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedBooks();

    //Assert
    expect(bookServiceSpy.getAllBooksByPagination).toHaveBeenCalled();
    expect(component.books).toEqual(mockBookList);
    expect(component.loading).toBe(false);
  })
  it('should fail to load paginated books ',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: false, data:mockBookList , message: 'Failed to fetch books',
    };
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse));
    spyOn(console,'error')
    //Act
    component.loadPaginatedBooks();

    //Assert
    expect(bookServiceSpy.getAllBooksByPagination).toHaveBeenCalled();
    expect(component.loading).toBe(false);
  })

  it('should handle Http error response',()=>{
    //Arrange
    const mockError = {message:'Network Error'};
    bookServiceSpy.getAllBooksByPagination.and.returnValue(throwError(mockError));
    spyOn(console,'error')

    //Act
    component.loadPaginatedBooks();

    //Assert
    expect(bookServiceSpy.getAllBooksByPagination).toHaveBeenCalled();
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books',mockError);

  })
  it('should load paginated books successfully with search',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedBooks("Title 1");

    //Assert
    expect(bookServiceSpy.getAllBooksByPagination).toHaveBeenCalled();
    expect(component.books).toEqual(mockBookList);
    expect(component.loading).toBe(false);
  })
  it('should load paginated books successfully with sortBy and Search',()=>{
    //Arrange
   
    const mockResponse :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse));
    //Act
    component.loadPaginatedBooks("Title 1","title");

    //Assert
    expect(bookServiceSpy.getAllBooksByPagination).toHaveBeenCalled();
    expect(component.books).toEqual(mockBookList);
    expect(component.loading).toBe(false);
  })

  it('should toggle sortOrder and reload data when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.sortOrder = 'asc';

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.sortColumn(initialColumn);

    expect(component.sortOrder).toBe('desc');
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalledWith(component.search);

  });
  it('should go to default sortOrder and reload data when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.sortOrder = 'asc';

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.sortColumn('default');

    expect(component.sortOrder).toBe('desc');
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalledWith(component.search);

  });

  it('should maintain currentPage when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    const initialPage = component.currentPage;
    component.sortBy = initialColumn;
    component.sortOrder = 'asc';

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.sortColumn(initialColumn);

    expect(component.currentPage).toBe(initialPage);

  });
  it('should toggle sortOrder and reload data when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.sortOrder = 'desc';

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.sortColumn(initialColumn);

    expect(component.sortOrder).toBe('asc');
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalledWith(component.search);

  });

  it('should maintain currentPage when sorting by the same column', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    const initialPage = component.currentPage;
    component.sortBy = initialColumn;
    component.sortOrder = 'desc';

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.sortColumn(initialColumn);

    expect(component.currentPage).toBe(initialPage);

  });
  it('should search book', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=1;
   

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.searchBooks();

    expect(component.sortOrder).toBe('asc');

  });
  it('should search book if string empty', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=1;
    component.search ='ert'
   if(component.search != ''  && component.search.length > 2){
    component.currentPage=1;
   }
    bookServiceSpy.getBooksCount('');
    bookServiceSpy.getAllBooksByPagination(1,2,'');
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.searchBooks();

    expect(component.sortOrder).toBe('asc');
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalledWith(component.search);

  });
  it('should load paginated page on pageChange()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    let page = 2;
    component.sortBy = initialColumn;
    component.currentPage=page;
   
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.onPageChange(page,initialColumn);

    expect(component.sortOrder).toBe('asc');

  });
  it('should load paginated page on pagesizeChange()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=1;
   let search = 'title'

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.onPageSizeChange(search);

    expect(component.sortOrder).toBe('asc');
    expect(bookServiceSpy.getBooksCount).toHaveBeenCalledWith(search);
  });
  it('should calculate serial number correctly', () => {
    const index = 0; 
    const expectedSerialNumber = (component.currentPage - 1) * component.pageSize + index + 1;
    const actualSerialNumber = component.calculateSrNo(index);
    component.calculateSrNo(index);
    expect(actualSerialNumber).toBe(expectedSerialNumber);
  });
  it('should show active button differently', () => {
    const pageNumber = 1; 
    component.currentPage === pageNumber;
    const expectedPageNumber =component.currentPage === pageNumber;;
    const actualPageNumber = component.isActive(pageNumber);
    component.isActive(pageNumber);
    expect(actualPageNumber).toBe(expectedPageNumber);
  });
  it('should load previous paginated page on onPrevPage()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=2;
    let search = 'title'

    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.onPrevPage(search);

    expect(component.sortOrder).toBe('asc');
  });
  it('should load next paginated page on onNextPage()', () => {
    const mockResponse :ApiResponse<number> ={
      success: true, data: 10, message: '',
    };
    const mockResponse1 :ApiResponse<Book[]> ={
      success: true, data: mockBookList, message: '',
    };
    const initialColumn = 'title';
    component.sortBy = initialColumn;
    component.currentPage=3;
    let search = 'title'
    component.totalPages.length = 6;
    
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse1));

    component.onNextPage(search);

    expect(component.sortOrder).toBe('asc');
  });

  it('should not delete book when user cancels deletion', () => {
    const bookId = 2;
    spyOn(window, 'confirm').and.returnValue(false); 

    component.deleteBook(bookId);

    expect(window.confirm).toHaveBeenCalled();
    expect(bookServiceSpy.deleteBook).not.toHaveBeenCalled(); 

    expect(component.loadPaginatedBooks).not.toHaveBeenCalled();
    expect(component.totalBooksCount).not.toHaveBeenCalled();
  });
  it('should delete books successfully',()=>{
    //Arrange
   let bookId = 1;
   const mockResponse :ApiResponse<number> ={
    success: true, data: 10, message: '',
  };
  const mockResponse1 :ApiResponse<string> ={
    success: true, data: '', message: '',
  };
  const mockResponse2 :ApiResponse<Book[]> ={
    success: true, data: mockBookList, message: '',
  };
  const initialColumn = 'title';
  component.sortBy = initialColumn;
  component.currentPage=1;
 
  spyOn(window, 'confirm').and.returnValue(true); 
 
    bookServiceSpy.deleteBook.and.returnValue(of(mockResponse1));
    bookServiceSpy.getBooksCount.and.returnValue(of(mockResponse));
    bookServiceSpy.getAllBooksByPagination.and.returnValue(of(mockResponse2));
  
    //Act
    component.deleteBook(bookId);

    //Assert
    expect(bookServiceSpy.deleteBook).toHaveBeenCalled();
  })
});
