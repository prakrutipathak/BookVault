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
    public class UserRepositoryTests
    {
        //Validate User
        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void ValidateUser_ReturnTrue()
        {
            //Arrange
            var users = new List<User>
            {
                new User
                {
                UserId = 1,
                Name = "Test",
                Email = "email@example.com",
                LoginId = "loginid",
                },
                new User
                {
                    UserId = 2,
                    Name = "Test1",
                    Email = "email@example.com",
                    LoginId = "loginid1",

                },
            }.AsQueryable();
            var username = "loginid";
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockAbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAbContext.Object);

            //Act
            var actual = target.ValidateUser(username);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.Users, Times.Exactly(1));
        }


        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void ValidateUser_WhenUsersIsNull()
        {
            //Arrange
            var users = new List<User>().AsQueryable();
            var username = "loginid";
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockAbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAbContext.Object);

            //Act
            var actual = target.ValidateUser(username);
            //Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAbContext.VerifyGet(c => c.Users, Times.Once);
        }

        // RegisterUser

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void RegisterUser_ReturnsFalse_WhenUserIsNull()
        {
            // Arrange
            User user = null;
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            // Act
            var actual = target.RegisterUser(user);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void RegisterUser_ReturnsTrue_WhenUserIsExists()
        {
            // Arrange
            var user = new User { UserId = 1, Name = "FirstName" };
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new UserRepository(mockAppDbContext.Object);

            // Act
            var actual = target.RegisterUser(user);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Add(user), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
            mockAppDbContext.VerifyGet(c => c.Users, Times.Once);
        }

        //GetUserById

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetUserById_ReturnsUser_WhenUserExists()
        {
            var id = 1;
            var user = new User { UserId = id, Name = "FirstName", LoginId = "TEst", Email = "Test@user.com" };
            var users = new List<User> { user }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();

            // This line sets up the Provider property of the mocked DbSet<Category> to return the Provider property of the categories IQueryable collection when accessed.
            // The Provider property is used by LINQ query execution.
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);

            // This line sets up the Expression property of the mocked DbSet<Category> to return the Expression property of the categories IQueryable collection when accessed.
            // The Expression property represents the LINQ expression tree associated with the IQueryable collection.
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetUserById(id);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(user, actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetUserById_ReturnsNull_WhenUserNotExists()
        {
            var id = 3;
            var user = new User { UserId = 1, Name = "FirstName", LoginId = "TEst", Email = "Test@user.com" };
            var users = new List<User> { user }.AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();

            // This line sets up the Provider property of the mocked DbSet<Category> to return the Provider property of the categories IQueryable collection when accessed.
            // The Provider property is used by LINQ query execution.
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);

            // This line sets up the Expression property of the mocked DbSet<Category> to return the Expression property of the categories IQueryable collection when accessed.
            // The Expression property represents the LINQ expression tree associated with the IQueryable collection.
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetUserById(id);

            //Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Users, Times.Once);
        }

        // UpdateUser

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void UpdateUser_ReturnsTrue()
        {
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);

            var target = new UserRepository(mockAppDbContext.Object);

            var user = new User()
            {
                UserId = 1,
                Name = "TestFName",
                LoginId = "Test",
                PhoneNumber = "74736273462",
                Email = "user@test.com",
                PasswordHint = 1,
                SecurityQuestion = new SecurityQuestion { PasswordHint = 1, Question = "testQue" },
            };

            var actual = target.UpdateUser(user);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(p => p.Update(user), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void UpdateUser_ReturnsFalse()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            var target = new UserRepository(mockAppDbContext.Object);
            User user = null;

            //Act
            var actual = target.UpdateUser(user);

            // Assert
            Assert.False(actual);
        }

        // UserExists

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void UserExists_ReturnsTrue_WhenLoginIdIsGiven()
        {
            // Arrange
            var loginId = "loginId";
            var email = "test@example.com";

            var user = new User()
            {
                LoginId = loginId,
                Email = "email@example.com",
            };
            var users = new List<User>()
            {
                user,
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            mockDbSet.As<IQueryable<User>>().Setup(o => o.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(o => o.Expression).Returns(users.Expression);

            mockAppDbContext.Setup(o => o.Users).Returns(mockDbSet.Object);

            var target = new UserRepository(mockAppDbContext.Object);

            // Act
            var actual = target.UserExists(loginId, email);

            // Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<User>>().Verify(o => o.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(o => o.Expression, Times.Once);
            mockAppDbContext.Verify(o => o.Users, Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void UserExists_ReturnsTrue_WhenEmailIsGiven()
        {
            // Arrange
            var loginId = "loginId";
            var email = "test@example.com";
            var contactNumber = "123456567";
            var user = new User()
            {
                LoginId = "login",
                Email = email,
                PhoneNumber = contactNumber,
            };
            var users = new List<User>()
            {
                user,
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            mockDbSet.As<IQueryable<User>>().Setup(o => o.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(o => o.Expression).Returns(users.Expression);

            mockAppDbContext.Setup(o => o.Users).Returns(mockDbSet.Object);

            var target = new UserRepository(mockAppDbContext.Object);

            // Act
            var actual = target.UserExists(loginId, email);

            // Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<User>>().Verify(o => o.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(o => o.Expression, Times.Once);
            mockAppDbContext.Verify(o => o.Users, Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void UserExists_ReturnsFalse()
        {
            // Arrange
            var loginId = "loginId";
            var email = "test@example.com";
            var contactNumber = "123456567";

            var user = new User()
            {
                LoginId = "temp",
                Email = "email@example.com",
            };
            var users = new List<User>()
            {
                user,
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            mockDbSet.As<IQueryable<User>>().Setup(o => o.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(o => o.Expression).Returns(users.Expression);

            mockAppDbContext.Setup(o => o.Users).Returns(mockDbSet.Object);

            var target = new UserRepository(mockAppDbContext.Object);

            // Act
            var actual = target.UserExists(loginId, email);

            // Assert
            Assert.False(actual);
            mockDbSet.As<IQueryable<User>>().Verify(o => o.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(o => o.Expression, Times.Once);
            mockAppDbContext.Verify(o => o.Users, Times.Once);
        }

        // UserCount

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void TotalUsers_ReturnsCorrectCount()
        {
            // Arrange
            var users = new List<User>
            {
            new User { UserId = 1, Name = "Test name1" },
            new User { UserId = 2, Name = "Test name2" },
            new User { UserId = 3, Name = "Test name3" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.GetEnumerator()).Returns(users.GetEnumerator());
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            // Act
            var result = target.UserCount();

            // Assert
            Assert.Equal(users.Count(), result);
        }

        // GetAllUsers

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetAllUsers_ReturnsUSers_WhenUserExists()
        {
            //Arrange
            var users = new List<User>()
            {
                new User{UserId =1,Name = "test1"},
                new User{UserId =2,Name = "test2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.GetEnumerator()).Returns(users.GetEnumerator());

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            // Act 
            var actual = target.GetAllUsers();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(users.Count(), actual.Count());
            mockAppDbContext.Verify(c => c.Users, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.GetEnumerator(), Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetAllUsers_ReturnsEmptyList_WhenUSerNotExists()
        {
            //Arrange
            var users = new List<User>().AsQueryable();
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.GetEnumerator()).Returns(users.GetEnumerator());

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            // Act 
            var actual = target.GetAllUsers();

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(users.Count(), actual.Count());
            mockAppDbContext.Verify(c => c.Users, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.GetEnumerator(), Times.Once);
        }

        // DeleteUser

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void DeleteUser_ReturnsTrue()
        {
            //Arrange
            var id = 2;
            var user = new User { UserId = 2, Name = "Test" };
            var users = new List<User> { user };
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.Setup(p => p.Find(id)).Returns<object[]>(ids => users.Find(c => c.UserId == (int)ids[0]));

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(x => x.SaveChanges()).Returns(1);

            var target = new UserRepository(mockAppDbContext.Object);

            // Act 
            var actual = target.DeleteUser(id);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(p => p.Find(id), Times.Once);
            mockAppDbContext.VerifyGet(c => c.Users, Times.Exactly(2));
            mockAppDbContext.Verify(x => x.SaveChanges(), Times.Once);
            mockDbSet.Verify(c => c.Remove(user), Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void DeleteUser_ReturnsFalse()
        {
            //Arrange
            var id = 1;
            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.Setup(p => p.Find(id)).Returns<User>(null);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);

            var target = new UserRepository(mockAppDbContext.Object);

            // Act 
            var actual = target.DeleteUser(id);

            // Assert
            Assert.False(actual);
            mockDbSet.Verify(p => p.Find(id), Times.Once);
            mockAppDbContext.VerifyGet(c => c.Users, Times.Once);
        }

        // TotalUser

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void TotalUser_ReturnsUSersCount()
        {
            // Arrange
            var serachString = "test";

            var users = new List<User>()
            {
                new User{UserId = 1, Name="test1"},
                new User{UserId = 2, Name="test2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalUsers(serachString);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(users.Count(), actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Users, Times.Once);
        }

        [Fact]
        [Trait("User","UserRepositoryTests")]
        public void TotalUser_ReturnsUSersCount_WhenSearchStringIsEmpty()
        {
            // Arrange
            var users = new List<User>()
            {
                new User{UserId = 1, Name="test1"},
                new User{UserId = 2, Name="test2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalUsers(null);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(users.Count(), actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Users, Times.Once);
        }

        // GetPaginatedUsers
        
        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetPaginatedUsers_retunsEmptyList_WhenUserExistsAscending()
        {
            //Arrange
            var page = 1;
            var pageSize = 4;
            var searchString = "test";
            var sortOrder = "asc";

            var users = new List<User>()
            {
                new User{UserId = 1, Name="test1"},
                new User{UserId = 2, Name="test2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetPaginatedUsers(page,pageSize,searchString,sortOrder);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(users, actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Users, Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetPaginatedUsers_retunsPaginatedUsers_WhenUserExistsDescending()
        {
            //Arrange
            var page = 1;
            var pageSize = 4;
            var searchString = "test";
            var sortOrder = "desc";

            var users = new List<User>()
            {
                new User{UserId = 1, Name="test1"},
                new User{UserId = 2, Name="test2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetPaginatedUsers(page, pageSize, searchString, sortOrder);

            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Users, Times.Once);
        }

        [Fact]
        [Trait("User", "UserRepositoryTests")]
        public void GetPaginatedUsers_retunsPaginatedUsers_WhenUserExistsDefault()
        {
            //Arrange
            var page = 1;
            var pageSize = 4;
            var searchString = "test";
            var sortOrder = "";

            var users = new List<User>()
            {
                new User{UserId = 1, Name="test1"},
                new User{UserId = 2, Name="test2"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(c => c.Expression).Returns(users.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);
            var target = new UserRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetPaginatedUsers(page, pageSize, searchString, sortOrder);

            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Provider, Times.Exactly(2));
            mockDbSet.As<IQueryable<User>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Users, Times.Once);
        }
    }
}
