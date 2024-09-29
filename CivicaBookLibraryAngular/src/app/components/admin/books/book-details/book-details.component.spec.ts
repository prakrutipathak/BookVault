/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BookDetailsComponent } from './book-details.component';
import { Book } from 'src/app/models/Book';
import { BookService } from 'src/app/services/book.service';
import { ActivatedRoute, Router } from '@angular/router';
import { BooksListComponent } from '../books-list/books-list.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { EditBook } from 'src/app/models/EditBook';

describe('BookDetailsComponent', () => {
  let component: BookDetailsComponent;
  let fixture: ComponentFixture<BookDetailsComponent>;
  let bookSpy : jasmine.SpyObj<BookService>;
  let router: Router;
  const mockBook:EditBook={
    title: 'Title 1',
    author: 'Author 1',
    totalQuantity: 10,
    availableQuantity: 0,
    issuedQuantity: 0,
    pricePerBook: 10.2,
    bookId: 1
  };
  beforeEach(() => {
    bookSpy = jasmine.createSpyObj('BookService',['getBookById','deleteBook']);

    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,FormsModule,ReactiveFormsModule,RouterTestingModule],
      declarations: [ BookDetailsComponent ],
      providers : [
        {
          provide : BookService, useValue : bookSpy
        },
        {
        provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (paramMap: string) => '1' // Simulating param 'BookId' with value '1'
              }
            }
          }
        }
      ]
    })
    
    fixture = TestBed.createComponent(BookDetailsComponent);
    component = fixture.componentInstance;
    bookSpy = TestBed.inject(BookService) as jasmine.SpyObj<BookService>;
    // fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize BookId from route params and load Book details', () => {
    // Arrange
    const mockResponse: ApiResponse<Book> = { success: true, data: mockBook, message: '' };
    bookSpy.getBookById.and.returnValue(of(mockResponse));

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(component.book.bookId).toBe(1);
    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);
    expect(component.book).toEqual(mockBook);
  });

  it('should fail to load Book details', () => {
    // Arrange
    const mockResponse: ApiResponse<Book> = { success: false, data: mockBook, message: 'Failed to fetch book.' };
    bookSpy.getBookById.and.returnValue(of(mockResponse));
    spyOn(console, 'error')
    
    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(console.error).toHaveBeenCalledWith('Failed to fetch book.',mockResponse.message)
    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);
  });

  it('should handle http error', () => {
    // Arrange
    const mockError = { message: 'Failed to fetch book.' };
    bookSpy.getBookById.and.returnValue(throwError(() => mockError));
    spyOn(console, 'error')

    // Act
    fixture.detectChanges(); // ngOnInit is called here

    // Assert
    expect(console.error).toHaveBeenCalledWith("Failed to fetch book.",mockError)
    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);
  });

  it('should delete Book and navigate to books list', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: true, data: '', message: 'Book deleted successfully' };
    const BookId = 1;
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(router, 'navigate').and.stub();
    bookSpy.deleteBook.and.returnValue(of(mockDeleteResponse));

    // Act
    component.deleteBook(BookId);

    // Assert
    //expect(window.confirm).toHaveBeenCalledWith("Are you sure?")
    expect(bookSpy.deleteBook).toHaveBeenCalledWith(BookId);
    expect(router.navigate).toHaveBeenCalledWith(['/bookList']);
  });


  it('should not delete Book if user cancels', () => {
    // Arrange
    const mockDeleteResponse: ApiResponse<string> = { success: false, data: '', message: 'Failed to delete Book' };
    const BookId = 1;
    spyOn(window, 'confirm').and.returnValue(false);
    spyOn(router, 'navigate').and.stub();
    bookSpy.deleteBook.and.returnValue(of(mockDeleteResponse));

    // Act
    component.deleteBook(BookId);

    // Assert
    expect(window.confirm).toHaveBeenCalledWith("Are you sure you want to delete this book?")
    expect(bookSpy.deleteBook).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
