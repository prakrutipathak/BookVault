using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Dtos;
using CivicaBookLibraryApi.Models;
using CivicaBookLibraryApi.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaBookLibraryApiTests.Services
{
    public class SecurityQuestionServiceTests
    {
        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionServiceTests")]

        public void GetAllSecurityquestions_ReturnsErrorMessage_WhenNoQuestionExists()
        {
            //Arrange
            var questions = new List<SecurityQuestionDto>();
            var response = new ServiceResponse<IEnumerable<SecurityQuestionDto>>()
            {
                Data = questions,
                Success = false,
                Message = "No question found!",
            };

            var mockSecurityQuestionRespository = new Mock<ISecurityQuestionRepository>();
            var target = new SecurityQuestionService(mockSecurityQuestionRespository.Object);

            // Act
            var actual = target.GetAllSecurityquestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockSecurityQuestionRespository.Verify(x => x.GetAllSecurityQuestions(), Times.Once);
        }

        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionServiceTests")]

        public void GetAllSecurityquestions_ReturnsFalse_WhenQuestionsAreNull()
        {
            // Arrange
            IEnumerable<SecurityQuestion> questions = null;

            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>()
            {
                Success = false,
                Message = "No question found!"
            };
            var mockSecurityQuestionRespository = new Mock<ISecurityQuestionRepository>();
            var target = new SecurityQuestionService(mockSecurityQuestionRespository.Object);

            mockSecurityQuestionRespository.Setup(c => c.GetAllSecurityQuestions()).Returns(questions);

            // Act
            var actual = target.GetAllSecurityquestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Success, actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockSecurityQuestionRespository.Verify(c => c.GetAllSecurityQuestions(), Times.Once);
        }

        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionServiceTests")]

        public void GetAllSecurityquestions_ReturnsTrue_WhenQuestionsExist()
        {
            // Arrange
            IEnumerable<SecurityQuestion> questions = new List<SecurityQuestion>()
            {
                new SecurityQuestion() { PasswordHint = 1,Question="TestQue1"},
                new SecurityQuestion() { PasswordHint = 2,Question="TestQue2"},
            };

            var response = new ServiceResponse<IEnumerable<SecurityQuestionDto>>()
            {
                Success = true,
                Message = "Success"
            };

            var mockSecurityQuestionRespository = new Mock<ISecurityQuestionRepository>();
            var target = new SecurityQuestionService(mockSecurityQuestionRespository.Object);

            mockSecurityQuestionRespository.Setup(c => c.GetAllSecurityQuestions()).Returns(questions);

            // Act
            var actual = target.GetAllSecurityquestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Success, actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockSecurityQuestionRespository.Verify(c => c.GetAllSecurityQuestions(), Times.Once);
        }
    }
}
