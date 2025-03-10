using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace Test_TestWorkForModsen
{
    public class UserModelTests
    {
        [Fact]
        public void User_Should_Set_Properties_Correctly()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Surname = "Test Surname",
                Email = "test@example.com",
                Birthday = new DateOnly(1999, 1, 1)
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("Test User", user.Name);
            Assert.Equal("Test Surname", user.Surname);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal(new DateOnly(1999, 1, 1), user.Birthday);
        }
    }
    public class UserApiTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<User>>();
            var testUser = new User
            {
                Id = 1,
                Name = "Test User",
                Surname = "Test Surname",
                Email = "test@example.com",
                Birthday = new DateOnly(1999, 1, 1)
            };
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(testUser);

            // Act
            var result = await mockRepository.Object.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.Id, result.Id);
            Assert.Equal(testUser.Name, result.Name);
            Assert.Equal(testUser.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<User>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((User)null);

            // Act
            var result = await mockRepository.Object.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser_WhenUserIsValid()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<User>>();
            var testUser = new User
            {
                Id = 1,
                Name = "Test User",
                Surname = "Test Surname",
                Email = "test@example.com",
                Birthday = new DateOnly(1999, 1, 1)
            };

            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback<User>(u =>
                {
                    Assert.Equal(testUser.Id, u.Id);
                    Assert.Equal(testUser.Name, u.Name);
                    Assert.Equal(testUser.Email, u.Email);
                })
                .Returns(Task.CompletedTask);

            // Act
            await mockRepository.Object.AddAsync(testUser);

            // Assert
            mockRepository.Verify(repo => repo.AddAsync(testUser), Times.Once);
        }
    }
}
