using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using CivicaBookLibraryApi.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryApiTests.Services
{
    public class ReportServiceTests
    {
        ////GetIssuesAndReturnsForUser
        [Fact]
        public void GetIssuesAndReturnsForUser_Returns_Valid_ServiceResponse_With_Data()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1);
            string type = "issue";
            int page = 1;
            int pageSize = 4;

            var mockReportRepository = new Mock<IReportRepository>();
            var mockBookIssues = new List<BookIssue>
            {
                new BookIssue { IssueId = 1, Book = new Book { Title = "Book 1", Author = "Author 1" }, IssueDate = DateTime.Now.AddDays(-7), ReturnDate = null },
                new BookIssue { IssueId = 2, Book = new Book { Title = "Book 2", Author = "Author 2" }, IssueDate = DateTime.Now.AddDays(-14), ReturnDate = DateTime.Now.AddDays(-2) }
            };
            mockReportRepository.Setup(x => x.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize))
                               .Returns(mockBookIssues);

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            
            var reportDtos = result.Data.ToList(); 
            Assert.Equal(2, reportDtos.Count);

            Assert.Equal(1, reportDtos[0].IssueId);
            Assert.Equal("Book 1", reportDtos[0].Title);
            Assert.Equal("Author 1", reportDtos[0].Author);
            Assert.Equal(mockBookIssues[0].IssueDate, reportDtos[0].IssueDate);
            Assert.Null(reportDtos[0].ReturnDate);

            Assert.Equal(2, reportDtos[1].IssueId);
            Assert.Equal("Book 2", reportDtos[1].Title);
            Assert.Equal("Author 2", reportDtos[1].Author);
            Assert.Equal(mockBookIssues[1].IssueDate, reportDtos[1].IssueDate);
            Assert.Equal(mockBookIssues[1].ReturnDate, reportDtos[1].ReturnDate); 
        }


        [Fact]
        public void GetIssuesAndReturnsForUser_Returns_Failure_ServiceResponse_When_No_Data_Found()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1);
            string type = "issue";
            int page = 1;
            int pageSize = 4;

            var mockReportRepository = new Mock<IReportRepository>();
            mockReportRepository.Setup(x => x.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize))
                               .Returns(() => null); 

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("No record found !.", result.Message);
            Assert.Null(result.Data); 
        }



        //TotalBookCountForUser
        [Fact]
        public void TotalBookCountForUser_Returns_Valid_ServiceResponse()
        {
            // Arrange
            int userId = 1;
            DateTime selectedDate = new DateTime(2024, 6, 1);
            string type = "issue";

            var mockReportRepository = new Mock<IReportRepository>();
            mockReportRepository.Setup(x => x.TotalBookCountForUser(userId, selectedDate, type))
                               .Returns(10); 

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.TotalBookCountForUser(userId, selectedDate, type);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(10, result.Data); 
        }




        ////IssueBookWithIssueDateOrUser
        [Fact]
        public void IssueBookWithIssueDateOrUser_Returns_Data_When_Records_Exist()
        {
            // Arrange
            int? userId = 1;
            DateTime? selectedDate = new DateTime(2024, 6, 1); 
            int page = 1;
            int pageSize = 4;

            var mockRepository = new Mock<IReportRepository>();
            var expectedBookIssues = new List<BookIssue>
            {
                new BookIssue
                {
                    IssueId = 1,
                    IssueDate = DateTime.Now.AddDays(-7),
                    Book = new Book { Title = "Book1", Author = "Author1" },
                    UserId = 1,
                    User = new User { UserId = 1, Name = "User1" }
                },
                new BookIssue
                {
                    IssueId = 2,
                    IssueDate = DateTime.Now.AddDays(-14),
                    Book = new Book { Title = "Book2", Author = "Author2" },
                    UserId = 2,
                    User = new User { UserId = 2, Name = "User2" }
                }
               
            };

            mockRepository.Setup(r => r.GetIssueBookWithDateOrStudent(userId, selectedDate, page, pageSize))
                          .Returns(expectedBookIssues);

            var service = new ReportService(mockRepository.Object); 

            // Act
            var response = service.IssueBookWithIssueDateOrUser(userId, selectedDate, page, pageSize);

            // Assert
            Assert.True(response.Success);
            Assert.NotEmpty(response.Data);
            Assert.Equal(expectedBookIssues.Count, response.Data.Count());
        }



        [Fact]
        public void IssueBookWithIssueDateOrUser_Returns_NotFound_When_No_Records_Exist()
        {
            // Arrange
            int? userId = 1;
            DateTime? selectedDate = new DateTime(2024, 6, 1); 
            int page = 1;
            int pageSize = 4;

            var mockRepository = new Mock<IReportRepository>();
            mockRepository.Setup(r => r.GetIssueBookWithDateOrStudent(userId, selectedDate, page, pageSize))
                          .Returns(new List<BookIssue>());

            var service = new ReportService(mockRepository.Object); 

            // Act
            var response = service.IssueBookWithIssueDateOrUser(userId, selectedDate, page, pageSize);

            // Assert
            Assert.False(response.Success);
            Assert.Null(response.Data);
            Assert.Equal("No record found !.", response.Message);
        }





        //TotalBookCountWithDateOrStudent
        [Fact]
        public void TotalBookCountWithDateOrStudent_Returns_Valid_Response()
        {
            // Arrange
            int? userId = 1;
            DateTime issuedate = new DateTime(2024, 6, 1);

            var mockReportRepository = new Mock<IReportRepository>();
            mockReportRepository.Setup(x => x.TotalBookCountWithDateOrStudent(userId, issuedate))
                               .Returns(10); 

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.TotalBookCountWithDateOrStudent(userId, issuedate);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(10, result.Data); 
        }



        [Fact]
        public void TotalBookCountWithDateOrStudent_Returns_Zero_When_No_Data_Found()
        {
            // Arrange
            int? userId = 1;
            DateTime issuedate = new DateTime(2024, 6, 1);

            var mockReportRepository = new Mock<IReportRepository>();
            mockReportRepository.Setup(x => x.TotalBookCountWithDateOrStudent(userId, issuedate))
                               .Returns(0); 

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.TotalBookCountWithDateOrStudent(userId, issuedate);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(0, result.Data); 
        }



        //GetUserWithBook
        [Fact]
        public void GetUserWithBook_Returns_Data_When_Records_Exist()
        {
            // Arrange
            int bookId = 1;
            int page = 1;
            int pageSize = 4;
            string type = "issue";



            var mockRepository = new Mock<IReportRepository>();
            var expectedUsers = new List<BookIssue>
            {
                new BookIssue
                {
                    IssueId = 1,
                    IssueDate = DateTime.Now.AddDays(-7),
                    UserId = 1,
                    User = new User { UserId = 1, Name = "User1" }
                },
                new BookIssue
                {
                    IssueId = 2,
                    IssueDate = DateTime.Now.AddDays(-14),
                    UserId = 2,
                    User = new User { UserId = 2, Name = "User2" }
                }
                
            };

            mockRepository.Setup(r => r.GetUserWithBook(bookId,type, page, pageSize))
                          .Returns(expectedUsers);

            var service = new ReportService(mockRepository.Object); 

            // Act
            var response = service.GetUserWithBook(bookId,type, page, pageSize);

            // Assert
            Assert.True(response.Success);
            Assert.NotEmpty(response.Data);
            Assert.Equal(expectedUsers.Count, response.Data.Count());
        }



        [Fact]
        public void GetUserWithBook_Returns_NotFound_When_No_Records_Exist()
        {
            // Arrange
            int bookId = 1;
            int page = 1;
            int pageSize = 4;
            string type = "issue";


            var mockRepository = new Mock<IReportRepository>();
            mockRepository.Setup(r => r.GetUserWithBook(bookId,type, page, pageSize))
                          .Returns(new List<BookIssue>());

            var service = new ReportService(mockRepository.Object); 

            // Act
            var response = service.GetUserWithBook(bookId,type, page, pageSize);

            // Assert
            Assert.False(response.Success);
            Assert.Null(response.Data);
            Assert.Equal("No record found !.", response.Message);
        }





        //TotalUserCountWithBook
        [Fact]
        public void TotalUserCountWithBook_Returns_Valid_Response()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";

            var expectedCount = 5; 

            var mockReportRepository = new Mock<IReportRepository>();
            mockReportRepository.Setup(x => x.TotalUserCountWithBook(bookId, type))
                               .Returns(expectedCount);

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.TotalUserCountWithBook(bookId,type);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data); 
        }



        [Fact]
        public void TotalUserCountWithBook_Returns_Zero_When_No_Data_Found()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";


            var mockReportRepository = new Mock<IReportRepository>();
            mockReportRepository.Setup(x => x.TotalUserCountWithBook(bookId, type))
                               .Returns(0); 

            var reportService = new ReportService(mockReportRepository.Object);

            // Act
            var result = reportService.TotalUserCountWithBook(bookId,type);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(0, result.Data); 
        }

    }
}
