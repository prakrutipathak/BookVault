using CivicaBookLibraryApi.Data;
using CivicaBookLibraryApi.Data.Implementation;
using CivicaBookLibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryApiTests.Repositories
{
    public class ReportRepositoryTests
    {
        //GetIssuesAndReturnsForUser
        [Fact]
        public void GetIssuesAndReturnsForUser_Returns_Issue_Records_By_Date()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1); 
            string type = "issue";
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); 
            Assert.Equal(1, result.First().IssueId); 
        }


        [Fact]
        public void GetIssuesAndReturnsForUser_Returns_Return_Records_By_Date()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1); 
            string type = "return";
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), ReturnDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 5, 20), ReturnDate = new DateTime(2024, 6, 5), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 25), ReturnDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count()); 
        }



        [Fact]
        public void GetIssuesAndReturnsForUser_Returns_Issue_Records_Without_Date()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = null;
            string type = "issue";
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); 
            Assert.Equal(1, result.First().IssueId); 
            Assert.Equal(3, result.Skip(1).First().IssueId);
        }




        [Fact]
        public void GetIssuesAndReturnsForUser_Returns_Return_Records_Without_Date()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = null;
            string type = "return";
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), ReturnDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 5, 20), ReturnDate = new DateTime(2024, 6, 5), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 25), ReturnDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); 
            Assert.Equal(3, result.First().IssueId); 
        }





        //TotalBookCountForUser
        [Fact]
        public void TotalBookCountForUser_Returns_Correct_Count_For_Issue()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1); 
            string type = "issue";

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalBookCountForUser(userId, selectedDate, type);

            // Assert
            Assert.Equal(2, result); 
        }




        [Fact]
        public void TotalBookCountForUser_Returns_Correct_Count_For_Return()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1); 
            string type = "return";

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), ReturnDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 5, 20), ReturnDate = new DateTime(2024, 6, 5), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 25), ReturnDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalBookCountForUser(userId, selectedDate, type);

            // Assert
            Assert.Equal(1, result); 
        }




        [Fact]
        public void TotalBookCountForUser_Returns_All_Records_When_SelectedDate_Null()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = null;
            string type = "issue"; 

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 5, 15), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 5, 30), UserId = 1, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalBookCountForUser(userId, selectedDate, type);

            // Assert
            Assert.Equal(3, result); 
        }



        [Fact]
        public void TotalBookCountForUser_Returns_Count_With_Return_Type()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = null;
            string type = "return";

            var mockDbContext = new Mock<IAppDbContext>(); 
            var repository = new ReportRepository(mockDbContext.Object); 
            var bookIssues = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), ReturnDate = new DateTime(2024, 6, 10), UserId = userId },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 3), ReturnDate = new DateTime(2024, 6, 12), UserId = userId },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 5), ReturnDate = null, UserId = userId },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(bookIssues.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(bookIssues.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(bookIssues.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(bookIssues.GetEnumerator());

            mockDbContext.Setup(db => db.BookIssues).Returns(mockSet.Object);

            // Act
            var count = repository.TotalBookCountForUser(userId, selectedDate, type);

            // Assert
            Assert.Equal(2, count); 
        }






        //TotalBookCountWithDateOrStudent
        [Fact]
        public void GetIssueBookWithDateOrStudent_Filters_By_UserId()
        {
            // Arrange
            int? userId = 1;
            DateTime? issueDate = null;
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 3 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 4 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssueBookWithDateOrStudent(userId, issueDate, page, pageSize);

            // Assert
            Assert.Equal(2, result.Count()); 
            Assert.Contains(result, bi => bi.UserId == 1); 
        }



        [Fact]
        public void GetIssueBookWithDateOrStudent_Filters_By_IssueDate()
         {
            // Arrange
            int? userId = null;
            DateTime? issueDate = new DateTime(2024, 6, 2); 
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 3 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 3), UserId = 3, BookId = 4 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssueBookWithDateOrStudent(userId, issueDate, page, pageSize);

            // Assert
            Assert.Equal(2, result.Count()); 
        }



        [Fact]
        public void GetIssueBookWithDateOrStudent_Returns_Empty_List_When_No_Match_Found()
        {
            // Arrange
            int? userId = 10; 
            DateTime? issueDate = new DateTime(2024, 6, 1); 
            int page = 1;
            int pageSize = 10;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 3 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 4 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetIssueBookWithDateOrStudent(userId, issueDate, page, pageSize);

            // Assert
            Assert.Empty(result); 
        }






        //TotalBookCountWithDateOrStudent
        [Fact]
        public void TotalBookCountWithDateOrStudent_Filters_By_UserId()
        {
            // Arrange
            int? userId = 1;
            DateTime? issueDate = null;

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 3 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 4 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalBookCountWithDateOrStudent(userId, issueDate);

            // Assert
            Assert.Equal(2, result); 
        }



        [Fact]
        public void TotalBookCountWithDateOrStudent_Filters_By_IssueDate()
        {
            // Arrange
            int? userId = null;
            DateTime? issueDate = new DateTime(2024, 6, 2); 

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 2), UserId = 1, BookId = 3 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 3), UserId = 3, BookId = 4 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalBookCountWithDateOrStudent(userId, issueDate);

            // Assert
            Assert.Equal(2, result); 
        }



        [Fact]
        public void TotalBookCountWithDateOrStudent_Returns_Zero_When_No_Match_Found()
        {
            // Arrange
            int? userId = 100; 
            DateTime? issueDate = new DateTime(2024, 6, 1); 

            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 2 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 3 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 4 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalBookCountWithDateOrStudent(userId, issueDate);

            // Assert
            Assert.Equal(0, result); 
        }




        [Fact]
        public void GetUserWithReturnedBook_Filters_By_BookId()
        {
            // Arrange
            int bookId = 1;
            int page = 1;
            int pageSize = 2;
            string type = "return";


            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, ReturnDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, ReturnDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 1 },
                new BookIssue { IssueId = 3, ReturnDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 4, ReturnDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetUserWithBook(bookId,type, page, pageSize);

            // Assert
            Assert.Equal(2, result.Count()); 
        }

        [Fact]
        public void GetUserWithIssuedBook_Filters_By_BookId()
        {
            // Arrange
            int bookId = 1;
            int page = 1;
            int pageSize = 2;
            string type = "issue";


            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 1 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetUserWithBook(bookId,type, page, pageSize);

            // Assert
            Assert.Equal(2, result.Count()); 
        }



        [Fact]
        public void GetUserWithBook_Returns_Empty_List_When_No_Match_Found()
        {
            // Arrange
            int bookId = 100; 
            int page = 1;
            int pageSize = 2;
            string type = "issue";


            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 1 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.GetUserWithBook(bookId,type, page, pageSize);

            // Assert
            Assert.Empty(result); 
        }






        //TotalUserCountWithBook
        [Fact]
        public void TotalUserCountWithIssueBook_Returns_Count_Of_Users()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";


            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 1 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalUserCountWithBook(bookId, type);

            // Assert
            Assert.Equal(2, result); 
        }

        [Fact]
        public void TotalUserCountWithReturnBook_Returns_Count_Of_Users()
        {
            // Arrange
            int bookId = 1;
            string type = "return";


            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, ReturnDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, ReturnDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 1 },
                new BookIssue { IssueId = 3, ReturnDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 4, ReturnDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalUserCountWithBook(bookId, type);

            // Assert
            Assert.Equal(2, result);
        }




        [Fact]
        public void TotalUserCountWithBook_Returns_Zero_When_No_Match_Found()
        {
            // Arrange
            int bookId = 100;
            string type = "issue";


            var mockData = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, IssueDate = new DateTime(2024, 6, 1), UserId = 1, BookId = 1 },
                new BookIssue { IssueId = 2, IssueDate = new DateTime(2024, 6, 2), UserId = 2, BookId = 1 },
                new BookIssue { IssueId = 3, IssueDate = new DateTime(2024, 6, 3), UserId = 1, BookId = 2 },
                new BookIssue { IssueId = 4, IssueDate = new DateTime(2024, 6, 4), UserId = 3, BookId = 3 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<BookIssue>>();
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<BookIssue>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            var mockContext = new Mock<IAppDbContext>();
            mockContext.Setup(c => c.BookIssues).Returns(mockSet.Object);

            var repository = new ReportRepository(mockContext.Object);

            // Act
            var result = repository.TotalUserCountWithBook(bookId, type);

            // Assert
            Assert.Equal(0, result); 
        }
    }
}
