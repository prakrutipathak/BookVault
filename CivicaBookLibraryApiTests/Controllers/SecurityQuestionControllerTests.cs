using CivicaBookLibraryApi.Controllers;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace CivicaBookLibraryApiTests.Controllers
{
    public class SecurityQuestionControllerTests
    {
        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionControllerTests")]
        public void GetAllSecurityQuestions_ReturnsOkResponse_WhenQuestionAreRetrivedSuccessfully()
        {
            // Arrange
            var questions = new List<SecurityQuestionDto>()
            {
                new SecurityQuestionDto{PasswordHint = 1, Question = "TestQuestion1"},
                new SecurityQuestionDto{PasswordHint = 2, Question = "TestQuestion2"},
            };

            var response = new ServiceResponse<IEnumerable<SecurityQuestionDto>>()
            {
                Data = questions,
                Success = true,
                Message = "",
            };

            var mockSecurityQuestionService = new Mock<ISecurityQuestionService>();
            mockSecurityQuestionService.Setup(c => c.GetAllSecurityquestions()).Returns(response);

            var target = new SecurityQuestionController(mockSecurityQuestionService.Object);

            // Act
            var actual = target.GetAllSecurityQuestions() as OkObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            mockSecurityQuestionService.Verify(c => c.GetAllSecurityquestions(), Times.Once);
        }


        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionControllerTests")]

        public void GetAllSecurityQuestions_ReturnsNotFound_WhenQuestionAreRetrivalFails()
        {
            // Arrange
            var questions = new List<SecurityQuestionDto>()
            {
                new SecurityQuestionDto{PasswordHint = 1, Question = "TestQuestion1"},
                new SecurityQuestionDto{PasswordHint = 2, Question = "TestQuestion2"},
            };

            var response = new ServiceResponse<IEnumerable<SecurityQuestionDto>>()
            {
                Data = questions,
                Success = false,
                Message = "",
            };

            var mockSecurityQuestionService = new Mock<ISecurityQuestionService>();
            mockSecurityQuestionService.Setup(c => c.GetAllSecurityquestions()).Returns(response);

            var target = new SecurityQuestionController(mockSecurityQuestionService.Object);

            // Act
            var actual = target.GetAllSecurityQuestions() as NotFoundObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.NotFound, actual.StatusCode);
            mockSecurityQuestionService.Verify(c => c.GetAllSecurityquestions(), Times.Once);

        }
    }
}
