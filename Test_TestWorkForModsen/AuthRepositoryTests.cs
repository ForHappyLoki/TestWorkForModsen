using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using Xunit;

namespace Test_TestWorkForModsen
{
    public class AuthRepositoryTests
    {
        private readonly DbContextOptions<DatabaseContext> _options;

        public AuthRepositoryTests()
        {
            // Используем In-Memory Database для тестов
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя базы данных для каждого теста
                .Options;
        }

        [Fact]
        public async Task AddRefreshTokenAsync_AddsTokenToDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new AuthRepository(context);
                var refreshToken = new RefreshToken
                {
                    Token = "test-token",
                    ExpiryTime = DateTime.UtcNow.AddDays(7),
                    AccountId = 1
                };

                // Act
                var result = await repository.AddRefreshTokenAsync(refreshToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("test-token", result.Token);
                Assert.Equal(1, result.AccountId);

                var tokenInDb = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "test-token");
                Assert.NotNull(tokenInDb);
                Assert.Equal("test-token", tokenInDb.Token);
            }
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ReturnsToken_WhenTokenExists()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var refreshToken = new RefreshToken
                {
                    Token = "test-token",
                    ExpiryTime = DateTime.UtcNow.AddDays(7),
                    AccountId = 1
                };
                context.RefreshTokens.Add(refreshToken);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new AuthRepository(context);

                // Act
                var result = await repository.GetRefreshTokenAsync("test-token");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("test-token", result.Token);
                Assert.Equal(1, result.AccountId);
            }
        }

        [Fact]
        public async Task GetRefreshTokenAsync_ReturnsNull_WhenTokenDoesNotExist()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new AuthRepository(context);

                // Act
                var result = await repository.GetRefreshTokenAsync("nonexistent-token");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task UpdateRefreshTokenAsync_UpdatesTokenInDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user = new User
                {
                    Name = "Test",
                    Surname = "Test",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "Test@gmail.com"
                };
                var refreshToken = new RefreshToken
                {
                    Token = "test-token",
                    ExpiryTime = DateTime.UtcNow.AddDays(7),
                    AccountId = 1,
                    Account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user }
                };
                context.RefreshTokens.Add(refreshToken);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new AuthRepository(context);
                var updatedToken = new RefreshToken
                {
                    Token = "test-token",
                    ExpiryTime = DateTime.UtcNow.AddDays(14), 
                    AccountId = 1
                };

                // Act
                await repository.UpdateRefreshTokenAsync(updatedToken);

                // Assert
                var tokenInDb = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "test-token");
                Assert.NotNull(tokenInDb);
            }
        }
    }
}