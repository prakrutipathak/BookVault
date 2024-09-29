using CivicaBookLibraryMVC.Controllers;
using CivicaBookLibraryMVC.Implementation;
using CivicaBookLibraryMVC.Infrastructure;
using CivicaBookLibraryMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryMVCTests.Controllers
{
    public class ReportControllerTests
    {

        //ShowUserbookReport 
        [Fact]
        public void ShowUserbookReport_NoSelectedDate_ReturnsCorrectView()
        {
            // Arrange
            var selectedDate = (DateTime?)null;
            var userId = 1;
            var type = "issue";
            var page = 1;
            var pageSize = 2;
            var apiGetBooksUrl = $"fakeEndPointReport/UserBookReport?userId={userId}&type={type}&page={page}&pageSize={pageSize}";
            var apiGetBooksCountUrl = $"fakeEndPointReport/GetBookCountForUser?userId={userId}&type={type}";

            var expectedUserBookReportData = new List<UserBookReportViewModel>
            {
                new UserBookReportViewModel { /* Initialize with mock data */ },
                new UserBookReportViewModel { /* Initialize with mock data */ }
            };
            var expectedResponseForGet = new ServiceResponse<IEnumerable<UserBookReportViewModel>>
            {
                Success = true,
                Data = expectedUserBookReportData
            };


            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();


            var cookies = new Dictionary<string, string>
            {
                { "userId", userId.ToString() }
            };


            mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1");

            var mockCountOfBooks = new ServiceResponse<int>
            {
                Success = true,
                Data = 5
            };


            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockCountOfBooks);


            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserBookReportViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponseForGet);

            var target = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = target.ShowUserbookReport(selectedDate, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUserBookReportData, result.Model);
            Assert.Equal(page, result.ViewData["CurrentPage"]);
            Assert.Equal(pageSize, result.ViewData["PageSize"]);
            Assert.Equal(type, result.ViewData["Type"]);
            Assert.Equal(userId, result.ViewData["UserId"]);
        }



        [Fact]
        public void ShowUserbookReport_NoSelectedDate_SuccessfulResponse_NonEmptyBookList()
        {
            // Arrange
            var selectedDate = (DateTime?)null;
            var userId = 1;
            var type = "issue";
            var page = 1;
            var pageSize = 2;
            var totalPages = 2;
            var apiGetBooksUrl = $"fakeEndPointReport/UserBookReport?userId={userId}&type={type}&page={page}&pageSize={pageSize}";
            var apiGetBooksCountUrl = $"fakeEndPointReport/GetBookCountForUser?userId={userId}&type={type}";

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var cookies = new Dictionary<string, string>
            {
                { "userId", userId.ToString() }
            };

            mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var mockCountOfBooks = new ServiceResponse<int>
            {
                Success = true,
                Data = 5
            };


            var expectedUserBookReportData = new List<UserBookReportViewModel>
            {
                new UserBookReportViewModel { /* Initialize with mock data */ },
                new UserBookReportViewModel { /* Initialize with mock data */ }
            };
            var expectedResponseForGet = new ServiceResponse<IEnumerable<UserBookReportViewModel>>
            {
                Success = true,
                Data = expectedUserBookReportData
            };

            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1");

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockCountOfBooks);

            // Setup for ExecuteApiRequest for list of user book report view models
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserBookReportViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponseForGet);

            var target = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = target.ShowUserbookReport(selectedDate, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserBookReportViewModel>>(result.Model);
            var model = result.Model as List<UserBookReportViewModel>;
            Assert.Equal(expectedUserBookReportData.Count, model.Count);
            Assert.Equal(page, result.ViewData["CurrentPage"]);
            Assert.Equal(pageSize, result.ViewData["PageSize"]);
            Assert.Equal(type, result.ViewData["Type"]);
            Assert.Equal(userId, result.ViewData["UserId"]);
        }



        [Fact]
        public void ShowUserbookReport_NoSelectedDate_SuccessfulResponse_EmptyBookList()
        {
            // Arrange
            var selectedDate = (DateTime?)null;
            var userId = 1;
            var type = "issue";
            var page = 1;
            var pageSize = 2;
            var apiGetBooksUrl = $"fakeEndPointReport/UserBookReport?userId={userId}&type={type}&page={page}&pageSize={pageSize}";
            var apiGetBooksCountUrl = $"fakeEndPointReport/GetBookCountForUser?userId={userId}&type={type}";

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var cookies = new Dictionary<string, string>
            {
                { "userId", userId.ToString() }
            };

            mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var mockCountOfBooks = new ServiceResponse<int>
            {
                Success = true,
                Data = 0
            };
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1");

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockCountOfBooks);

            var target = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = target.ShowUserbookReport(selectedDate, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserBookReportViewModel>>(result.Model);
            var model = result.Model as List<UserBookReportViewModel>;
            Assert.Empty(model);
            Assert.Equal(page, result.ViewData["CurrentPage"]);
            Assert.Equal(pageSize, result.ViewData["PageSize"]);
            Assert.Equal(0, result.ViewData["TotalPages"]);
            Assert.Equal(type, result.ViewData["Type"]);
            Assert.Equal(userId, result.ViewData["UserId"]);
        }



        [Fact]
        public void ShowUserbookReport_SelectedDate_SuccessfulResponse_NonEmptyBookList()
        {
            // Arrange
            var selectedDate = new DateTime(2024, 7, 1);
            var userId = 1;
            var type = "issue";
            var page = 1;
            var pageSize = 2;
            var totalPages = 1;
            var apiGetBooksUrl = $"fakeEndPointReport/UserBookReport?userId={userId}&selectedDate={selectedDate}&type={type}&page={page}&pageSize={pageSize}";
            var apiGetBooksCountUrl = $"fakeEndPointReport/GetBookCountForUser?userId={userId}&selectedDate={selectedDate}&type={type}";

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var cookies = new Dictionary<string, string>
            {
                { "userId", userId.ToString() }
            };

            mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1");
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var mockCountOfBooks = new ServiceResponse<int>
            {
                Success = true,
                Data = 5
            };


            var expectedUserBookReportData = new List<UserBookReportViewModel>
            {
                new UserBookReportViewModel {  },
                new UserBookReportViewModel {  }
            };
            var expectedResponseForGet = new ServiceResponse<IEnumerable<UserBookReportViewModel>>
            {
                Success = true,
                Data = expectedUserBookReportData
            };


            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockCountOfBooks);


            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserBookReportViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponseForGet);

            var target = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = target.ShowUserbookReport(selectedDate, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserBookReportViewModel>>(result.Model);
            var model = result.Model as List<UserBookReportViewModel>;
            Assert.Equal(expectedUserBookReportData.Count, model.Count);
            Assert.Equal(page, result.ViewData["CurrentPage"]);
            Assert.Equal(pageSize, result.ViewData["PageSize"]);
            Assert.Equal(type, result.ViewData["Type"]);
            Assert.Equal(userId, result.ViewData["UserId"]);
        }


        [Fact]
        public void ShowUserbookReport_RedirectsToCorrectAction_WhenPageExceedsTotalPages()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();

            var mockConfiguration = new Mock<IConfiguration>();
            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object);


            var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1");

            var userId = 1;
            var selectedDate = DateTime.UtcNow;
            var type = "issue";
            var page = 5;
            var pageSize = 2;
            var totalPages = 2;


            var countOfBooksResponse = new ServiceResponse<int>()
            {
                Data = 5
            };
            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), It.IsAny<HttpMethod>(), It.IsAny<HttpRequest>(), null, 60))
                .Returns(countOfBooksResponse);

            // Act
            var result = controller.ShowUserbookReport(selectedDate, type, page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ShowUserbookReport", result.ActionName);
            Assert.Equal(pageSize, result.RouteValues["pageSize"]);
        }


        [Fact]
        public void ShowUserbookReport_PageExceedsTotalPages_RedirectToViewModel()
        {
            // Arrange
            int userId = 1;
            DateTime? selectedDate = null;
            var type = "issue";
            int page = 2;
            int pageSize = 2;
            string endPoint = "fakeEndPoint";

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 3
            };

            var mockFailureResponse = new ServiceResponse<IEnumerable<UserBookReportViewModel>>
            {
                Success = false,
                Data = null,
                Message = "Failed to retrieve data"
            };
        
            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<UserBookReportViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockFailureResponse);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.ShowUserbookReport(selectedDate, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserBookReportViewModel>>(result.Model);
            var model = result.Model as List<UserBookReportViewModel>;
            Assert.Empty(model);
        }



        //AdminPartialView
        [Fact]
        public void AdminPartialreport_ReturnsPartialView()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var target = new ReportController(mockHttpClientService.Object, mockConfiguration.Object);

            // Act
            var result = target.AdminPartialreport() as PartialViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("_AdminReportPartialView", result.ViewName);
        }



        //AdminReport
        [Fact]
        public void AdminReport_WithUserId_SuccessfulResponse_ReturnsCorrectView()
        {
            // Arrange
            int userId = 1;
            DateTime? issuedate = null;
            int page = 1;
            int pageSize = 4;
            int total = 0;
            string endPoint = "fakeEndPoint";
            var mockHttpClientService = new Mock<IHttpClientService>();
            var expectedAdminReportData = new List<AdminReportBookViewModel>
            {
                new AdminReportBookViewModel { Title = "Title 1" },
                new AdminReportBookViewModel { Title = "Title 1"  }
            };
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Success = true,
                Data = usersList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);


            var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);


            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var apiGetContactsUrl = $"{endPoint}Report/AdminBookReport?userId={userId}&page={page}&pageSize={pageSize}";
            var apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent?userId={userId}";

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 10
            };

            var successfulResponseBooks = new ServiceResponse<IEnumerable<AdminReportBookViewModel>>
            {
                Success = true,
                Data = expectedAdminReportData
            };

            mockHttpClientService.Setup(x =>
       x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(),
           HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
       .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportBookViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseBooks);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminReport(userId, issuedate, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var model = result.Model as List<AdminReportBookViewModel>;
            Assert.NotNull(model);
            Assert.Equal(expectedAdminReportData.Count, model.Count);

            Assert.Equal(userId, controller.ViewBag.UserId);
            Assert.Equal(page, controller.ViewBag.CurrentPage);
            Assert.Equal(pageSize, controller.ViewBag.PageSize);
            Assert.Null(controller.ViewBag.IssueDate);


        }


        [Fact]
        public void AdminReport_WithIssueDate_SuccessfulResponse_ReturnsCorrectView1()
        {
            // Arrange
            int? userId = null;
            DateTime issuedate = new DateTime(2024, 7, 5);
            int page = 1;
            int pageSize = 4;
            string endPoint = "fakeEndPoint";

            var expectedAdminReportData = new List<AdminReportBookViewModel>
            {
                new AdminReportBookViewModel { Title = "Title 1" },
                new AdminReportBookViewModel { Title = "Title 2" }
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = false,
                Data = 2
            };
            var apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent?issuedate={issuedate.ToString("yyyy-MM-dd")}";
            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    apiGetCountUrl, HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);
            var successfulResponseBooks = new ServiceResponse<IEnumerable<AdminReportBookViewModel>>
            {
                Success = true,
                Data = expectedAdminReportData
            };
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Success = true,
                Data = usersList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);


            var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(),
                    HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportBookViewModel>>>(
                    It.IsAny<string>(),
                    HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseBooks);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(
                    $"{endPoint}User/GetAllUsers", HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedQueResponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();

            mockHttpContext.SetupGet(c => c.Request).Returns(mockHttpRequest.Object);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };

            // Act
            var result = controller.AdminReport(userId, issuedate, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var model = result.Model as List<AdminReportBookViewModel>;
            Assert.NotNull(model);
            Assert.Equal(expectedAdminReportData.Count, model.Count);

            Assert.Null(controller.ViewBag.UserId);
            Assert.Equal(page, controller.ViewBag.CurrentPage);
            Assert.Equal(pageSize, controller.ViewBag.PageSize);
            Assert.Equal(issuedate, controller.ViewBag.IssueDate);
        }


        [Fact]
        public void AdminReport_UnsuccessfulResponse_ReturnsEmptyList()
        {
            // Arrange
            int? userId = null;
            DateTime? issuedate = null;
            int page = 1;
            int pageSize = 4;
            string endPoint = "fakeEndPoint";

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var apiGetContactsUrl = $"{endPoint}Report/AdminBookReport?page={page}&pageSize={pageSize}";
            var apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent";

            var unsuccessfulResponseCount = new ServiceResponse<int>
            {
                Success = false,
                Message = "Failed to fetch book count."
            };

            var unsuccessfulResponseBooks = new ServiceResponse<IEnumerable<AdminReportBookViewModel>>
            {
                Success = false,
                Message = "Failed to fetch admin report data."
            };

            var unsuccessfulResponseUsers = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Success = false,
                Message = "Failed to fetch user data."
            };

            var unsuccessfulResponseBooksList = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = false,
                Message = "Failed to fetch books data."
            };

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(unsuccessfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportBookViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(unsuccessfulResponseBooks);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(unsuccessfulResponseUsers);

            mockHttpClientService.Setup(x =>
               x.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(
                   It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(unsuccessfulResponseBooksList);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminReport(userId, issuedate, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var model = result.Model as List<AdminReportBookViewModel>;
            Assert.NotNull(model);
            Assert.Empty(model);

        }


        [Fact]
        public void AdminReport_PageGreaterThanTotalPages_RedirectsToCorrectAction()
        {
            // Arrange
            int? userId = 1; 
            DateTime? issuedate = null; 
            int page = 40; 
            int pageSize = 8;
            string endPoint = "fakeEndPoint";
            int totalCount = 12; 
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var apiGetContactsUrl = $"{endPoint}Report/AdminBookReport?page={page}&pageSize={pageSize}";
            var apiGetCountUrl = $"{endPoint}Report/GetBookCountWithDateOrStudent";

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = totalCount
            };

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);
             var usersList = new List<UserViewModel>()
                {
                    new UserViewModel{PasswordHint = 1, UserId = 1},
                    new UserViewModel{PasswordHint = 2, UserId = 2},
                };

                var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
                {
                    Success = true,
                    Data = usersList
                };

                mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                   .Returns(expectedQueResponse);


                var booksList = new List<BooksViewModel>()
                {
                    new BooksViewModel{Title = "1", Author = "1"},
                    new BooksViewModel{Title = "2", Author = "2"},
                };

                var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
                {
                    Success = true,
                    Data = booksList
                };

                mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                    .Returns(booksexpectedQueResponse);

                var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };

                // Act
                var result = controller.AdminReport(userId, issuedate, page, pageSize) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("AdminReport", result.ActionName); 
                Assert.Equal(userId, result.RouteValues["userId"]); 
                Assert.Equal(issuedate, result.RouteValues["issuedate"]); 
                Assert.Equal(1, result.RouteValues["page"]); 
                Assert.Equal(pageSize, result.RouteValues["pageSize"]); 
            }


        [Fact]
        public void AdminReport_PageExceedsTotalPages_RedirectsToFirstPage1()
        {
            // Arrange
            int bookId = 1;
            DateTime? issuedate = null;
            var type = "issue";
            int page = 2;
            int pageSize = 2;
            string endPoint = "fakeEndPoint";

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 3
            };

            var mockFailureResponse = new ServiceResponse<IEnumerable<AdminReportBookViewModel>>
            {
                Success = false,
                Data = null,
                Message = "Failed to retrieve data"
            };
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Success = true,
                Data = usersList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);


            var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportBookViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockFailureResponse);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminReport(bookId, issuedate, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<AdminReportBookViewModel>>(result.Model);
            var model = result.Model as List<AdminReportBookViewModel>;
            Assert.Empty(model);
        }




        //AdminUserReport
        [Fact]
        public void AdminUserReport_WithValidData_SuccessfulResponse_ReturnsCorrectView()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";
            int page = 1;
            int pageSize = 4;
            string endPoint = "fakeEndPoint";

            var expectedUserReportData = new List<AdminReportUserViewModel>
            {
                new AdminReportUserViewModel { /* Initialize with mock data */ },
                new AdminReportUserViewModel { /* Initialize with mock data */ }
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var apiGetUsersUrl = $"{endPoint}Report/AdminUserReport?bookId={bookId}&type={type}&page={page}&pageSize={pageSize}";
            var apiGetCountUserUrl = $"{endPoint}Report/GetUserCount?bookId={bookId}&type={type}";

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 10 
            };

            var successfulResponseUsers = new ServiceResponse<IEnumerable<AdminReportUserViewModel>>
            {
                Success = true,
                Data = expectedUserReportData
            };
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Success = true,
                Data = usersList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);


            var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);
            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportUserViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseUsers);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminUserReport(bookId, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var model = result.Model as List<AdminReportUserViewModel>;
            Assert.NotNull(model);
            Assert.Equal(expectedUserReportData.Count, model.Count);
            Assert.Equal(bookId, controller.ViewBag.BookId);
            Assert.Equal(page, controller.ViewBag.CurrentPage);
            Assert.Equal(pageSize, controller.ViewBag.PageSize);
            Assert.Equal(type, controller.ViewBag.Type);
        }


        [Fact]
        public void AdminUserReport_WithEmptyData_ReturnsEmptyView()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";
            int page = 1;
            int pageSize = 4;
            string endPoint = "fakeEndPoint";

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var apiGetUsersUrl = $"{endPoint}Report/AdminUserReport?bookId={bookId}&type={type}&page={page}&pageSize={pageSize}";
            var apiGetCountUserUrl = $"{endPoint}Report/GetUserCount?bookId={bookId}&type={type}";

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 0 
            };

            var successfulResponseUsers = new ServiceResponse<IEnumerable<AdminReportUserViewModel>>
            {
                Success = true,
                Data = new List<AdminReportUserViewModel>() 
            };

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

                    var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
                    {
                        Success = true,
                        Data = usersList
                    };

                    mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                       .Returns(expectedQueResponse);


                    var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportUserViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseUsers);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminUserReport(bookId, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var model = result.Model as List<AdminReportUserViewModel>;
            Assert.NotNull(model);
            Assert.Empty(model); 
            Assert.Equal(bookId, controller.ViewBag.BookId);
            Assert.Equal(page, controller.ViewBag.CurrentPage);
            Assert.Equal(pageSize, controller.ViewBag.PageSize);
            Assert.Equal(type, controller.ViewBag.Type);
        }


        [Fact]
        public void AdminUserReport_PageExceedsTotalPages_RedirectsToFirstPage()
        {
            // Arrange
            int bookId = 1;
            string type = "issue";
            int page = 40; 
            int pageSize = 4;
            string endPoint = "fakeEndPoint";

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 10 
            };

            var successfulResponseUsers = new ServiceResponse<IEnumerable<AdminReportUserViewModel>>
            {
                Success = true,
                Data = new List<AdminReportUserViewModel>() 
            };
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

                    var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
                    {
                        Success = true,
                        Data = usersList
                    };

                    mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                       .Returns(expectedQueResponse);


                    var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportUserViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseUsers);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminUserReport(bookId, type, page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AdminUserReport", result.ActionName); 
            Assert.Equal(bookId, result.RouteValues["bookId"]);
            Assert.Equal(type, result.RouteValues["type"]);
            Assert.Equal(1, result.RouteValues["page"]); 
            Assert.Equal(pageSize, result.RouteValues["pageSize"]);
        }


        [Fact]
        public void AdminUserReport_PageExceedsTotalPages_RedirectsToFirstPage1()
        {
            // Arrange
            int bookId = 1;
            var type = "issue";
            int page = 2;
            int pageSize = 2;
            string endPoint = "fakeEndPoint";

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpRequest = new Mock<HttpRequest>();

            var successfulResponseCount = new ServiceResponse<int>
            {
                Success = true,
                Data = 3
            };

            var mockFailureResponse = new ServiceResponse<IEnumerable<AdminReportUserViewModel>>
            {
                Success = false,
                Data = null, 
                Message = "Failed to retrieve data" 
            };
            var usersList = new List<UserViewModel>()
            {
                new UserViewModel{PasswordHint = 1, UserId = 1},
                new UserViewModel{PasswordHint = 2, UserId = 2},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Success = true,
                Data = usersList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);


            var booksList = new List<BooksViewModel>()
            {
                new BooksViewModel{Title = "1", Author = "1"},
                new BooksViewModel{Title = "2", Author = "2"},
            };

            var booksexpectedQueResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = booksList
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(booksexpectedQueResponse);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(successfulResponseCount);

            mockHttpClientService.Setup(x =>
                x.ExecuteApiRequest<ServiceResponse<IEnumerable<AdminReportUserViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(mockFailureResponse);

            var controller = new ReportController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.AdminUserReport(bookId, type, page, pageSize) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<AdminReportUserViewModel>>(result.Model);
            var model = result.Model as List<AdminReportUserViewModel>;
            Assert.Empty(model);
        }


       
    }
}
