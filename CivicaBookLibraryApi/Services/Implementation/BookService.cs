using CivicaBookLibraryApi.Data;
using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using static System.Reflection.Metadata.BlobBuilder;

namespace CivicaBookLibraryApi.Services.Implementation
{
    public class BookService: IBookService
    {
        private readonly IBookRepository _bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public ServiceResponse<IEnumerable<BookDto>> GetAllBooks()
        {
            var response = new ServiceResponse<IEnumerable<BookDto>>();
            var books = _bookRepository.GetAllBooks();
            if (books != null && books.Any())
            {
                List<BookDto> bookDtos = new List<BookDto>();
                foreach (var book in books)
                {
                    bookDtos.Add(new BookDto()
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        Author = book.Author,
                        TotalQuantity = book.TotalQuantity,
                        AvailableQuantity = book.AvailableQuantity,
                        IssuedQuantity = book.IssuedQuantity,
                        PricePerBook = book.PricePerBook,

                    });
                }
                response.Success = true;
                response.Data = bookDtos;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found!";
            }
            return response;
        }
        public ServiceResponse<IEnumerable<BookDto>> GetPaginatedBooks(int page, int pageSize, string? search, string sortOrder,string? sortBy)
        {
            var response = new ServiceResponse<IEnumerable<BookDto>>();
            var books = _bookRepository.GetPaginatedBooks(page, pageSize, search, sortOrder,sortBy);

            if (books != null && books.Any())
            {
                List<BookDto> booksDto = new List<BookDto>();
                foreach (var book in books.ToList())
                {
                    booksDto.Add(new BookDto()
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        Author = book.Author,
                        TotalQuantity = book.TotalQuantity,
                        AvailableQuantity = book.AvailableQuantity,
                        IssuedQuantity = book.IssuedQuantity,
                        PricePerBook = book.PricePerBook,

                    });
                }


                response.Data = booksDto;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "No records found";
            }

            return response;
            }
        public ServiceResponse<BookDto> GetBookById(int id)
        {
            var response = new ServiceResponse<BookDto>();
            var existingBook = _bookRepository.GetBookById(id);
            if (existingBook != null)
            {
                var book = new BookDto()
                {
                    BookId = existingBook.BookId,
                    Title = existingBook.Title,
                    Author = existingBook.Author,
                    TotalQuantity = existingBook.TotalQuantity,
                    AvailableQuantity = existingBook.AvailableQuantity,
                    IssuedQuantity = existingBook.IssuedQuantity,
                    PricePerBook = existingBook.PricePerBook,

                };
                response.Success = true;
                response.Data = book;
            }

            else
            {
                response.Success = false;
                response.Message = "Something went wrong,try after sometime";
            }
            return response;
        }
        public ServiceResponse<int> TotalBooks(string? search)
        {
            var response = new ServiceResponse<int>();
            int totalPositions = _bookRepository.TotalBooks(search);

            response.Success = true;
            response.Data = totalPositions;
            return response;
        }
        public ServiceResponse<string> AddBook(AddBookDto bookDto)
        {
            var response = new ServiceResponse<string>();
            if (_bookRepository.BookExists(bookDto.Title,bookDto.Author))
            {
                response.Success = false;
                response.Message = "Book Already Exists";
                return response;
            }
            var book = new Book()

            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                TotalQuantity = bookDto.TotalQuantity,
                AvailableQuantity = bookDto.TotalQuantity,
                IssuedQuantity = 0,
                PricePerBook = bookDto.PricePerBook,
            };

            var result = _bookRepository.InsertBook(book);
            if (result)
            {
                response.Success = true;
                response.Message = "Book Saved Successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong. Please try later";
            }
            return response;
        }
        public ServiceResponse<string> ModifyBook(BookDto bookDto)
        {
            var response = new ServiceResponse<string>();
            var message = string.Empty;
            if (_bookRepository.BookExists(bookDto.BookId, bookDto.Title,bookDto.Author))
            {
                response.Success = false;
                response.Message = "Book already exists.";
                return response;

            }

            var existingBook = _bookRepository.GetBookById(bookDto.BookId);
            var result = false;
            if (existingBook != null)
            {
                existingBook.Title = bookDto.Title;
                existingBook.Author = bookDto.Author;
                existingBook.TotalQuantity = bookDto.TotalQuantity;
                existingBook.AvailableQuantity = bookDto.TotalQuantity-bookDto.IssuedQuantity;
                existingBook.IssuedQuantity = bookDto.IssuedQuantity;
                existingBook.PricePerBook = bookDto.PricePerBook;
                
                result = _bookRepository.UpdateBook(existingBook);
            }
            if (result)
            {
                response.Success = true;
                response.Message = "Book updated successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong,try after sometime";
            }
            return response;

        }
        public ServiceResponse<string> RemoveBook(int id)
        {
            var response = new ServiceResponse<string>();
            var result = _bookRepository.DeleteBook(id);

            if (result)
            {
                response.Success = true;
                response.Message = "Book deleted successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong";
            }

            return response;
        }

        public ServiceResponse<string> AddBookIssue(BookIssue bookIssue)
        {
            var response = new ServiceResponse<string>();
            if (_bookRepository.BookIssueExists(bookIssue.BookId,bookIssue.UserId))
            {
                response.Success = false;
                response.Message = "Book is already issued.";
                return response;
            }

            if (_bookRepository.GetIssuedBookCountForUser(bookIssue.UserId) >= 2)
            {
                response.Success = false;
                response.Message = "User already has 2 books issued. Cannot issue more.";
                return response;
            }

            var result = _bookRepository.InsertBookIssue(bookIssue);
            if (result)
            {
                response.Success = true;
                response.Message = "Book Issued successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong after sometime";
            }
            return response;
        }
        public ServiceResponse<string> SubmitBook(int id)
        {
            var response = new ServiceResponse<string>();
            var result = _bookRepository.SubmitBook(id);
            if (result)
            {
                response.Success = true;
                response.Message = "Book Return Successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong please try after sometime";
            }
            return response;
        }
    }
}
