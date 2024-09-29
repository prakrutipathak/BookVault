using CivicaBookLibraryApi.Controllers;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryApiTests.Controllers
{
    public class UserControllerTests
    {

        //Login 
        [Theory]
        [Trait("User", "UserControllerTests")]
        [InlineData("Invalid user login id or password!")]
        [InlineData("Something went wrong, Please try after sometime.")]
        public void Login_ReturnsBadRequest_WhenLoginFails(string message)
        {
            // Arrange
            var loginDto = new LoginDto();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = message

            };
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.LoginUserService(loginDto))
                           .Returns(expectedServiceResponse);

            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.Login(loginDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.False(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockUserService.Verify(service => service.LoginUserService(loginDto), Times.Once);
        }


        [Fact]
        [Trait("User", "UserControllerTests")]
        public void Login_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "username", Password = "password" };
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = string.Empty

            };
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.LoginUserService(loginDto))
                           .Returns(expectedServiceResponse);

            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.Login(loginDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(string.Empty, ((ServiceResponse<string>)actual.Value).Message);
            Assert.True(((ServiceResponse<string>)actual.Value).Success);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
            mockUserService.Verify(service => service.LoginUserService(loginDto), Times.Once);
        }

        // GetUserById

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetUserById_ReturnsOkResponse_WhenUserIsFound()
        {
            // Arrange
            var response = new ServiceResponse<UserDto>()
            {
                Data = { },
                Success = true,
                Message = "",
            };

            var userId = 1;
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.GetUserById(userId)).Returns(response);
            var target = new UserController(mockUserService.Object);

            // Act 
            var actual = target.GetUserById(userId) as OkObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            mockUserService.Verify(c => c.GetUserById(userId), Times.Once);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetUserById_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var response = new ServiceResponse<UserDto>()
            {
                Data = { },
                Success = false,
                Message = "",
            };

            var userId = 1;
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.GetUserById(userId)).Returns(response);
            var target = new UserController(mockUserService.Object);

            // Act 
            var actual = target.GetUserById(userId) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal((int)HttpStatusCode.NotFound, actual.StatusCode);
            mockUserService.Verify(c => c.GetUserById(userId), Times.Once);
        }

        // ChangePassword

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void ChangePassword_ReturnsOkResponse_WhenPasswordChangedSucessfully()
        {
            // Arrange
            var changePassword = new ChangePasswordDto() { LoginId = "abc", OldPassword = "Password@123", NewPassword = "Password@1234", ConfirmNewPassword = "Password@1234" };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = true,
                Message = "",
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.ChangePassword(changePassword)).Returns(response);
            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.ChangePassword(changePassword) as OkObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            mockUserService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void ChangePassword_ReturnsBadRequest_WhenPasswordChangeFails()
        {
            // Arrange
            var changePassword = new ChangePasswordDto() { LoginId = "abc", OldPassword = "Password@123", NewPassword = "Password@1234", ConfirmNewPassword = "Password@1234" };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = false,
                Message = "",
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.ChangePassword(changePassword)).Returns(response);
            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.ChangePassword(changePassword) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            mockUserService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        // ResetPassword

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void ResetPassword_ReturnsOkResponse_WhenPasswordResetSuccessfully()
        {
            //Arrnge
            var resetPassword = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 1,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = true,
                Message = "",
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.ResetPassword(resetPassword)).Returns(response);
            var target = new UserController(mockUserService.Object);

            //Act
            var actual = target.ResetPassword(resetPassword) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            mockUserService.Verify(c => c.ResetPassword(It.IsAny<ResetPasswordDto>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void ResetPassword_ReturnsBAdRequest_WhenPasswordResetFails()
        {
            //Arrnge
            var resetPassword = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 1,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = false,
                Message = "",
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.ResetPassword(resetPassword)).Returns(response);
            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.ResetPassword(resetPassword) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            mockUserService.Verify(c => c.ResetPassword(It.IsAny<ResetPasswordDto>()), Times.Once);
        }

        //Register

        [Theory]
        [Trait("User", "UserControllerTests")]
        [InlineData("User already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        [InlineData("Mininum password length should be 8")]
        [InlineData("Password should be apphanumeric")]
        [InlineData("Password should contain special characters")]
        public void Register_ReturnsBadRequest_WhenRegistrationFails(string message)
        {
            // Arrange
            var registerDto = new RegisterDto();
            var mockUserService = new Mock<IUserService>();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = message
            };
            mockUserService.Setup(service => service.RegisterUserService(registerDto))
                           .Returns(expectedServiceResponse);

            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.RegisterUser(registerDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.False(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockUserService.Verify(service => service.RegisterUserService(registerDto), Times.Once);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void Login_ReturnsOkResponse_WhenLoginSuccess()
        {
            // Arrange
            var registerDto = new RegisterDto();
            var mockUserService = new Mock<IUserService>();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = ""
            };
            mockUserService.Setup(service => service.RegisterUserService(registerDto))
                           .Returns(expectedServiceResponse);

            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.RegisterUser(registerDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.True(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
            mockUserService.Verify(service => service.RegisterUserService(registerDto), Times.Once);
        }

        // GetAllUsers

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetAllUsers_ReturnsNotFound_WhenUserNotExists()
        {
            // Arrange
            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserDto>>
            {
                Message = "No record found!",
                Success = false
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c=>c.GetAllUsers()).Returns(expectedServiceResponse);
            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.GetAllUsers() as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actual);
            Assert.IsType<ServiceResponse<IEnumerable<UserDto>>>(notFoundResult.Value);
            Assert.False(((ServiceResponse<IEnumerable<UserDto>>)notFoundResult.Value).Success);
            mockUserService.Verify(service => service.GetAllUsers(), Times.Once);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetAllUsers_ReturnsOkResponse_WhenUserExists()
        {
            // Arrange
            var userDto = new List<UserDto>()
            {
                new UserDto{UserId = 2, Name = "TestUser1" },
                new UserDto{UserId = 3, Name = "TestUser2" },
            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserDto>>
            {
                Message = string.Empty,
                Success = true,
                Data = userDto
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(c => c.GetAllUsers()).Returns(expectedServiceResponse);
            var target = new UserController(mockUserService.Object);

            // Act
            var actual = target.GetAllUsers() as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<IEnumerable<UserDto>>>(okResult.Value);
            Assert.True(((ServiceResponse<IEnumerable<UserDto>>)okResult.Value).Success);
            Assert.Equal(userDto, ((ServiceResponse<IEnumerable<UserDto>>)okResult.Value).Data);
            mockUserService.Verify(service => service.GetAllUsers(), Times.Once);
        }

        // DeleteUser

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void DeleteUser_ReturnsNotFound_WhenUserIdIsZero()
        {
            // Arrange
            var id = 0;
            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);
             
            //Act
            var actual = target.DeleteUser(id) as BadRequestObjectResult;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.Equal("Please enter valid data.", badRequestResult.Value);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void DeleteUser_ReturnsNotFound_WhenServicesFails()
        {
            // Arrange
            var id = 1; // Assuming valid ID
            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);

            mockUserService.Setup(x => x.RemoveUser(id)).Returns(new ServiceResponse<string> { Success = false });

            //Act
            var actual = target.DeleteUser(id) as BadRequestObjectResult;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void DeleteUser_ReturnsNotFound_WhenServiceSucceeds()
        {
            // Arrange
            var id = 1; // Assuming valid ID
            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);

            mockUserService.Setup(x => x.RemoveUser(id)).Returns(new ServiceResponse<string> { Success = true });

            //Act
            var actual = target.DeleteUser(id) as OkObjectResult;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
        }

        // GetTotalCountOfUsers

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetTotalCountOfUsers_ReturnsTotalCount_WhenUserExists()
        {
            //Arrange
            var searchedString = "test";
            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);

            mockUserService.Setup(x=>x.TotalUsers(searchedString)).Returns(new ServiceResponse<int> {Success = true});

            //Act
            var actual = target.GetTotalCountOfUsers(searchedString) as OkObjectResult;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<int>>(okResult.Value);
            Assert.True(((ServiceResponse<int>)okResult.Value).Success);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetTotalCountOfUsers_ReturnsNotFound_WhenServicesFails()
        {
            //Arrange
            var searchedString = "test";
            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);

            mockUserService.Setup(x => x.TotalUsers(searchedString)).Returns(new ServiceResponse<int> { Success = false });

            //Act
            var actual = target.GetTotalCountOfUsers(searchedString) as NotFoundObjectResult;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actual);
            Assert.IsType<ServiceResponse<int>>(notFoundResult.Value);
            Assert.False(((ServiceResponse<int>)notFoundResult.Value).Success);
        }

        // GetPaginatedUSer

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetPaginatedUSer_ReturnsNotFound_WhenServicesFailes()
        {
            //Arrange
            var searchString = "test";
            var page = 1;
            var pageSize = 4;
            var sortOrder = "asc";

            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);

            mockUserService.Setup(x=>x.GetPaginatedUsers(page,pageSize,searchString,sortOrder))
                .Returns(new ServiceResponse<IEnumerable<UserDto>> { Success = false });

            //Act
            var actual = target.GetPaginatedUSer(searchString,page,pageSize,sortOrder) as NotFoundObjectResult;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actual);
            Assert.IsType<ServiceResponse<IEnumerable<UserDto>>>(notFoundResult.Value);
            Assert.False(((ServiceResponse<IEnumerable<UserDto>>)notFoundResult.Value).Success);
        }

        [Fact]
        [Trait("User", "UserControllerTests")]
        public void GetPaginatedUSer_ReturnsOkResponse_WhenUsersExists()
        {
            //Arrange
            var searchString = "test";
            var page = 1;
            var pageSize = 4;
            var sortOrder = "asc";

            var mockUserService = new Mock<IUserService>();
            var target = new UserController(mockUserService.Object);

            mockUserService.Setup(x => x.GetPaginatedUsers(page, pageSize, searchString, sortOrder))
                .Returns(new ServiceResponse<IEnumerable<UserDto>> { Success = true });

            //Act
            var actual = target.GetPaginatedUSer(searchString, page, pageSize, sortOrder) as OkObjectResult;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<IEnumerable<UserDto>>>(okResult.Value);
            Assert.True(((ServiceResponse<IEnumerable<UserDto>>)okResult.Value).Success);
        }
    }
}
