/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { BookService } from './book.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiResponse } from '../models/ApiResponse{T}';
import { Book } from '../models/Book';
import { AddBook } from '../models/AddBook';
import { EditBook } from '../models/EditBook';
import { BookIssue } from '../models/bookIssue.model';

describe('BookService', () => {
  let service: BookService;
  let httpMock: HttpTestingController;
  
  const mockApiResponse: ApiResponse<Book[]>={
    success: true,
    data: [
      { bookId: 1, 
        title: 'Title 1',
        author: 'Author 1',
        totalQuantity: 4,
        availableQuantity: 2,
        issuedQuantity: 1,
        pricePerBook: 100
      },
      {
        bookId: 2, 
        title: 'Title 2',
        author: 'Author 2',
        totalQuantity: 4,
        availableQuantity: 2,
        issuedQuantity: 1,
        pricePerBook: 200
      }
    ],
    message: ''
  };
  

  const mockSuccessResponse: ApiResponse<string>={
    success: true,
    data: "",
    message: "Book Saved Successfully"
  };

  const mockErrorResponse: ApiResponse<string>={
    success: false,
    data: "",
    message: "Book Already Exists"
  };

  const mockHttpError={
    status: 500,
    statusText: "Internal Server Error."
  };


  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BookService],
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(BookService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(()=>{
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });


  //getAllBooks
  it('should fetch all books successfully', ()=>{
    //Arrange
    const apiUrl='http://localhost:5159/api/Book/GetAllBooks';

    //Act
    service.getAllBooks().subscribe((response)=>{
      //Assert
      expect(response.data.length).toBe(2);
      expect(response.data).toEqual(mockApiResponse.data);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);

  });


  it('should handle an empty book list', () => {
    //Arrange
    const apiUrl='http://localhost:5159/api/Book/GetAllBooks';

    const emptyResponse: ApiResponse<Book[]>={
      success: true,
      data: [],
      message: ''
    }

    //Act
    service.getAllBooks().subscribe((response)=>{
      //Assert
      expect(response.data.length).toBe(0);
      expect(response.data).toEqual([]);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(emptyResponse);
  });


  it('should handle HTTP error gracefully', ()=>{
    //Arrange
    const apiUrl='http://localhost:5159/api/Book/GetAllBooks';
    const errorMessage = 'Failed to fetch books.';

    //Act
    service.getAllBooks().subscribe(
      ()=>fail('expected an error, not books'),
      (error) => {
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal server error');
      }
    );
    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');

    req.flush(errorMessage, {status: 500, statusText:'Internal server error'});

  });



  //addBook
  it('should add a book successfully',()=>{
    //Arrange
    const addBook: AddBook={
      title: "New book",
      author: "Author of new book",
      totalQuantity: 0,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 0
    }

    const mockSuccessResponse: ApiResponse<string>={
      success: true,
      data: '',
      message: "Book Saved Successfully"
    };

    //Act
    service.addBook(addBook).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockSuccessResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/InsertBook');
    expect(req.request.method).toBe('POST');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed addition',()=>{
    //Arrange
    const addBook: AddBook={
      title: "New book",
      author: "author of new book",
      totalQuantity: 0,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 0
    }

    const mockErrorResponse: ApiResponse<string>={
      success: false,
      data: '',
      message: "Book Already Exists"
    };

    //Act
    service.addBook(addBook).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockErrorResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/InsertBook');
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse);
  });


  it('should handle HTTP error',()=>{
    //Arrange
    const addBook: AddBook={
      title: "New title",
      author: "Author of new book",
      totalQuantity: 0,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 0
    }

    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.addBook(addBook).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/InsertBook');
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });




  //getBookById
  it('should fetch a book by id successfully', ()=>{
    //Arrange
    const bookId = 1;
    const mockSuccessResponse: ApiResponse<EditBook>={
      success: true,
      data: {
        bookId: bookId,
        title: '',
        author: '',
        totalQuantity: 0,
        availableQuantity: 0,
        issuedQuantity: 0,
        pricePerBook: 0
      },
      message:''
    };

    //Act
    service.getBookById(bookId).subscribe(response => {
      //Assert
      expect(response.success).toBeTrue();
      expect(response.message).toBe('');
      expect(response.data).toEqual(mockSuccessResponse.data);
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/GetBookById?id='+bookId);
    expect(req.request.method).toBe('GET');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed book retrieval', () => {
    //Arrange
    const bookId=1;
    const mockErrorResponse: ApiResponse<EditBook>={
      success: false,
      data: {
        bookId: bookId,
        title: '',
        author: '',
        totalQuantity: 0,
        availableQuantity: 0,
        issuedQuantity: 0,
        pricePerBook: 0
      },
      message: 'No record found!'
    };

    //Act
    service.getBookById(bookId).subscribe(response => {
      //Assert
      expect(response).toEqual(mockErrorResponse);
      expect(response.message).toEqual("No record found!");
      expect(response.success).toBeFalse();
    });
    
    const req=httpMock.expectOne('http://localhost:5159/api/Book/GetBookById?id='+bookId);
    expect(req.request.method).toBe('GET');
    req.flush(mockErrorResponse);

  });


  it('should handle HTTP errors', () => {
    //Arrange
    const bookId = 3;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getBookById(bookId).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/GetBookById?id='+bookId);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });



  //modifyBook
  it('should update book successfully', ()=>{
    //Arrange
    const updatedBook: EditBook = {
      bookId: 1,
      title: 'Updated title',
      author: 'Updated author',
      totalQuantity: 0,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 0
    };

    const mockSuccessResponse: ApiResponse<string>={
      data: '',
      success: true,
      message: 'Book updated successfully.'
    };

    //Act
    service.modifyBook(updatedBook).subscribe(
      response =>{
        //Assert
        expect(response).toEqual(mockSuccessResponse);
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/ModifyBook');
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed updation',()=>{
    //Arrange
    const updatedBook: EditBook = {
      bookId: 1,
      title: 'Updated title',
      author: 'Updated description',
      totalQuantity: 0,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 0
    };

    const mockErrorResponse: ApiResponse<string>={
      success: false,
      data: '',
      message: "Book already exists."
    };

    //Act
    service.modifyBook(updatedBook).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockErrorResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/ModifyBook');
    expect(req.request.method).toBe('PUT');
    req.flush(mockErrorResponse);
  });


  it('should handle HTTP error while updating',()=>{
    //Arrange
    const updatedBook: EditBook = {
      bookId: 1,
      title: 'New Title',
      author: 'New author',
      totalQuantity: 0,
      availableQuantity: 0,
      issuedQuantity: 0,
      pricePerBook: 0
    };

    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.modifyBook(updatedBook).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/ModifyBook');
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });




  //deleteBook
  it('should delete a book by id successfully', ()=> {
    //Arrange
    const bookId = 1;
    const mockSuccessResponse: ApiResponse<string>={
      success: true,
      data:'',
      message: "Book deleted successfully"
    };

    //Act
    service.deleteBook(bookId).subscribe(response => {
      //Assert
      expect(response).toEqual(mockSuccessResponse);
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/RemoveBook/'+bookId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed book delete', ()=> {
    //Arrange
    const bookId = 1;
    const mockErrorResponse: ApiResponse<Book>={
      success: false,
      data: {
        bookId: 1,
        title: '',
        author: '',
        totalQuantity: 0,
        availableQuantity: 0,
        issuedQuantity: 0,
        pricePerBook: 0
      },
      message: "Something went wrong"
    };

    //Act
    service.deleteBook(bookId).subscribe(response => {
      //Assert
      expect(response.message).toEqual('Something went wrong');
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/RemoveBook/'+bookId);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockErrorResponse);
  });


  it('should handle HTTP errors', () => {
    //Arrange
    const bookId = 3;
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.deleteBook(bookId).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/RemoveBook/'+bookId);
    expect(req.request.method).toBe('DELETE');
    req.flush({},mockHttpError);
  });




  //getAllBooksByPagination
  it('should retrieve all books without search and sort parameters', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';

    const mockResponse: ApiResponse<Book[]> = {
      success: true,
      data: [
        {
          bookId: 1,
          title: 'Book 1',
          author: 'Author 1',
          totalQuantity: 5,
          availableQuantity: 3,
          issuedQuantity: 2,
          pricePerBook: 10
        },
        {
          bookId: 2,
          title: 'Book 2',
          author: 'Author 2',
          totalQuantity: 8,
          availableQuantity: 6,
          issuedQuantity: 2,
          pricePerBook: 12
        }
      ],
      message: 'Books retrieved successfully'
    };

    // Act
    service.getAllBooksByPagination(page, pageSize, sortOrder).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/Book/GetAllBooksByPagination?page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should retrieve books with search parameter only', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const search = 'query';

    const mockResponse: ApiResponse<Book[]> = {
      success: true,
      data: [
        {
          bookId: 1,
          title: 'Book 1',
          author: 'Author 1',
          totalQuantity: 5,
          availableQuantity: 3,
          issuedQuantity: 2,
          pricePerBook: 10
        }
      ],
      message: 'Books retrieved successfully'
    };

    // Act
    service.getAllBooksByPagination(page, pageSize, sortOrder, search).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/Book/GetAllBooksByPagination?search=${search}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should retrieve books with sort parameter only', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const sortBy = 'title';

    const mockResponse: ApiResponse<Book[]> = {
      success: true,
      data: [
        {
          bookId: 1,
          title: 'Book 1',
          author: 'Author 1',
          totalQuantity: 5,
          availableQuantity: 3,
          issuedQuantity: 2,
          pricePerBook: 10
        }
      ],
      message: 'Books retrieved successfully'
    };

    // Act
    service.getAllBooksByPagination(page, pageSize, sortOrder, undefined, sortBy).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/Book/GetAllBooksByPagination?sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should retrieve books with search and sort parameters', () => {
    // Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const search = 'query';
    const sortBy = 'title';

    const mockResponse: ApiResponse<Book[]> = {
      success: true,
      data: [
        {
          bookId: 1,
          title: 'Book 1',
          author: 'Author 1',
          totalQuantity: 5,
          availableQuantity: 3,
          issuedQuantity: 2,
          pricePerBook: 10
        }
      ],
      message: 'Books retrieved successfully'
    };

    // Act
    service.getAllBooksByPagination(page, pageSize, sortOrder, search, sortBy).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/Book/GetAllBooksByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should handle HTTP errors', () => {
    //Arrange
    const page = 1;
    const pageSize = 10;
    const sortOrder = 'asc';
    const search = 'query';
    const sortBy = 'title';

    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getAllBooksByPagination(page, pageSize, sortOrder, search, sortBy).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne(`http://localhost:5159/api/Book/GetAllBooksByPagination?search=${search}&sortBy=${sortBy}&page=${page}&pageSize=${pageSize}&sortOrder=${sortOrder}`);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });




  //getBooksCount
  it('should retrieve books count without search parameter', () => {
    // Arrange
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 10,
      message: 'Books count retrieved successfully'
    };

    // Act
    service.getBooksCount().subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne('http://localhost:5159/api/Book/GetBooksCount');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });



  it('should retrieve books count with search parameter', () => {
    // Arrange
    const search = 'query';
    const mockResponse: ApiResponse<number> = {
      success: true,
      data: 5,
      message: 'Books count retrieved successfully'
    };

    // Act
    service.getBooksCount(search).subscribe(response => {
      // Assert
      expect(response).toEqual(mockResponse);
    });

    // Assert
    const req = httpMock.expectOne(`http://localhost:5159/api/Book/GetBooksCount?search=${search}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });


  it('should handle HTTP errors', () => {
    //Arrange
    const search = 'query';
    const mockHttpError = {
      status: 500,
      statusText: 'Internal Server Error'
    };

    service.getBooksCount(search).subscribe({
      next: ()=>fail('should have failed with 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req=httpMock.expectOne(`http://localhost:5159/api/Book/GetBooksCount?search=${search}`);
    expect(req.request.method).toBe('GET');
    req.flush({},mockHttpError);
  });
  //bookreturn
  it('should  book returnsuccessfully', ()=>{
    //Arrange
    const mockSuccessResponse: ApiResponse<string>={
      data:"",
      success: true,
      message: 'Book updated successfully.'
    };

    //Act
    service.returnBook(1).subscribe(
      response =>{
        //Assert
        expect(response).toEqual(mockSuccessResponse);
    });

    const req=httpMock.expectOne('http://localhost:5159/api/Book/BookReturn/'+1);
    expect(req.request.method).toBe('PUT');
    req.flush(mockSuccessResponse);
  });

  it('should handle HTTP error while book return',()=>{
    //Arrange
    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.returnBook(1).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/BookReturn/'+1);
    expect(req.request.method).toBe('PUT');
    req.flush({}, mockHttpError);
  });
  //BookIssue
  
  it('should issue a book successfully',()=>{
    //Arrange
    const bookIssue: BookIssue={
      issueDate: "2024-06-25",
      returnDate: null,
      userId: 1,
      bookId: 1
     
    }

    const mockSuccessResponse: ApiResponse<string>={
      success: true,
      data: '',
      message: "Book Issue Successfully"
    };

    //Act
    service.bookIssue(bookIssue).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockSuccessResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/BookIssue');
    expect(req.request.method).toBe('POST');
    req.flush(mockSuccessResponse);
  });


  it('should handle failed while book issue',()=>{
    //Arrange
    const bookIssue: BookIssue={
      issueDate: "2024-06-25",
      returnDate: null,
      userId: 1,
      bookId: 1
     
    }

    const mockErrorResponse: ApiResponse<string>={
      success: false,
      data: '',
      message: "Book Already Issued"
    };

    //Act
    service.bookIssue(bookIssue).subscribe(response=>{
      
      //Assert
      expect(response).toBe(mockErrorResponse);
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/BookIssue');
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse);
  });


  it('should handle HTTP error while book issuing',()=>{
    //Arrange
    const bookIssue: BookIssue={
      issueDate: "2024-06-25",
      returnDate: null,
      userId: 1,
      bookId: 1
     
    }
    const mockHttpError={
      status: 500,
      statusText: "Internal Server Error."
    };

    //Act
    service.bookIssue(bookIssue).subscribe({
      next:()=> fail('should have failed with the 500 error'),
      error: (error)=>{
        //Assert
        expect(error.status).toEqual(500);
        expect(error.statusText).toEqual('Internal Server Error.');
    }
    });

    //Api call
    const req=httpMock.expectOne('http://localhost:5159/api/Book/BookIssue');
    expect(req.request.method).toBe('POST');
    req.flush({}, mockHttpError);
  });


});
