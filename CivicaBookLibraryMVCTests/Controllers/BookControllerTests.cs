
using AutoFixture;
using CivicaBookLibraryMVC.Controllers;
using CivicaBookLibraryMVC.Infrastructure;
using CivicaBookLibraryMVC.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
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
    public class BookControllerTests
    {
        //Test cases for BookIssue() httpget
        [Fact]
        [Trait("BookController","MVC")]
        public void BookIssue_ReturnsViewResult_WithBookList_WhenBookExists()
        {
            //Arrange
            var expectedBook = new List<BooksViewModel>
            {
                new BooksViewModel {BookId = 1,Title = "Title 1"},
                new BooksViewModel {BookId = 2,Title = "Title 2"},

            };
           
            var expectedResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = expectedBook,
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            //Act
            var actual = target.BookIssue() as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"],Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void BooksIssue_ReturnsViewResult_WithEmptyBookList_WhenBookDoesNotExists()
        {
            //Arrange
            var expectedResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = false,
            };
           
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);
           
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            //Act
            var actual = target.BookIssue() as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
           

        }
        //Test cases for BookIssue() httppost
        [Fact]
        [Trait("BookController", "MVC")]
        public void BookIssue_GetBook_RedirectToActionWhenBookIssueSuccessFully()
        {
            var expectedBook = new List<BooksViewModel>
            {
                new BooksViewModel {BookId = 1,Title = "Title 1"},
                new BooksViewModel {BookId = 2,Title = "Title 2"},

            };
            var book = new BookIssueViewModel { BookId = 1, UserId = 1 };

            var expectedResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = expectedBook,
            };
            var successMessage = "Book Issue Successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage,
            };
            var expectedModelResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            // Setup HttpContext mock
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1"); // Replace "1" with the actual user ID
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), book, It.IsAny<HttpRequest>())).Returns(expectedModelResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            //Act
            var actual = target.BookIssue(book) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("ShowUserbookReport", actual.ActionName);
            Assert.Equal("Report", actual.ControllerName);
            Assert.Equal("Book Issue Successfully", target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), book, It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void BookIssue_GetBook_RedirectToViewResultWhenErrorResponseIsNull()
        {
            var expectedBook = new List<BooksViewModel>
            {
                new BooksViewModel {BookId = 1,Title = "Title 1"},
                new BooksViewModel {BookId = 2,Title = "Title 2"},

            };
            var book = new BookIssueViewModel { BookId = 1, UserId = 1 };

            var expectedResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = expectedBook,
            };
            var errorMessage = "Something went wrong";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage,
            };
            var expectedModelResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            // Setup HttpContext mock
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1"); // Replace "1" with the actual user ID
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), book, It.IsAny<HttpRequest>())).Returns(expectedModelResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            //Act
            var actual = target.BookIssue(book) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Something went wrong", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), book, It.IsAny<HttpRequest>()),Times.Once);


        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void BookIssue_GetBook_RedirectToActionWhenErrorResponseIsNotNull()
        {
            var expectedBook = new List<BooksViewModel>
            {
                new BooksViewModel {BookId = 1,Title = "Title 1"},
                new BooksViewModel {BookId = 2,Title = "Title 2"},

            };
            var book = new BookIssueViewModel { BookId = 1, UserId = 1 };

            var expectedResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Success = true,
                Data = expectedBook,
            };
            var expectedModelResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            // Setup HttpContext mock
            mockHttpContext.Setup(m => m.Request.Cookies["userId"]).Returns("1"); // Replace "1" with the actual user ID
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), book, It.IsAny<HttpRequest>())).Returns(expectedModelResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            //Act
            var actual = target.BookIssue(book) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            Assert.Equal("Something went wrong try after some time", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), book, It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void ReturnBook_ReturnsRedirectToAction_WhenReturnBookSuccessfully()
        {
            // Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = "Success",
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Put, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedServiceResponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            // Act
            var actual = target.ReturnBook(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("ShowUserbookReport", actual.ActionName);
            Assert.Equal("Report", actual.ControllerName);
            Assert.Equal(expectedServiceResponse.Message, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Put, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void ReturnBook_ReturnViewResult_WhenReturnBookFail()
        {
            // Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = "Fail",
                Success = false
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Put, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedServiceResponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            // Act
            var actual = target.ReturnBook(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("ReturnBook", actual.ActionName);
            Assert.Equal("Book", actual.ControllerName);
            Assert.Equal(expectedServiceResponse.Message, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Put, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }
        //Index
        [Fact]
        [Trait("BookController", "MVC")]
        public void Index_ReturnsView_WithBooks()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var expectedContacts = new List<BooksViewModel>
            {
                new BooksViewModel
                {BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M },
            new BooksViewModel{
                BookId = 2,
                Title = "Title 2",
                Author = "Author 2",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M},
            };

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = expectedContacts.Count
                });
            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(new ServiceResponse<IEnumerable<BooksViewModel>>
               {
                   Success = true,
                   Data = expectedContacts
               });

            // Act
            var result = target.Index("title", "title", "asc", 1, 2) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedContacts, result.Model);
        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Index_RedirectsToFirstPage_WhenPageGreaterThanTotalPages()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var search = "title";
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { search, page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index(search, "title", "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Index_ReturnsList_WhenSerchisNull()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index(null, "title", "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Index_ReturnsList_WhenSerchisNotNullButSortByisNull()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index("title", null, "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Index_ReturnsList()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index(null, null, "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Index_ReturnsView_EmptyBooks_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = new List<BooksViewModel>
            {
                new BooksViewModel
                {BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M },
            new BooksViewModel{
                BookId = 2,
                Title = "Title 2",
                Author = "Author 2",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M},
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, null, sort_dir, page, pageSize) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        [Trait("BookController", "MVC")]

        public void Index_ReturnsView_Countzero_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = new List<BooksViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<BooksViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = 0,
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, null, sort_dir, page, pageSize) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BooksViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Never);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        //Add
        [Fact]
        [Trait("BookController", "MVC")]
        public void Create_ReturnsView()
        {
            //Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var actual = target.Add() as ViewResult;
            //Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once());
        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Create_BooksSavedSuccessfully_ReturnsViews()
        {
            //Arrange
            var fixture = new Fixture();
            var Book = new AddBookViewModel()
            {
                Title = "Titwass",
                Author = "Authosadad",
                TotalQuantity = 11,
                IssuedQuantity = 10,
                AvailableQuantity=1,
                PricePerBook = 1.23M
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            string successMessage = "Books Saved Successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message=successMessage
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), Book, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Add(Book) as ViewResult;
            //Assert
            Assert.Null(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), Book, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Create_ReturnsSameViewPage_WhenServiceResponseIsNull_RedirectToAction()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Build<AddBookViewModel>().Without(c => c.PricePerBook).Create();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var successMessage = "Books saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Add(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Create_ReturnsSameViewPage_WhenServiceResponseIsFalse_RedirectToAction()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Build<AddBookViewModel>().Without(c=>c.PricePerBook).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();

            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act

            var actual = target.Add(viewModel) as RedirectToActionResult;

            //Assert

            Assert.NotNull(actual);

            Assert.True(target.ModelState.IsValid);

            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Create_BookFailedToSave_ReturnRedirectToActionResult()

        {

            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Build<AddBookViewModel>().Without(c => c.PricePerBook).Create();


            var mockHttpClientService = new Mock<IHttpClientService>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };
            //Act
            var actual = target.Add(viewModel) as ViewResult;

            //Assert

            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, actual.TempData["ErrorMessage"]);

            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]

        public void Create_BookFailedToSave_ReturnSomethingwentWrong()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Build<AddBookViewModel>().Without(c => c.PricePerBook).Create();


            var mockHttpClientService = new Mock<IHttpClientService>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };
            //Act
            var actual = target.Add(viewModel) as ViewResult;

            //Assert

            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Something went wrong. Please try after sometime", actual.TempData["ErrorMessage"]);

            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        //Details
        [Fact]
        [Trait("BookController", "MVC")]
        public void Details_ReturnsDetailsView()
        {
            //Arrange
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceReponse = new ServiceResponse<BooksViewModel>()
            {
                Message = "",
                Success = true,
                Data = Book
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {

                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(Book.ToString(), actual.Model.ToString());
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Details_RedirectToIndex_WhenResponseSuccessIsFalse()
        {
            //Arrange
   
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "";

            var expectedServiceReponse = new ServiceResponse<BooksViewModel>()
            {
                Message = errorMessage,
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Details_RedirectToIndex_WhenResponseDataIsNull()
        {
            //Arrange
            var fixture = new Fixture();
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "something wrong";

            var expectedServiceReponse = new ServiceResponse<BooksViewModel>()
            {
                Message = errorMessage,
                Data = null,
                Success = true
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Details_RedirectToIndex_WhenErrorResponseIsNotNull()
        {
            //Arrange
            var fixture = new Fixture();
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "";

            var expectedServiceReponse = new ServiceResponse<BooksViewModel>()
            {
                Message = errorMessage,
                Data = Book,
                Success = true
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Details_RedirectToIndex_WhenErrorResponseIsNull()
        {
            //Arrange
            var fixture = new Fixture();
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "Something went wrong. Please try after sometime";

            var expectedServiceReponse = new ServiceResponse<BooksViewModel>()
            {
                Message = errorMessage,
                Data = Book,
                Success = true
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = null
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Details_RedirectToAction_WhenBookNotExists()
        {
            // Arrange
            int categoryId = 1;
            var expectedSuccessResponseContent = new ServiceResponse<BooksViewModel>
            {
                Success = false,
                Message = string.Empty,
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedSuccessResponse);
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                TempData = tempData,
            };

            // Act
            var actual = target.Details(categoryId) as RedirectToActionResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }
        //Delete
        [Fact]
        [Trait("BookController", "MVC")]
        public void Delete_ReturnsRedirectToAction_WhenBookDeletedSuccessfully()
        {
            var id = 1;
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity= 10,
                AvailableQuantity= 10,
                IssuedQuantity= 0,
                PricePerBook= 10.2M
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");
            var errorMessage = "Book Deleted Successfully";
            var expectedSuccessData = new ServiceResponse<string>
            {
                Success = true,
                Message = errorMessage
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSuccessData))
            };

            var httpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedSuccessData);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext.Object,
                }
            };

            //Act
            var actual = target.Delete(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Delete_ReturnsRedirectToAction_WhenBookisNotDeleted()
        {
            var id = 1;
            var Book = new BooksViewModel()
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                AvailableQuantity = 10,
                IssuedQuantity = 0,
                PricePerBook = 10.2M
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");
            var errorMessage = "Something went wrong, please try after sometime.";
            var expectedSuccessData = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSuccessData))
            };

            var httpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedSuccessData);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext.Object,
                }
            };

            //Act

            var actual = target.Delete(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }
        //Edit
        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var fixture = new Fixture();
            var viewModel = fixture.Build<BooksViewModel>().Without(c => c.PricePerBook).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockHttpContext = new Mock<HttpContext>();

            var expectedServiceResponse = new ServiceResponse<BooksViewModel>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);


            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_RedirectToIndex_WhenStatusCodeIsSuccess_serviceResponseIsFalse()
        {
            var id = 1;
            var fixture = new Fixture();
            var Book = fixture.Build<BooksViewModel>().Without(c => c.PricePerBook).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var mockHttpContext = new Mock<HttpContext>();

            var expectedServiceResponse = new ServiceResponse<BooksViewModel>
            {
                Success = false,
                Data = Book,
                Message = "error"
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_RedirectToIndex_WhenStatusCodeIsSuccess_serviceResponseIsNull()
        {
            var id = 1;
            var fixture = new Fixture();
            var Book = fixture.Build<BooksViewModel>().Without(c => c.PricePerBook).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var mockHttpContext = new Mock<HttpContext>();

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_RedirectToIndex_WhenStatusCodeIsSuccess_serviceResponseDataIsNull()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<BooksViewModel>
            {
                Message = "",
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_WhenErrorMessageIsNull_WhenStatusCodeIsNotSuccess()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<BooksViewModel>
            {
                Message = null,
                Data = new BooksViewModel
                {
                    BookId = id,
                    Title = "Title 1",
                    Author = "Author 1",
                    TotalQuantity = 10,
                    PricePerBook = 10.2M
                },
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_WhenErrorMessageNotNull_WhenStatusCodeIsNotSuccess()
        {
            //Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Something went wrong. Please try after sometime", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<BooksViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        //Edit
        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_BookSavedSuccessfully_RedirectToAction()
        {
            //Arrange
            var id = 1;
            var fixture = new Fixture();
            var viewModel = fixture.Build<BooksViewModel>().Without(c=>c.PricePerBook).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "Book saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_BookFailedToSave_WhenErrorResponseIsNotNull_ReturnRedirectToActionResult()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Build<BooksViewModel>().Without(c => c.PricePerBook).Create();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };


            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(viewModel);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_BookFailedToSave_ReturnSomethingWentWrong()
        {
            //Arrange
            var viewModel = new BooksViewModel
            {
                BookId = 1,
                Title = "Title 1",
                Author = "Author 1",
                TotalQuantity = 10,
                PricePerBook = 10.2M
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };


            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(viewModel);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Something went wrong. Please try after sometime", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        [Trait("BookController", "MVC")]
        public void Edit_whenModelFails()
        {
            //Arrange
            var viewModel = new BooksViewModel
            {
                Title= "c1"

            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
         
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var target = new BookController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            target.ModelState.AddModelError("Author", "Author is required.");

            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.False(target.ModelState.IsValid);
        }


    }
}
