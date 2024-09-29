/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BookAddComponent } from './book-add.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BookService } from 'src/app/services/book.service';
import { Router } from '@angular/router';
import { BooksListComponent } from '../books-list/books-list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ApiResponse } from 'src/app/models/ApiResponse{T}';
import { of, throwError } from 'rxjs';
import { AddBook } from 'src/app/models/AddBook';

describe('BookAddComponent', () => {
  let component: BookAddComponent;
  let fixture: ComponentFixture<BookAddComponent>;
  let bookSpy : jasmine.SpyObj<BookService>;
  let router : Router;

  beforeEach(() => {
    bookSpy = jasmine.createSpyObj('BookService',['addBook']);
    TestBed.configureTestingModule({
      imports:[HttpClientTestingModule,FormsModule,ReactiveFormsModule,RouterTestingModule.withRoutes([{path:'bookList',component : BooksListComponent}])],
      declarations: [ BookAddComponent ],
      providers : [
        {
          provide : BookService,useValue : bookSpy
        },
        // {
        //    provide: Router, useClass: class { navigate = jasmine.createSpy('navigate'); } 
        // }
      ]
    });
    fixture = TestBed.createComponent(BookAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    router = TestBed.inject(Router);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should initialize bookForm with default values', () => {
    expect(component.bookForm).toBeDefined();
    expect(component.bookForm.get('title')).toBeTruthy();
    expect(component.bookForm.get('author')).toBeTruthy();
    expect(component.bookForm.get('totalQuantity')).toBeTruthy();
    expect(component.bookForm.get('availableQuantity')).toBeTruthy();
    expect(component.bookForm.get('issuedQuantity')).toBeTruthy();
    expect(component.bookForm.get('pricePerBook')).toBeTruthy();
  });
  it('should add book suessfully and nevigate to products list',()=>{
    //Arrange
    const mockBook:AddBook={
      title: 'Title 1',
      author: 'Author 1',
      totalQuantity: 10,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 10.2
    };

    component.bookForm.setValue(mockBook)
    const mockResponse: ApiResponse<string> = { success: true, data: '', message: '' };

    bookSpy.addBook.and.returnValue(of(mockResponse));

    //Act
    
    component.OnSubmit();

    //Assert
    expect(bookSpy.addBook).toHaveBeenCalledWith(mockBook);
   // expect(router.navigate).toHaveBeenCalledWith(['/products']);


  })
  it('should handle error when add book fails', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockBook:AddBook={
      title: 'Title 1',
      author: 'Author 1',
      totalQuantity: 10,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 10.2
    };

    component.bookForm.setValue(mockBook)
    const mockResponse: ApiResponse<string> = { success: false, data: '', message: 'Error' };
    
    bookSpy.addBook.and.returnValue(of(mockResponse));

    // Act
    component.OnSubmit();

    // Assert
   expect(bookSpy.addBook).toHaveBeenCalledWith(mockBook);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockResponse.message);
  })
  it('should handle error when http error', () => {
    // Arrange
    spyOn(window, 'alert');
    spyOn(router, 'navigate');
    const mockBook:AddBook={
      title: 'Title 1',
      author: 'Author 1',
      totalQuantity: 10,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 10.2
    };

    component.bookForm.setValue(mockBook)
    //const mockError = { error: { message: 'HTTP error' } };
    const mockError = { error: { message: 'Failed to add product' } };
    bookSpy.addBook.and.returnValue(throwError(mockError));

    // Act
    component.OnSubmit();

    // Assert
   expect(bookSpy.addBook).toHaveBeenCalledWith(mockBook);
    expect(router.navigate).not.toHaveBeenCalled(); // Should not navigate on error
    expect(window.alert).toHaveBeenCalledWith(mockError.error.message);
  })
});
