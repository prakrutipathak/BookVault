using CivicaBookLibraryApi.Data.Implementation;
using CivicaBookLibraryApi.Data;
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
    public class SecurityQuestionRepositoryTests
    {
        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionRepositoryTests")]
        public void GetAllSecurityQuestions_ReturnsQuestions_WhenQuestionsExists()
        {
            var questions = new List<SecurityQuestion>()
            {
                new SecurityQuestion{PasswordHint = 1, Question = "TestQue1"},
                new SecurityQuestion{PasswordHint = 2, Question = "TestQue2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<SecurityQuestion>>();
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(c => c.GetEnumerator()).Returns(questions.GetEnumerator());

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.SecurityQuestions).Returns(mockDbSet.Object);
            var target = new SecurityQuestionRepository(mockAppDbContext.Object);

            // Act 
            var actual = target.GetAllSecurityQuestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(questions.Count(), actual.Count());
            mockAppDbContext.Verify(c => c.SecurityQuestions, Times.Once);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Verify(c => c.GetEnumerator(), Times.Once);
        }

        [Fact]
        [Trait("SecurityQuestion", "SecurityQuestionRepositoryTests")]

        public void GetAllSecurityQuestions_ReturnsEmptyList_WhenNoQuestionsExists()
        {
            var questions = new List<SecurityQuestion>().AsQueryable();
            var mockDbSet = new Mock<DbSet<SecurityQuestion>>();
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(c => c.GetEnumerator()).Returns(questions.GetEnumerator());

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.SecurityQuestions).Returns(mockDbSet.Object);
            var target = new SecurityQuestionRepository(mockAppDbContext.Object);

            // Act 
            var actual = target.GetAllSecurityQuestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(questions.Count(), actual.Count());
            mockAppDbContext.Verify(c => c.SecurityQuestions, Times.Once);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Verify(c => c.GetEnumerator(), Times.Once);
        }
    }
}
