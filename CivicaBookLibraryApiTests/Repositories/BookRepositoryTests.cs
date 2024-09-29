using CivicaBookLibraryApi.Data;
using CivicaBookLibraryApi.Data.Implementation;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace CivicaBookLibraryApiTests.Repositories
{
    public class BookRepositoryTests
    {
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetAllBooks_ReturnsBooks_WhenBooksExists()
        {
            //Arrange
            var books = new List<Book>
            {
                new Book{  BookId = 1,
                Title = "T1"},
                 new Book{  BookId = 2,
                Title = "T2"},
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.GetEnumerator()).Returns(books.GetEnumerator());
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.Setup(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAbContext.Object);
            //Act
            var actual = target.GetAllBooks();
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(books.Count(), actual.Count());
            mockAbContext.Verify(c => c.Books, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.GetEnumerator(), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetAllBooks_ReturnsBook_WhenBooksNotExists()
        {
            //Arrange
            var books = new List<Book>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.GetEnumerator()).Returns(books.GetEnumerator());
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.Setup(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAbContext.Object);
            //Act
            var actual = target.GetAllBooks();
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(books.Count(), actual.Count());
            mockAbContext.Verify(c => c.Books, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.GetEnumerator(), Times.Once);

        }
        [Fact]
        public void InsertBookIssue_ReturnsTrue()
        {
            //Arrange
            var bookIssue = new BookIssue
            {
                UserId = 1,
                IssueId = 1,
                BookId = 1,
            };
            var book = new Book
            {
                BookId = 1,
                Title = "T1"
            };

            var mockDbSet = new Mock<DbSet<BookIssue>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.BookIssues).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            mockAppDbContext.Setup(c => c.Books.Find(bookIssue.BookId)).Returns(book);
            var target = new BookRepository(mockAppDbContext.Object);
           

            //Act
            var actual = target.InsertBookIssue(bookIssue);

            //Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Add(bookIssue), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Exactly(2));
            mockAppDbContext.Verify(c => c.Books.Find(bookIssue.BookId),Times.Once);
        }

        [Fact]
        public void InsertBookIssue_ReturnsFalse()
        {
            //Arrange
            BookIssue bookIssue = null;
            var mockAbContext = new Mock<IAppDbContext>();
            var target = new BookRepository(mockAbContext.Object);

            //Act
            var actual = target.InsertBookIssue(bookIssue);
            //Assert
            Assert.False(actual);
        }
        [Fact]
        public void SubmitBook_ReturnsFalse()
        {
            //Arrange
            var id = 1;
            var bookIssues = new List<BookIssue>().AsQueryable();
            var mockDbSet = new Mock<DbSet<BookIssue>>();
            mockDbSet.As<IQueryable<BookIssue>>().Setup(c => c.Provider).Returns(bookIssues.Provider);
            mockDbSet.As<IQueryable<BookIssue>>().Setup(c => c.Expression).Returns(bookIssues.Expression);
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.BookIssues).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAbContext.Object);

            //Act
            var actual = target.SubmitBook(id);
            //Assert
            Assert.False(actual);
            mockDbSet.As<IQueryable<BookIssue>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<BookIssue>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.BookIssues, Times.Once);
        }
        [Fact]
        public void SubmitBook_ReturnsTrue()
        {
            //Arrange
            var bookId = 1;
            var id = 1;
            var bookIssue = new List<BookIssue>
            { new BookIssue
            {
                UserId = 1,
                IssueId = 1,
                BookId = 1,
            },
             new BookIssue
            {
                UserId = 2,
                IssueId = 2,
                BookId = 2,
            } }.AsQueryable();
            var book = new Book
            {
                BookId = 1,
                Title = "T1"
            };

            var mockDbSet = new Mock<DbSet<BookIssue>>();
            mockDbSet.As<IQueryable<BookIssue>>().Setup(c => c.Provider).Returns(bookIssue.Provider);
            mockDbSet.As<IQueryable<BookIssue>>().Setup(c => c.Expression).Returns(bookIssue.Expression);
           
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.BookIssues).Returns(mockDbSet.Object);
            mockAbContext.Setup(c => c.SaveChanges()).Returns(1);
            mockAbContext.Setup(c => c.Books.Find(bookId)).Returns(book);
            var target = new BookRepository(mockAbContext.Object);

            //Act
            var actual = target.SubmitBook(id);
            //Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<BookIssue>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<BookIssue>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.BookIssues, Times.Once);
            mockAbContext.Verify(c => c.SaveChanges(), Times.Exactly(2));
            mockAbContext.Verify(c => c.Books.Find(bookId), Times.Once);
        }
        [Fact]
        public void BookIssueExists_Should_Return_True_If_BookIssue_Exists()
        {
            // Arrange
            var bookIssuesData = new List<BookIssue>
        {
            new BookIssue { BookId = 1, UserId = 1, ReturnDate = null },
            new BookIssue { BookId = 2, UserId = 1, ReturnDate = null },
            new BookIssue { BookId = 1, UserId = 2, ReturnDate = null },
            new BookIssue { BookId = 2, UserId = 2, ReturnDate = DateTime.Now.AddDays(-7) } 
        }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssuesData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssuesData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssuesData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssuesData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var target = new BookRepository(mockContext.Object);

            // Act
            var actual = target.BookIssueExists(1, 1);

            // Assert
            Assert.True(actual);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssuesData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssuesData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssuesData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssuesData.GetEnumerator());

        }

        [Fact]
        public void BookIssueExists_Should_Return_False_If_BookIssue_Does_Not_Exist()
        {
            // Arrange
            var bookIssuesData = new List<BookIssue>
        {
            new BookIssue { BookId = 1, UserId = 1, ReturnDate = null },
            new BookIssue { BookId = 2, UserId = 1, ReturnDate = null },
            new BookIssue { BookId = 1, UserId = 2, ReturnDate = null },
            new BookIssue { BookId = 2, UserId = 2, ReturnDate = DateTime.Now.AddDays(-7) } // Issue returned
        }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssuesData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssuesData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssuesData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssuesData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var target = new BookRepository(mockContext.Object);

            // Act
            var actual = target.BookIssueExists(3, 1); 

            // Assert
            Assert.False(actual);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssuesData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssuesData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssuesData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssuesData.GetEnumerator());
        }
        [Fact]
        public void GetIssuedBookCountForUser_Should_Return_True_If_BookIssue_Exists()
        {
            // Arrange
            var bookIssuesData = new List<BookIssue>
        {
            new BookIssue { BookId = 1, UserId = 1, ReturnDate = null },
            new BookIssue { BookId = 2, UserId = 1, ReturnDate = null },
            new BookIssue { BookId = 1, UserId = 2, ReturnDate = null },
            new BookIssue { BookId = 2, UserId = 2, ReturnDate = DateTime.Now.AddDays(-7) }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssuesData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssuesData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssuesData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssuesData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var target = new BookRepository(mockContext.Object);

            // Act
            var actual = target.GetIssuedBookCountForUser(1);

            // Assert
            Assert.Equal(2,2);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssuesData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssuesData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssuesData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssuesData.GetEnumerator());

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetBookById_ReturnsBook_WhenBookExists()
        {
            //Arrange
            var id = 1;
            var books = new List<Book>
            {
                new Book{  BookId = 1,
                Title = "T1"},
                 new Book{  BookId = 2,
                Title = "T2"},
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAbContext.SetupGet(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAbContext.Object);
            //Act
            var actual = target.GetBookById(id);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetBookById_ReturnsBook_WhenBookDoesNotExists()
        {
            //Arrange
            var id = 1;
            var books = new List<Book>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAbContext.SetupGet(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAbContext.Object);
            //Act
            var actual = target.GetBookById(id);
            //Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsCorrectPageandSize()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author Z", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var result = target.GetPaginatedBooks(1, 2, null, "asc", "title");

            // Assert
            Assert.Equal(2, result.Count()); 
            Assert.Equal("Book A", result.ElementAt(0).Title);
            Assert.Equal("Book B", result.ElementAt(1).Title);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_withSearch()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author Z", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, "Book A", "asc", "title");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(1, actual.Count()); 
            Assert.Equal("Book A", actual.ElementAt(0).Title);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByAuthor()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null, "asc", "author");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Author A", actual.ElementAt(0).Author);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByAuthorDesc()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null, "desc", "author");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Author Y", actual.ElementAt(0).Author);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByTitle()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null, "asc", "title");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Book A", actual.ElementAt(0).Title);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByTitleDesc()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null, "desc", "title");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Book C", actual.ElementAt(0).Title);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByPrice()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 19 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null, "asc", "price");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal(19, actual.ElementAt(0).PricePerBook);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByTitlePrice()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null, "desc", "price");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal(30, actual.ElementAt(0).PricePerBook);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void GetPaginatedBooks_ReturnsBooks_SortByIsNull()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book C", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book A", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetPaginatedBooks(1, 2, null,null,null);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Book A", actual.ElementAt(0).Title);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void TotalBooks_ReturnsBookCount_WhenSerchIsNull()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book C", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book A", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalBooks(null);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(3, actual);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void TotalBooks_ReturnsBookCount_UsingSearch()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book C", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = "Book B", Author = "Author Y", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book A", Author = "Author A", PricePerBook = 30 }
        }.AsQueryable();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalBooks("Book A");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(1, actual);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Book>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void InsertBook_ReturnsTrue_WhenSuccessfull()
        {
            // Arrange
            var book = new Book
            {
                BookId = 1,
                Title = "Book C",
                Author = "Author X",
                PricePerBook = 20
            };
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(db => db.SaveChanges()).Returns(1);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.InsertBook(book);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void InsertBook_ReturnsFalse_WhenInsertFails()
        {
            // Arrange
            var book = new Book();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.InsertBook(null);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void UpdateBook_ReturnsTrue_WhenSuccessfull()
        {
            // Arrange
            var book = new Book
            {
                BookId = 1,
                Title = "Book C",
                Author = "Author X",
                PricePerBook = 20
            };
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockAppDbContext.SetupGet(db => db.Books).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(db => db.SaveChanges()).Returns(1);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.UpdateBook(book);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);

        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void UpdateBook_ReturnsFalse_WhenInsertFails()
        {
            // Arrange
            var book = new Book();
            var mockAppDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Book>>();
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.UpdateBook(null);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void DeleteBook_ReturnsTrue_WhenSuccessfull()
        {
            // Arrange
            var bookId = 1;
            var books = new Book { BookId = bookId, Title = "Book A", Author = "Author X", PricePerBook = 20 };
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Books.Find(bookId)).Returns(books);
            mockAppDbContext.Setup(db => db.SaveChanges()).Returns(1);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.DeleteBook(bookId);

            //Assert
            Assert.NotNull(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Exactly(2));
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
            mockAppDbContext.Verify(c => c.Books.Find(bookId), Times.Once);


        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void DeleteBook_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            var bookId1 = 1;
            var bookId2 = 2;
            var books = new Book { BookId = bookId2, Title = "Book A", Author = "Author X", PricePerBook = 20 };
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Books.Find(bookId2)).Returns(books);
            var mockDbSet = new Mock<DbSet<Book>>();
            var target = new BookRepository(mockAppDbContext.Object);

            //Act
            var actual = target.DeleteBook(bookId1);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void BookExists_ReturnsTrue()
        {
            // Arrange
            var bookId = 1;
            var title = "Book A";
            var author = "Author X";
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = 2, Title = title, Author = author, PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 19 }
        }.AsQueryable(); 
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act   
            var actual = target.BookExists(title,author);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void BookExists_ReturnsFalse()
        {
            // Arrange
            var bookId = 1;
            var title = "Book A";
            var author = "Author X";
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book", Author = "Author", PricePerBook = 20 },
            new Book { BookId = 2, Title = "title", Author = "author", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 19 }
        }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act   
            var actual = target.BookExists(title, author);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void BookExistsOverLoad_ReturnsTrue()
        {
            // Arrange
            var bookId = 1;
            var title = "Book A";
            var author = "Author X";
            var books = new List<Book>
        {
            new Book { BookId = 6, Title = "Book A", Author = "Author X", PricePerBook = 20 },
            new Book { BookId = bookId, Title = title, Author = author, PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 19 }
        }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act   
            var actual = target.BookExists(bookId,title, author);

            //Assert
            Assert.NotNull(actual);
            Assert.True(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
        }
        [Fact]
        [Trait("Books", "BookRepositoryTests")]
        public void BookExistsOverLoad_ReturnsFalse()
        {
            // Arrange
            var bookId = 1;
            var title = "Book A";
            var author = "Author X";
            var books = new List<Book>
        {
            new Book { BookId = 4, Title = "Book", Author = "Author", PricePerBook = 20 },
            new Book { BookId = 2, Title = "title", Author = "author", PricePerBook = 25 },
            new Book { BookId = 3, Title = "Book C", Author = "Author A", PricePerBook = 19 }
        }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Book>>();
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Provider).Returns(books.Provider);
            mockDbSet.As<IQueryable<Book>>().Setup(c => c.Expression).Returns(books.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Books).Returns(mockDbSet.Object);
            var target = new BookRepository(mockAppDbContext.Object);

            //Act   
            var actual = target.BookExists(bookId,title, author);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual);
            mockAppDbContext.VerifyGet(c => c.Books, Times.Once);
        }



    }
}

