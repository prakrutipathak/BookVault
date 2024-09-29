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
    public class ReportControllerTests
    {

        //GetBookCountForUser
        [Fact]
        public void GetBookCountForUser_Returns_OkResult_For_Issue_Type()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = new DateTime(2024, 6, 1);
            string type = "issue";
            var mockService = new Mock<IReportService>();
            mockService.Setup(service => service.TotalBookCountForUser(userId, selectedDate, type))
                       .Returns(new ServiceResponse<int> { Success = true, Data = 5 });

            var controller = new ReportController(mockService.Object);

            // Act
            var result = controller.GetBookCountForUser(userId, selectedDate, type) as OkObjectResult;
            var responseData = result.Value as ServiceResponse<int>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(responseData);
            Assert.True(responseData.Success);
            Assert.Equal(5, responseData.Data);
        }



        [Fact]
        public void GetBookCountForUser_Returns_OkResult_For_Return_Type()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = new DateTime(2024, 6, 1);
            string type = "return";
            var mockService = new Mock<IReportService>();
            mockService.Setup(service => service.TotalBookCountForUser(userId, selectedDate, type))
                       .Returns(new ServiceResponse<int> { Success = true, Data = 3 });

            var controller = new ReportController(mockService.Object);

            // Act
            var result = controller.GetBookCountForUser(userId, selectedDate, type) as OkObjectResult;
            var responseData = result.Value as ServiceResponse<int>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(responseData);
            Assert.True(responseData.Success);
            Assert.Equal(3, responseData.Data);
        }



        [Fact]
        public void GetBookCountForUser_Returns_NotFoundResult_When_Unsuccessful()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = null;
            string type = "issue";
            var mockService = new Mock<IReportService>();
            mockService.Setup(service => service.TotalBookCountForUser(userId, selectedDate, type))
                       .Returns(new ServiceResponse<int> { Success = false, Message = "No records found." });

            var controller = new ReportController(mockService.Object);

            // Act
            var result = controller.GetBookCountForUser(userId, selectedDate, type) as NotFoundObjectResult;
            var responseData = result.Value as ServiceResponse<int>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.NotNull(responseData);
            Assert.False(responseData.Success);
            Assert.Equal("No records found.", responseData.Message);
        }





        //GetBookCountWithDateOrStudent
        [Fact]
        public void GetBookCountWithDateOrStudent_Returns_Ok_Result_With_Valid_Data()
        {
            // Arrange
            int userId = 1;
            DateTime? issueDate = new DateTime(2024, 6, 1);
            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.TotalBookCountWithDateOrStudent(userId, issueDate))
                       .Returns(new ServiceResponse<int> { Success = true, Data = 5 });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.GetBookCountWithDateOrStudent(userId, issueDate);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode); 
        }



        [Fact]
        public void GetBookCountWithDateOrStudent_Returns_NotFound_Result_When_Service_Fails()
        {
            // Arrange
            int? userId = null; 
            DateTime? issueDate = new DateTime(2024, 6, 1);
            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.TotalBookCountWithDateOrStudent(userId, issueDate))
                       .Returns(new ServiceResponse<int> { Success = false, Message = "No record found!" });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.GetBookCountWithDateOrStudent(userId, issueDate);
            var notFoundResult = actionResult as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode); 
        }






        //GetUserWithBook
        [Fact]
        public void GetUserWithBook_Returns_Ok_Result_With_Valid_Data()
        {
            // Arrange
            int bookId = 1;
            int page = 1;
            int pageSize = 4;
            string type = "issue";
            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.GetUserWithBook(bookId,type, page, pageSize))
                       .Returns(new ServiceResponse<IEnumerable<AdminReportUserDto>>
                       {
                           Success = true,
                           Data = new List<AdminReportUserDto>
                           {
                               new AdminReportUserDto { UserId = 1, IssueDate = new System.DateTime(2024, 6, 1), User = new User { UserId = 1, Name = "John Doe" } },
                               new AdminReportUserDto { UserId = 2, IssueDate = new System.DateTime(2024, 6, 2), User = new User { UserId = 2, Name = "Jane Smith" } }
                           }
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.GetUserWithBook(bookId,type, page, pageSize);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode); 
        }



        [Fact]
        public void GetUserWithBook_Returns_NotFound_Result_When_Service_Fails()
        {
            // Arrange
            int bookId = 1;
            int page = 1;
            int pageSize = 4;
            string type = "issue";

            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.GetUserWithBook(bookId,type, page, pageSize))
                       .Returns(new ServiceResponse<IEnumerable<AdminReportUserDto>>
                       {
                           Success = false,
                           Message = "No record found!"
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.GetUserWithBook(bookId,type, page, pageSize);
            var notFoundResult = actionResult as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode); 
        }





        //TotalUserCountWithBook
        [Fact]
        public void TotalUserCountWithBook_Returns_Ok_Result_With_Valid_Data()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";
            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.TotalUserCountWithBook(bookId, type))
                       .Returns(new ServiceResponse<int>
                       {
                           Success = true,
                           Data = 5 
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.TotalUserCountWithBook(bookId, type);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }



        [Fact]
        public void TotalUserCountWithBook_Returns_NotFound_Result_When_Service_Fails()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";

            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.TotalUserCountWithBook(bookId, type))
                       .Returns(new ServiceResponse<int>
                       {
                           Success = false,
                           Message = "No users found for the book."
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.TotalUserCountWithBook(bookId, type);
            var notFoundResult = actionResult as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode); 
        }





        //GetIssueBookWithIssueDateOrUser
        [Fact]
        public void GetIssueBookWithIssueDateOrUser_Returns_Ok_Result_With_Valid_Data()
        {
            // Arrange
            int? userId = 1;
            DateTime? issueDate = new DateTime(2024, 6, 1); 
            int page = 1;
            int pageSize = 4;
            var mockService = new Mock<IReportService>();
            var expectedData = new List<AdminReportBookDto>
            {
                new AdminReportBookDto
                {
                    Title = "Book1",
                    Author = "Author1",
                    IssueDate = DateTime.Now.AddDays(-7),
                    UserId = 1,
                    User = new User { UserId = 1, Name = "User1" }
                },
                new AdminReportBookDto
                {
                    Title = "Book2",
                    Author = "Author2",
                    IssueDate = DateTime.Now.AddDays(-14),
                    UserId = 2,
                    User = new User { UserId = 2, Name = "User2" }
                }
                
            };
            mockService.Setup(s => s.IssueBookWithIssueDateOrUser(userId, issueDate, page, pageSize))
                       .Returns(new ServiceResponse<IEnumerable<AdminReportBookDto>>
                       {
                           Success = true,
                           Data = expectedData
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.GetIssueBookWithIssueDateOrUser(userId, issueDate, page, pageSize);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }



        [Fact]
        public void GetIssueBookWithIssueDateOrUser_Returns_NotFound_Result_When_Service_Fails()
        {
            // Arrange
            int? userId = 1;
            DateTime? issueDate = new DateTime(2024, 6, 1); 
            int page = 1;
            int pageSize = 4;
            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.IssueBookWithIssueDateOrUser(userId, issueDate, page, pageSize))
                       .Returns(new ServiceResponse<IEnumerable<AdminReportBookDto>>
                       {
                           Success = false,
                           Message = "No records found."
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.GetIssueBookWithIssueDateOrUser(userId, issueDate, page, pageSize);
            var notFoundResult = actionResult as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode); 
        }





        //IssueBooksReport
        [Fact]
        public void IssueBooksReport_Returns_Ok_Result_With_Valid_Data()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = new DateTime(2024, 6, 1); 
            string type = "issue";
            int page = 1;
            int pageSize = 4;
            var mockService = new Mock<IReportService>();
            var expectedData = new List<ReportDto>
            {
                new ReportDto
                {
                    Title = "Book1",
                    Author = "Author1",
                    IssueDate = DateTime.Now.AddDays(-7),
                    ReturnDate = null,
                    IssueId = 1
                },
                new ReportDto
                {
                    Title = "Book2",
                    Author = "Author2",
                    IssueDate = DateTime.Now.AddDays(-14),
                    ReturnDate = DateTime.Now.AddDays(-2),
                    IssueId = 2
                }
               
            };
            mockService.Setup(s => s.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize))
                       .Returns(new ServiceResponse<IEnumerable<ReportDto>>
                       {
                           Success = true,
                           Data = expectedData
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.IssueBooksReport(userId, selectedDate, type, page, pageSize);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode); 
        }



        [Fact]
        public void IssueBooksReport_Returns_NotFound_Result_When_Service_Fails()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = new DateTime(2024, 6, 1); 
            string type = "issue";
            int page = 1;
            int pageSize = 4;
            var mockService = new Mock<IReportService>();
            mockService.Setup(s => s.GetIssuesAndReturnsForUser(userId, selectedDate, type, page, pageSize))
                       .Returns(new ServiceResponse<IEnumerable<ReportDto>>
                       {
                           Success = false,
                           Message = "No records found."
                       });

            var controller = new ReportController(mockService.Object); 

            // Act
            var actionResult = controller.IssueBooksReport(userId, selectedDate, type, page, pageSize);
            var notFoundResult = actionResult as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode); 
        }
    }
}
