/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BookModifyComponent } from './book-modify.component';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService } from 'src/app/services/book.service';
import { EditBook } from 'src/app/models/EditBook';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { BooksListComponent } from '../books-list/books-list.component';
import { Book } from 'src/app/models/Book';

describe('BookModifyComponent', () => {
  let component: BookModifyComponent;
  let fixture: ComponentFixture<BookModifyComponent>;
  let bookSpy : jasmine.SpyObj<BookService>;
  let router: Router;

  const mockBook:Book={
    bookId: 1,
    title: 'Book Title',
    author: 'Mock Author',
    totalQuantity: 10,
    availableQuantity: 5,
    issuedQuantity: 5,
    pricePerBook: 15.99
  };
  beforeEach(() => {
    bookSpy = jasmine.createSpyObj('BookService',['getBookById','modifyBook']);
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,RouterTestingModule.withRoutes([{path : 'bookList',component:BooksListComponent}]),ReactiveFormsModule],
      declarations: [ BookModifyComponent ],
      providers : [
        {
          provide : BookService, useValue : bookSpy
        },
        {
        provide: ActivatedRoute,
          useValue: {
            params:of({BookId:1})
          }
        }
      ]
    });
    fixture = TestBed.createComponent(BookModifyComponent);
    component = fixture.componentInstance;
    bookSpy = TestBed.inject(BookService) as jasmine.SpyObj<BookService>;
    router = TestBed.inject(Router) as any;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should fetch book details and update form on successful response', () => {
    const mockApiResponse: ApiResponse<EditBook> = { success: true, data: mockBook, message: '' };
    bookSpy.getBookById.and.returnValue(of(mockApiResponse));

    component.getBook(1); // Simulating getting book with ID 1

    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);

  });

  it('should handle error when fetching book details fails', () => {
    const mockError:ApiResponse<EditBook> = {
      message: 'Failed to fetch book', success: false,
      data: mockBook
    };
    bookSpy.getBookById.and.returnValue(of(mockError));

    spyOn(console, 'error');

    component.getBook(1); // Simulating getting book with ID 1

    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);
    expect(console.error).toHaveBeenCalledWith('Failed to fetch books', mockError.message);
    expect(component.loading).toBeFalse(); // Assuming loading state is updated correctly
  });
  it('should set form values correctly', () => {
    const mockBookData = {
      bookId: 1,
      title: 'Book Title',
      author: 'Mock Author',
      totalQuantity: 10,
      availableQuantity: 5,
      issuedQuantity: 5,
      pricePerBook: 15.99
    };
    bookSpy.getBookById.and.returnValue(of({ success: true, data: mockBookData,message:'' }));
  
    
  
    expect(component.bookForm.value.title).toEqual('');
    expect(component.bookForm.value.author).toEqual('');
  });
  it('should handle error on fething data', () => {
    // Arrange
    const mockResponse: ApiResponse<Book> = { success: false, data: mockBook, message: 'Failed to fetch books' };
    bookSpy.getBookById.and.returnValue(of(mockResponse));
    spyOn(console,"error")

    // Act
    fixture.detectChanges(); // ngOnInit is called here
    // component.ngOnInit();

    // Assert
    expect(component.bookId).toBe(1);
    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);
  });
  it('should handle Http error for fetching data', () => {
    // Arrange
    
    const mockError = { error: { message: 'Failed to fetch books' } };
    bookSpy.getBookById.and.returnValue(throwError(mockError));
    spyOn(console,"error");
    spyOn(window,"alert");

    // Act
    fixture.detectChanges(); // ngOnInit is called here
    // component.ngOnInit();

    // Assert
    expect(component.bookId).toBe(1);
    expect(bookSpy.getBookById).toHaveBeenCalledWith(1);
  });

  it('should update book suessccfully and navigate to books list',()=>{
    //Arrange
    spyOn(router,"navigate")

    const mockBooks = {
      bookId: 1,
      title: 'Mock Book Title',
      author: 'Mock Author',
      totalQuantity: 10,
      availableQuantity: 5,
      issuedQuantity: 5,
      pricePerBook: 15.99
    };
    component.bookForm.setValue(mockBooks);
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: 'Book added successfully' };
    bookSpy.modifyBook.and.returnValue(of(mockResponse));

    //Act
    
    component.OnSubmit();

    //Assert
    expect(bookSpy.modifyBook).toHaveBeenCalledWith(mockBooks);
    expect(router.navigate).toHaveBeenCalledWith(['/bookList']);


  })

  it('should handle error when add book fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');

    component.bookForm.setValue(mockBook);
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    bookSpy.modifyBook.and.returnValue(of(mockResponse));

    // Act
    component.OnSubmit();

    // Assert
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message);

  })


  it('should handle error when http error on update book', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');

    component.bookForm.setValue(mockBook)
    const mockError = { error: { message: 'HTTP error' } };
    bookSpy.modifyBook.and.returnValue(throwError(mockError));

    // Act
    component.OnSubmit();

    // Assert
   expect(bookSpy.modifyBook).toHaveBeenCalledWith(component.bookForm.value);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  })
  it('should handle error when http error on update book', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');

    component.bookForm.setValue(mockBook)
    const mockError = { error: { message: 'HTTP error' } };
    bookSpy.getBookById.and.returnValue(throwError(mockError));

    // Act
    component.getBook(1);

    // Assert
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  })
});
