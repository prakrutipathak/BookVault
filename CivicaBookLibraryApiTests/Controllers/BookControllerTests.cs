using AutoFixture;
using CivicaBookLibraryApi.Controllers;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryApiTests.Controllers
{
    public class BookControllerTests
    {
        //GetAllBooks
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetAllBooks_ReturnsOkWithBooks_WhenBooksExists()
        {
            //Arrange

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetAllBooks()).Returns(response);

            //Act
            var actual = target.GetAllBooks() as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.GetAllBooks(), Times.Once);
        }

        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetAllBooks_ReturnsNotFound_WhenNoBooksExists()
        {
            //Arrange
            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = false,
                Data = new List<BookDto>(),

            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetAllBooks()).Returns(response);

            //Act
            var actual = target.GetAllBooks() as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.GetAllBooks(), Times.Once);
        }
        //AddIssueBook
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void AddBookIssue_ReturnsBadRequest_WhenModelIsInValid()
        {
            var fixture = new Fixture();
            var addBookIssueDto = fixture.Create<BookIssueDto>();
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            target.ModelState.AddModelError("UserId", "UserId is required");
            //Act

            var actual = target.AddBookIssue(addBookIssueDto) as BadRequestResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.False(target.ModelState.IsValid);
        }

        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void AddBookIssue_ReturnsOk_WhenBookIssueSuccessfully()
        {
            var fixture = new Fixture();
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var addBookIssueDto = fixture.Create<BookIssueDto>();
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.AddBookIssue(It.IsAny<BookIssue>())).Returns(response);

            //Act

            var actual = target.AddBookIssue(addBookIssueDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.AddBookIssue(It.IsAny<BookIssue>()), Times.Once);

        }

        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void AddBookIssue_ReturnsBadRequest_WhenBookIssueIsNotAdded()
        {
            var fixture = new Fixture();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var addBookIssueDto = fixture.Create<BookIssueDto>();
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.AddBookIssue(It.IsAny<BookIssue>())).Returns(response);


            //Act

            var actual = target.AddBookIssue(addBookIssueDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.AddBookIssue(It.IsAny<BookIssue>()), Times.Once);

        }
        //SubmitBook
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void SubmitBook_ReturnsOkResponse_WhenReturnBookSuccessfully()
        {

            var issueId = 1;
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.SubmitBook(issueId)).Returns(response);

            //Act

            var actual = target.SubmitBook(issueId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.SubmitBook(issueId), Times.Once);
        }

        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void SubmitBook_ReturnsBadRequest_WhenNoReturnBook()
        {

            var issueId = 1;
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.SubmitBook(issueId)).Returns(response);

            //Act

            var actual = target.SubmitBook(issueId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.SubmitBook(issueId), Times.Once);
        }

        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void SubmitBook_ReturnsBadRequest_WhenBookIsLessThanZero()
        {

            var issueId = 0;

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);

            //Act

            var actual = target.SubmitBook(issueId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal("Please enter proper data", actual.Value);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetAllBooksByPagination_ReturnsOkWithBooks_WhenBooksExists()
        {
            //Arrange

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = true,

            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetPaginatedBooks(1,2,null,"asc",null)).Returns(response);

            //Act
            var actual = target.GetPaginatedBooks(null, null, 1, 2, "asc") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.GetPaginatedBooks(1, 2, null, "asc", null), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetAllBooksByPagination_ReturnsNotFound_WhenNoBooksExists()
        {
            //Arrange

            var response = new ServiceResponse<IEnumerable<BookDto>>
            {
                Success = false,
                Data = new List<BookDto>(),
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetPaginatedBooks(1, 2, null, "asc", null)).Returns(response);

            //Act
            var actual = target.GetPaginatedBooks(null, null, 1, 2, "asc") as OkObjectResult;

            //Assert
            Assert.Null(actual);
            mockBookService.Verify(c => c.GetPaginatedBooks(1, 2, null, "asc", null), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetBookById_ReturnsNotFound()
        {
            var bookId = 1;
            var book = new BookDto
            {

                BookId = bookId,
                Title = "title 1"
            };

            var response = new ServiceResponse<BookDto>
            {
                Success = false,
                Data = new BookDto
                {
                    BookId = bookId,
                    Title = book.Title
                }
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetBookById(bookId)).Returns(response);

            //Act
            var actual = target.GetBookById(bookId) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.GetBookById(bookId), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetBookById_OkResponse()
        {
            var bookId = 1;
            var book = new BookDto
            {

                BookId = bookId,
                Title = "title 1"
            };

            var response = new ServiceResponse<BookDto>
            {
                Success = true,
                Data = new BookDto
                {
                    BookId = bookId,
                    Title = book.Title
                }
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetBookById(bookId)).Returns(response);

            //Act
            var actual = target.GetBookById(bookId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.GetBookById(bookId), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetBookById_ErrorMessage_WhenIdislessThanZero()
        {
            var bookId = -11;
            var book = new BookDto
            {

                BookId = bookId,
                Title = "title 1"
            };

            var response = new ServiceResponse<BookDto>
            {
                Success = false,
                Data=null,
                Message= "Please enter valid data."
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.GetBookById(bookId)).Returns(response);

            //Act
            var actual = target.GetBookById(bookId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response.Message, actual.Value);
            mockBookService.Verify(c => c.GetBookById(bookId), Times.Never);
        }

        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void AddBook_ReturnsBadRequest_WhenBookIsNotAdded()
        {

            var addBookDto = new AddBookDto
            {
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            };
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.AddBook(It.IsAny<AddBookDto>())).Returns(response);

            //Act

            var actual = target.AddBook(addBookDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.AddBook(It.IsAny<AddBookDto>()), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void AddBook_ReturnsOkResponse_WhenBookIsAdded()
        {

            var addBookDto = new AddBookDto
            {
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            };
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.AddBook(It.IsAny<AddBookDto>())).Returns(response);

            //Act

            var actual = target.AddBook(addBookDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.AddBook(It.IsAny<AddBookDto>()), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void ModifyBook_ReturnsBadRequest_WhenBookIsNotAdded()
        {

            var bookDto = new BookDto
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            };
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.ModifyBook(It.IsAny<BookDto>())).Returns(response);

            //Act

            var actual = target.UpdateBook(bookDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.ModifyBook(It.IsAny<BookDto>()), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void ModifyBook_ReturnsOkResponse_WhenBookIsAdded()
        {

            var bookDto = new BookDto
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                PricePerBook = 10.2m,
                IssuedQuantity = 0,
                AvailableQuantity = 10,
                TotalQuantity = 10,
            };
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.ModifyBook(It.IsAny<BookDto>())).Returns(response);

            //Act

            var actual = target.UpdateBook(bookDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.ModifyBook(It.IsAny<BookDto>()), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void RemoveBook_ReturnsNotFound()
        {
            var bookId = 1;
            var book = new BookDto
            {

                BookId = bookId,
                Title = "title 1"
            };

            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.RemoveBook(bookId)).Returns(response);

            //Act
            var actual = target.DeleteBook(bookId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.RemoveBook(bookId), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void RemoveBook_OkResponse()
        {
            var bookId = 1;
            var book = new BookDto
            {

                BookId = bookId,
                Title = "title 1"
            };

            var response = new ServiceResponse<string>
            {
                Success = true,

            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.RemoveBook(bookId)).Returns(response);

            //Act
            var actual = target.DeleteBook(bookId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.RemoveBook(bookId), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void RemoveBook_ErrorMessage_WhenIdislessThanZero()
        {
            var bookId = -11;
            var book = new BookDto
            {

                BookId = bookId,
                Title = "title 1"
            };

            var response = new ServiceResponse<string>
            {
                Success = false,
                Message = "Please enter valid data."
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.RemoveBook(bookId)).Returns(response);

            //Act
            var actual = target.DeleteBook(bookId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response.Message, actual.Value);
            mockBookService.Verify(c => c.RemoveBook(bookId), Times.Never);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetBookCounts_ReturnsOkResponse()
        {
            //Arrange

            var response = new ServiceResponse<int>
            {
                Success = true,
                Data=2
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.TotalBooks(null)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfContacts(null) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.TotalBooks(null), Times.Once);
        }
        [Fact]
        [Trait("Books", "BookControllerTests")]
        public void GetBookCounts_ReturnsNotFoundResponse()
        {
            //Arrange

            var response = new ServiceResponse<int>
            {
                Success = false,
            };

            var mockBookService = new Mock<IBookService>();
            var target = new BookController(mockBookService.Object);
            mockBookService.Setup(c => c.TotalBooks(null)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfContacts(null) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockBookService.Verify(c => c.TotalBooks(null), Times.Once);
        }
    }
}
