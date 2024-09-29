using AutoFixture;
using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Contract;
using CivicaBookLibraryApi.Services.Implementation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryApiTests.Services
{
    public class UserServiceTests
    {
        //LoginUserService
        [Fact]
        [Trait("User", "UserServiceTests")]
        public void LoginUserService_ReturnsSomethingWentWrong_WhenLoginDtoIsNull()
        {
            //Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockConfiguration = new Mock<IVerifyPasswordHashService>();

            var target = new UserService(mockUserRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, Please try after sometime.", result.Message);

        }


        [Fact]
        [Trait("User", "UserServiceTests")]
        public void LoginUserService_ReturnsInvalidUsernameOrPassword_WhenUserIsNull()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username"
            };
            var mockUserRepository = new Mock<IUserRepository>();
            var mockConfiguration = new Mock<IVerifyPasswordHashService>();
            mockUserRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns<User>(null);

            var target = new UserService(mockUserRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid user login id or password!", result.Message);
            mockUserRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);


        }


        [Fact]
        [Trait("User", "UserServiceTests")]
        public void LoginUserService_ReturnsInvalidUsernameOrPassword_WhenPasswordIsWrong()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var user = new User
            {
                LoginId = loginDto.Username,
                PasswordHash = new byte[] { 0x01, 0x23, 0x45 },
                PasswordSalt = new byte[] { 0xAB, 0xCD }
            };
            var mockUserRepository = new Mock<IUserRepository>();
            var mockConfiguration = new Mock<IVerifyPasswordHashService>();
            mockUserRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns(user);
            mockConfiguration.Setup(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt)).Returns(false);

            var target = new UserService(mockUserRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid user login id or password!", result.Message);
            mockUserRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);
            mockConfiguration.Verify(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt), Times.Once);


        }


        [Fact]
        [Trait("User", "UserServiceTests")]
        public void LoginUserService_ReturnsResponse_WhenLoginIsSuccessful()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var user = new User
            {
                LoginId = loginDto.Username,
                PasswordHash = new byte[] { 0x01, 0x23, 0x45 },
                PasswordSalt = new byte[] { 0xAB, 0xCD }
            };
            var mockUserRepository = new Mock<IUserRepository>();
            var mockConfiguration = new Mock<IVerifyPasswordHashService>();
            mockUserRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns(user);
            mockConfiguration.Setup(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt)).Returns(true);
            mockConfiguration.Setup(repo => repo.CreateToken(user)).Returns("");

            var target = new UserService(mockUserRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            mockUserRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);
            mockConfiguration.Verify(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt), Times.Once);
            mockConfiguration.Verify(repo => repo.CreateToken(user), Times.Once);


        }

        // Change Password

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenDtoIsNull()
        {
            //Arrange
            ChangePasswordDto dto = null;
            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = dto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            //Act
            var actual = target.ChangePassword(dto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenExistingUerIsNull()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns<User>(null);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenNewAndOldPasswordIsSame()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "NewTest@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "New password cannot be same as old password."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenVerifyPasswordHashFails()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Old password is incorrect."
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(false);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenUpdationFails()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockUserRepository.Setup(p => p.UpdateUser(It.IsAny<User>())).Returns(false);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockUserRepository.Verify(p => p.UpdateUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ChangePassword_ReturnsSuccessMessage_WhenUpdatedSuccessfully()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = true,
                Message = "Successfully updated password.Signin again!"
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockUserRepository.Setup(p => p.UpdateUser(It.IsAny<User>())).Returns(true);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockUserRepository.Verify(p => p.UpdateUser(It.IsAny<User>()), Times.Once);
        }

        // Reset password

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsSuccessMessage_WhenDtoIsNull()
        {
            //Arrange
            ResetPasswordDto resetPasswordDto = null;
            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenExistingUerIsNull()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint= 1 ,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Invalid loginId!"
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns<User>(null);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenQuestionSelectedIsNotSame()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 2,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "User verification failed!"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                PasswordHint = 1,
                LoginId = resetPasswordDto.LoginId,
            };
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenAnswerVerificationFails()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 1,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "User verification failed!"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                PasswordHint = 1,
                LoginId = resetPasswordDto.LoginId,
            };
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(c=>c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(false);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>()),Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenPasswordStrengthIsWeak()
        {
            //Arrange
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 1,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "test",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Mininum password length should be 8\r\nPassword should be alphanumeric\r\nPassword should contain special characters\r\n"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                PasswordHint = 1,
                LoginId = resetPasswordDto.LoginId,
            };
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenPasswordUpdateFails()
        {
            //Arrange
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 1,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                PasswordHint = 1,
                LoginId = resetPasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockUserRepository.Setup(x => x.UpdateUser(user)).Returns(false);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUser(user),Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void ResetPassword_ReturnsSuccess_WhenPasswordResetSuccessfully()
        {
            //Arrange
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                PasswordHint = 1,
                PasswordHintAnswer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = true,
                Message = "Successfully updated password."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                PhoneNumber = "6798765678",
                PasswordHint = 1,
                LoginId = resetPasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockUserRepository.Setup(x => x.UpdateUser(user)).Returns(true);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(c => c.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUser(user), Times.Once);
        }

        //register

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RegisterUserService_ReturnsSuccess_WhenValidRegistration()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            mockUserRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockUserRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(true);

            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            var registerDto = new RegisterDto
            {
                Name = "firstname",
                Email = "email@example.com",
                LoginId = "loginid",
                PhoneNumber = "1234567890",
                Password = "Password@123",
                PasswordHintAnswer = "TestAnswer",
                DateOfBirth = Convert.ToDateTime("2002-02-20"),
            };

            // Act
            var result = target.RegisterUserService(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Message);
            mockUserRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockUserRepository.Verify(c => c.RegisterUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RegisterUserService_ReturnsFailure_WhenRegistrationFails()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            mockUserRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            mockUserRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(false);

            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            var registerDto = new RegisterDto
            {
                Name = "firstname",
                Email = "email@example.com",
                LoginId = "loginid",
                PhoneNumber = "1234567890",
                Password = "Test@1234",
                PasswordHintAnswer = "TestAnswer",
                DateOfBirth = Convert.ToDateTime("2002-02-20"),
            };

            // Act
            var result = target.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try after sometime.", result.Message);
            mockUserRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockUserRepository.Verify(c => c.RegisterUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RegisterUserService_ReturnsFailure_WhenPasswordStrengthIsWeak()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            var registerDto = new RegisterDto
            {
                Name = "firstname",
                Email = "email@example.com",
                LoginId = "loginid",
                PhoneNumber = "1234567890",
                Password = "test",
                PasswordHintAnswer = "TestAnswer",
                DateOfBirth = Convert.ToDateTime("2002-02-20"),
            };

            // Act
            var actual = target.RegisterUserService(registerDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Mininum password length should be 8\r\nPassword should be alphanumeric\r\nPassword should contain special characters\r\n", actual.Message);
            Assert.False(actual.Success);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RegisterUserService_ReturnsFailure_WhenUserAlreayExists()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            var registerDto = new RegisterDto
            {
                Name = "firstname",
                Email = "email@example.com",
                LoginId = "loginid",
                PhoneNumber = "1234567890",
                Password = "Test@1234",
                PasswordHintAnswer = "TestAnswer",
                DateOfBirth = Convert.ToDateTime("2002-02-20"),
            };

            mockUserRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = target.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User already exists", result.Message);
            mockUserRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RegisterUserService_ReturnsFailure_WhenDOBisOfFuture()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            var registerDto = new RegisterDto
            {
                Name = "firstname",
                Email = "email@example.com",
                LoginId = "loginid",
                PhoneNumber = "1234567890",
                Password = "Test@1234",
                PasswordHintAnswer = "TestAnswer",
                DateOfBirth = Convert.ToDateTime("2026-02-20"),
            };

            mockUserRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = target.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Date of birth cannot be in future.", result.Message);
            mockUserRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RegisterUserService_ReturnsFailure_WhenAgeIsLessThan18()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            var registerDto = new RegisterDto
            {
                Name = "firstname",
                Email = "email@example.com",
                LoginId = "loginid",
                PhoneNumber = "1234567890",
                Password = "Test@1234",
                PasswordHintAnswer = "TestAnswer",
                DateOfBirth = Convert.ToDateTime("2020-02-20"),
            };

            mockUserRepository.Setup(repo => repo.UserExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = target.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Age cannot be less than 18 or greater than 120.", result.Message);
            mockUserRepository.Verify(c => c.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        // getUserById

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetUserById_ReturnsUser_WhenUSerExists()
        {
            //Assert
            var user = new User()
            {
                UserId = 1,
                Name = "Testname",
                LoginId = "test",
                Email = "user@test.com",
                PhoneNumber = "9898989898",
                PasswordHint = 1,
                SecurityQuestion = new SecurityQuestion()
                {
                    PasswordHint = 1,
                    Question = "testQuestion"
                },
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(x => x.GetUserById(user.UserId)).Returns(user);

            //Act
            var actual = target.GetUserById(user.UserId);

            //Assert
            Assert.NotNull(actual);
            mockUserRepository.Verify(x => x.GetUserById(user.UserId), Times.Once);
            Assert.True(actual.Success);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetUserById_ReturnsUser_WhenUserNotExists()
        {
            //Assert
            var user = new User()
            {
                UserId = 1,
                Name = "Testname",
                LoginId = "test",
                Email = "user@test.com",
                PhoneNumber = "9898989898",
                PasswordHint = 1,
                SecurityQuestion = new SecurityQuestion()
                {
                    PasswordHint = 1,
                    Question = "testQuestion"
                },
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(x => x.GetUserById(user.UserId)).Returns(It.IsAny<User>());

            //Act
            var actual = target.GetUserById(user.UserId);

            //Assert
            Assert.NotNull(actual);
            mockUserRepository.Verify(x => x.GetUserById(user.UserId), Times.Once);
            Assert.Equal("No record found!", actual.Message);
            Assert.False(actual.Success);
        }

        // GetAllUsers

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetAllUsers_ReturnsErrorMessage_WhenNoUserExists()
        {
            // Arrange
            var users = new List<User>();
            var response = new ServiceResponse<IEnumerable<User>>()
            {
                Data = users,
                Success = false,
                Message = "No record found!",
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(X=>X.GetAllUsers()).Returns(users);

            //Act
            var actual = target.GetAllUsers();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetAllUsers_ReturnsFalse_WhenUSersAreNull()
        {
            //Arrange
            IEnumerable<User> users = null;
            var response = new ServiceResponse<IEnumerable<UserDto>>()
            {
                Success = false,
                Message = "No record found!"
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(X => X.GetAllUsers()).Returns(users);

            //Act
            var actual = target.GetAllUsers();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Data, actual.Data);
            Assert.Equal(response.Success, actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.GetAllUsers(), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetAllUsers_ReturnsTrue_WhenUsersExists()
        {
            //Arrange
            IEnumerable<User> user = new List<User>()
            {
                new User{UserId = 2, Name ="testUser1"},
                new User{UserId = 3, Name ="testUser2"},
            };

            var response = new ServiceResponse<IEnumerable<UserDto>>()
            {
                Success = true,
                Message = ""
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(X => X.GetAllUsers()).Returns(user);

            //Act
            var actual = target.GetAllUsers();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Success, actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.GetAllUsers(), Times.Once);
        }

        // GetPaginatedUsers

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetPaginatedUsers_ReturnsErrorMessage_WhenNoUserExists()
        {
            // Arrange
            var page = 1;
            var pageSize = 4;
            var searchString = "test";
            var sortOrder = "asc";
            var users = new List<User>();
            var response = new ServiceResponse<IEnumerable<User>>()
            {
                Data = users,
                Success = false,
                Message = "No records found",
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(X => X.GetPaginatedUsers(page,pageSize,searchString,sortOrder)).Returns(users);

            //Act
            var actual = target.GetPaginatedUsers(page,pageSize,searchString,sortOrder);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.GetPaginatedUsers(page, pageSize, searchString, sortOrder), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetPaginatedUsers_ReturnsFalse_WhenUSersAreNull()
        {
            //Arrange
            var page = 1;
            var pageSize = 4;
            var searchString = "test";
            var sortOrder = "asc";
            IEnumerable<User> users = null;
            var response = new ServiceResponse<IEnumerable<UserDto>>()
            {
                Success = false,
                Message = "No records found"
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(X => X.GetPaginatedUsers(page, pageSize, searchString, sortOrder)).Returns(users);

            //Act
            var actual = target.GetPaginatedUsers(page, pageSize, searchString, sortOrder);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Data, actual.Data);
            Assert.Equal(response.Success, actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.GetPaginatedUsers(page, pageSize, searchString, sortOrder), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void GetPaginatedUsers_ReturnsTrue_WhenUsersExists()
        {
            //Arrange
            var page = 1;
            var pageSize = 4;
            var searchString = "test";
            var sortOrder = "asc";
            IEnumerable<User> user = new List<User>()
            {
                new User{UserId = 2, Name ="testUser1"},
                new User{UserId = 3, Name ="testUser2"},
            };

            var response = new ServiceResponse<IEnumerable<User>>()
            {
                Data = user,
                Success = true,
                Message = ""
            };

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(X => X.GetPaginatedUsers(page, pageSize, searchString, sortOrder)).Returns(user);

            //Act
            var actual = target.GetPaginatedUsers(page, pageSize, searchString, sortOrder);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Success, actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.GetPaginatedUsers(page, pageSize, searchString, sortOrder), Times.Once);
        }

        // RemoveUser

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RemoveUser_ReturnsErrorMessage_WhenDeletionFails()
        {
            // Arrange
            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = false,
                Message = "Something went wrong",
            };
            var id = 2;

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(x => x.DeleteUser(id)).Returns(false);

            //Act
            var actual = target.RemoveUser(id);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.DeleteUser(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void RemoveUser_ReturnsSuccessMessage_WhenDeletedSuccessfully()
        {
            // Arrange
            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = false,
                Message = "User deleted successfully",
            };
            var id = 2;

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(x => x.DeleteUser(id)).Returns(true);

            //Act
            var actual = target.RemoveUser(id);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.DeleteUser(It.IsAny<int>()), Times.Once);
        }

        // TotalUsers

        [Fact]
        [Trait("User", "UserServiceTests")]
        public void TotalUsers_ReturnsTotalCount()
        {
            //Arrange
            // Arrange
            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = true,
                Message = "",
            };
            var searchString = "test";

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenService = new Mock<IVerifyPasswordHashService>();
            var target = new UserService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(z=>z.TotalUsers(searchString)).Returns(2);

            var actual = target.TotalUsers(searchString);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(x => x.TotalUsers(searchString), Times.Once);
        }
    }
}
