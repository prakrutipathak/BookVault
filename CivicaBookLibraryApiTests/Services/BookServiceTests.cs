using AutoFixture;
using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Implementation;
using Fare;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace CivicaBookLibraryApiTests.Services
{
    public class BookServiceTests
    {
        //GetAllBooks
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetAllBooks_ReturnList_WhenNoBooksExist()
        {
            //Arrange
            var mockRepository = new Mock<IBookRepository>();
            var target = new BookService(mockRepository.Object);
            //Act
            var actual = target.GetAllBooks();

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("No record found!", actual.Message);
            Assert.False(actual.Success);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetAllBooks_ReturnsBooksList_WhenBooksExist()
        {

            //Arrange
            var books = new List<Book>
        {
            new Book
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            },
            new Book
            {
                   BookId = 2,
               Title = "Title 2",
                Author = "Author 2",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            }
        };

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.GetAllBooks()).Returns(books);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.GetAllBooks();

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            mockRepository.Verify(c => c.GetAllBooks(), Times.Once);
            Assert.Equal(books.Count, actual.Data.Count()); // Ensure the counts are equal

            for (int i = 0; i < books.Count; i++)
            {
                Assert.Equal(books[i].BookId, actual.Data.ElementAt(i).BookId);
                Assert.Equal(books[i].Title, actual.Data.ElementAt(i).Title);
                Assert.Equal(books[i].Author, actual.Data.ElementAt(i).Author);
                Assert.Equal(books[i].TotalQuantity, actual.Data.ElementAt(i).TotalQuantity);
                Assert.Equal(books[i].AvailableQuantity, actual.Data.ElementAt(i).AvailableQuantity);
                Assert.Equal(books[i].IssuedQuantity, actual.Data.ElementAt(i).IssuedQuantity);
                Assert.Equal(books[i].PricePerBook, actual.Data.ElementAt(i).PricePerBook);
            }
        }
        //SubmitBook
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void SubmitBook_ReturnsBooksSuccessfully()
        {
            var issueId = 1;


            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.SubmitBook(issueId)).Returns(true);

            var contactService = new BookService(mockRepository.Object);

            // Act
            var actual = contactService.SubmitBook(issueId);

            // Assert
            Assert.True(actual.Success);
            Assert.NotNull(actual);
            Assert.Equal("Book Return Successfully", actual.Message);
            mockRepository.Verify(r => r.SubmitBook(issueId), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void SubmitBook_SomethingWentWrong_WhenReturnBooksFailed()
        {
            var issueId = 1;


            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.SubmitBook(issueId)).Returns(false);

            var contactService = new BookService(mockRepository.Object);

            // Act
            var actual = contactService.SubmitBook(issueId);

            // Assert
            Assert.False(actual.Success);
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong please try after sometime", actual.Message);
            mockRepository.Verify(r => r.SubmitBook(issueId), Times.Once);
        }
        //AddBookIssue
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBookIssue_ReturnsBookIssuedSuccessfully_WhenBookIssueisSaved()
        {
            var book = new BookIssue()
            {
                IssueDate = DateTime.Now,
                UserId = 1,
                BookId = 1,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.BookIssueExists(book.BookId, book.UserId)).Returns(false);
            mockRepository.Setup(r => r.GetIssuedBookCountForUser(book.UserId)).Returns(1);
            mockRepository.Setup(r => r.InsertBookIssue(book)).Returns(true);


            var bookService = new BookService(mockRepository.Object);

            // Act
            var actual = bookService.AddBookIssue(book);


            // Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            Assert.Equal("Book Issued successfully", actual.Message);
            mockRepository.Verify(r => r.BookIssueExists(book.BookId, book.UserId), Times.Once);
            mockRepository.Verify(r => r.GetIssuedBookCountForUser(book.UserId), Times.Once);
            mockRepository.Verify(r => r.InsertBookIssue(book), Times.Once);


        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBookIssue_ReturnsSomethingWentWrong_WhenBookIssueisNotSaved()
        {

            var book = new BookIssue()
            {
                IssueDate = DateTime.Now,
                UserId = 1,
                BookId = 1,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.BookIssueExists(book.BookId, book.UserId)).Returns(false);
            mockRepository.Setup(r => r.GetIssuedBookCountForUser(book.UserId)).Returns(1);
            mockRepository.Setup(r => r.InsertBookIssue(book)).Returns(false);

            var bookService = new BookService(mockRepository.Object);

            // Act
            var actual = bookService.AddBookIssue(book);


            // Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("Something went wrong after sometime", actual.Message);
            mockRepository.Verify(r => r.BookIssueExists(book.BookId, book.UserId), Times.Once);
            mockRepository.Verify(r => r.GetIssuedBookCountForUser(book.UserId), Times.Once);
            mockRepository.Verify(r => r.InsertBookIssue(book), Times.Once);


        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBookIssue_ReturnsAlreadyExists_WhenBooksIsAlreadyIssued()
        {
            var book = new BookIssue()
            {
                IssueDate = DateTime.Now,
                UserId = 1,
                BookId = 1,
            };

            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.BookIssueExists(book.BookId, book.UserId)).Returns(true);

            var bookService = new BookService(mockRepository.Object);

            // Act
            var actual = bookService.AddBookIssue(book);


            // Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("Book is already issued.", actual.Message);
            mockRepository.Verify(r => r.BookIssueExists(book.BookId, book.UserId), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBookIssue_UserAlreadyHasTwoBooks_WhenBookIssueisNotSaved()
        {

            var book = new BookIssue()
            {
                IssueDate = DateTime.Now,
                UserId = 1,
                BookId = 1,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.BookIssueExists(book.BookId, book.UserId)).Returns(false);
            mockRepository.Setup(r => r.GetIssuedBookCountForUser(book.UserId)).Returns(3);

            var bookService = new BookService(mockRepository.Object);

            // Act
            var actual = bookService.AddBookIssue(book);


            // Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("User already has 2 books issued. Cannot issue more.", actual.Message);
            mockRepository.Verify(r => r.BookIssueExists(book.BookId, book.UserId), Times.Once);
            mockRepository.Verify(r => r.GetIssuedBookCountForUser(book.UserId), Times.Once);
      
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetPaginatedBooks_ReturnsBooksList_WhenBooksExist()
        {

            //Arrange
            var books = new List<Book>
        {
            new Book
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            },
            new Book
            {
                   BookId = 2,
               Title = "Title 2",
                Author = "Author 2",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            }, new Book
            {
                   BookId = 3,
               Title = "Title 3",
                Author = "Author 3",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            }
        };

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.GetPaginatedBooks(1,2,null,"asc",null)).Returns(books);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.GetPaginatedBooks(1, 2, null, "asc", null);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            mockRepository.Verify(c => c.GetPaginatedBooks(1, 2, null, "asc", null), Times.Once);
            Assert.Equal(books.Count, actual.Data.Count()); // Ensure the counts are equal

            for (int i = 0; i < books.Count; i++)
            {
                Assert.Equal(books[i].BookId, actual.Data.ElementAt(i).BookId);
                Assert.Equal(books[i].Title, actual.Data.ElementAt(i).Title);
                Assert.Equal(books[i].Author, actual.Data.ElementAt(i).Author);
                Assert.Equal(books[i].TotalQuantity, actual.Data.ElementAt(i).TotalQuantity);
                Assert.Equal(books[i].AvailableQuantity, actual.Data.ElementAt(i).AvailableQuantity);
                Assert.Equal(books[i].IssuedQuantity, actual.Data.ElementAt(i).IssuedQuantity);
                Assert.Equal(books[i].PricePerBook, actual.Data.ElementAt(i).PricePerBook);
            }
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetPaginatedBooks_ReturnListWith_WhenNoBooksExist()
        {
            //Arrange
            var mockRepository = new Mock<IBookRepository>();
            var target = new BookService(mockRepository.Object);
            //Act
            var actual = target.GetPaginatedBooks(1, 2, null, "asc", null);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("No records found", actual.Message);
            Assert.False(actual.Success);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetBookById_ReturnList_WhenNoBooksExist()
        {
            //Arrange
            var mockRepository = new Mock<IBookRepository>();
            var target = new BookService(mockRepository.Object);
            //Act
            var actual = target.GetBookById(1);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong,try after sometime", actual.Message);
            Assert.False(actual.Success);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetBookById_ReturnsBooksList_WhenBooksExist()
        {

            //Arrange
            int bookId = 1;
            var books = 
            new Book
            {
                BookId = bookId,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            
        };

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.GetBookById(bookId)).Returns(books);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.GetBookById(bookId);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            mockRepository.Verify(c => c.GetBookById(bookId), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetTotalCount_ReturnsCount()
        {

            //Arrange
            var books = new List<Book>
        {
            new Book
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            },
            new Book
            {
                   BookId = 2,
               Title = "Title 2",
                Author = "Author 2",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            }
        };

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.TotalBooks(null)).Returns(books.Count());
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.TotalBooks(null);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            mockRepository.Verify(c => c.TotalBooks(null), Times.Once);
            Assert.Equal(books.Count, actual.Data);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void GetTotalCountwithSearch_ReturnsCount()
        {

            //Arrange
            var search = "Title 1";
            var books = new List<Book>
        {
            new Book
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            },
            new Book
            {
                   BookId = 2,
               Title = "Title 2",
                Author = "Author 2",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            }
        };

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.TotalBooks(search)).Returns(books.Count());
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.TotalBooks(search);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            mockRepository.Verify(c => c.TotalBooks(search), Times.Once);
            Assert.Equal(books.Count, actual.Data);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBook_ReturnsAlreadyExist()
        {
            //Assert
            var book = new AddBookDto
            {
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            };
            var response = new ServiceResponse<IEnumerable<string>>
            {
                Success = false,
                Message= "Book Already Exists"
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.BookExists(book.Title,book.Author)).Returns(true);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.AddBook(book);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("Book Already Exists", actual.Message);
            mockRepository.Verify(c => c.BookExists(book.Title, book.Author), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBook_ReturnsSuccessMessage()
        {
            //Assert
            var book = new AddBookDto
            {
               
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
                
            };
            var books = new Book
            {
                BookId = 1,
                Title = book.Title,
                Author = book.Author,
                PricePerBook = book.PricePerBook,
                IssuedQuantity = book.IssuedQuantity,
                AvailableQuantity = book.AvailableQuantity,
                TotalQuantity = book.TotalQuantity,
                
            };
            var response = new ServiceResponse<IEnumerable<string>>
            {
                Success = true,
                Message = "Book Saved Successfully"
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.BookExists(books.Title, books.Author)).Returns(false);
            mockRepository.Setup(c=>c.InsertBook(It.IsAny<Book>())).Returns(true);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.AddBook(book);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            Assert.Equal("Book Saved Successfully", actual.Message);
            mockRepository.Verify(c => c.BookExists(book.Title, book.Author), Times.Once);
            mockRepository.Verify(c => c.InsertBook(It.IsAny<Book>()),Times.Once);

        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void AddBook_ReturnsFailureMessage()
        {
            //Assert
            var book = new AddBookDto
            {

                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,

            };
            var books = new Book
            {
                BookId = 1,
                Title = book.Title,
                Author = book.Author,
                PricePerBook = book.PricePerBook,
                IssuedQuantity = book.IssuedQuantity,
                AvailableQuantity = book.AvailableQuantity,
                TotalQuantity = book.TotalQuantity,

            };
            var response = new ServiceResponse<IEnumerable<string>>
            {
                Success = false,
                Message = "Something went wrong. Please try later"
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.BookExists(books.Title, books.Author)).Returns(false);
            mockRepository.Setup(c => c.InsertBook(It.IsAny<Book>())).Returns(false);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.AddBook(book);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("Something went wrong. Please try later", actual.Message);
            mockRepository.Verify(c => c.BookExists(book.Title, book.Author), Times.Once);
            mockRepository.Verify(c => c.InsertBook(It.IsAny<Book>()), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void ModifyBook_ReturnsAlreadyExist()
        {
            //Assert
            var book = new BookDto
            {
                BookId=1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            };
            var response = new ServiceResponse<IEnumerable<string>>
            {
                Success = false,
                Message = "Book already exists."
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.BookExists(book.BookId,book.Title, book.Author)).Returns(true);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.ModifyBook(book);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("Book already exists.", actual.Message);
            mockRepository.Verify(c => c.BookExists(book.BookId,book.Title, book.Author), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void ModifyBook_ReturnsSuccessMessage()
        {
            //Assert
            var book = new Book
            {
                BookId=1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,

            };
            var books = new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                PricePerBook = book.PricePerBook,
                IssuedQuantity = book.IssuedQuantity,
                AvailableQuantity = book.AvailableQuantity,
                TotalQuantity = book.TotalQuantity,

            };
            var response = new ServiceResponse<IEnumerable<string>>
            {
                Success = true,
                Message = "Book updated successfully."
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.BookExists(books.BookId,books.Title, books.Author)).Returns(false);
            mockRepository.Setup(c => c.GetBookById(books.BookId)).Returns(book);
            mockRepository.Setup(c => c.UpdateBook(It.IsAny<Book>())).Returns(true);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.ModifyBook(books);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            Assert.Equal("Book updated successfully.", actual.Message);
            mockRepository.Verify(c => c.BookExists(books.BookId,book.Title, book.Author), Times.Once);
            mockRepository.Verify(c => c.UpdateBook(It.IsAny<Book>()), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void ModifyBook_ReturnsFailureMessage()
        {
            //Assert
            var book = new Book
            {
                BookId=1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,

            };
            var books = new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                PricePerBook = book.PricePerBook,
                IssuedQuantity = book.IssuedQuantity,
                AvailableQuantity = book.AvailableQuantity,
                TotalQuantity = book.TotalQuantity,

            };
            var response = new ServiceResponse<IEnumerable<string>>
            {
                Success = false,
                Message = "Something went wrong,try after sometime"
            };
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(c => c.BookExists(books.BookId,books.Title, books.Author)).Returns(false);
            mockRepository.Setup(c => c.GetBookById(books.BookId)).Returns(book);
            mockRepository.Setup(c => c.UpdateBook(It.IsAny<Book>())).Returns(false);
            var target = new BookService(mockRepository.Object);

            //Act
            var actual = target.ModifyBook(books);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal("Something went wrong,try after sometime", actual.Message);
            mockRepository.Verify(c => c.BookExists(books.BookId,book.Title, book.Author), Times.Once);
            mockRepository.Verify(c => c.UpdateBook(It.IsAny<Book>()), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void RemoveBook_ReturnsSuccessMessage()
        {
            //Arrange
            var bookId = 1;
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.DeleteBook(bookId)).Returns(true);

            var bookService = new BookService(mockRepository.Object);

            // Act
            var actual = bookService.RemoveBook(bookId);

            // Assert
            Assert.True(actual.Success);
            Assert.NotNull(actual);
            Assert.Equal("Book deleted successfully", actual.Message);
            mockRepository.Verify(r => r.DeleteBook(bookId), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookServiceTests")]
        public void RemoveBook_ReturnsFailureMessage()
        {
            //Arrange
            var bookId = 1;
            var mockRepository = new Mock<IBookRepository>();
            mockRepository.Setup(r => r.DeleteBook(bookId)).Returns(false);

            var bookService = new BookService(mockRepository.Object);

            // Act
            var actual = bookService.RemoveBook(bookId);

            // Assert
            Assert.False(actual.Success);
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong", actual.Message);
            mockRepository.Verify(r => r.DeleteBook(bookId), Times.Once);

        }
    }
}

