using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using Xunit;

namespace Test_TestWorkForModsen
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<DatabaseContext> _options;

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User
                {
                    Id = 1,
                    Name = "User1",
                    Surname = "Surname1",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "user1@example.com"
                };
                var user2 = new User
                {
                    Id = 2,
                    Name = "User2",
                    Surname = "Surname2",
                    Birthday = new DateOnly(2001, 2, 2),
                    Email = "user2@example.com"
                };
                context.User.AddRange(user1, user2);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUser_WhenExists()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User
                {
                    Id = 1,
                    Name = "User1",
                    Surname = "Surname1",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "user1@example.com"
                };
                context.User.Add(user1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("User1", result.Name);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByIdAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenExists()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User
                {
                    Id = 1,
                    Name = "User1",
                    Surname = "Surname1",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "user1@example.com"
                };
                context.User.Add(user1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByEmailAsync("user1@example.com");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("user1@example.com", result.Email);
            }
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByEmailAsync("nonexistent@example.com");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_AddsUserToDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);
                var newUser = new User
                {
                    Id = 1,
                    Name = "NewUser",
                    Surname = "NewSurname",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "newuser@example.com"
                };

                // Act
                await repository.AddAsync(newUser);

                // Assert
                var result = await context.User.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("NewUser", result.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUserInDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User
                {
                    Id = 1,
                    Name = "User1",
                    Surname = "Surname1",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "user1@example.com"
                };
                context.User.Add(user1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);
                var updatedUser = new User
                {
                    Id = 1,
                    Name = "UpdatedUser",
                    Surname = "UpdatedSurname",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "updated@example.com"
                };

                // Act
                await repository.UpdateAsync(updatedUser);

                // Assert
                var result = await context.User.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("UpdatedUser", result.Name);
                Assert.Equal("updated@example.com", result.Email);
            }
        }

        [Fact]
        public async Task DeleteAsync_RemovesUserFromDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User
                {
                    Id = 1,
                    Name = "User1",
                    Surname = "Surname1",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "user1@example.com"
                };
                context.User.Add(user1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                await repository.DeleteAsync(1);

                // Assert
                var result = await context.User.FindAsync(1);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsPagedUsers()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User
                {
                    Id = 1,
                    Name = "User1",
                    Surname = "Surname1",
                    Birthday = new DateOnly(2000, 1, 1),
                    Email = "user1@example.com"
                };
                var user2 = new User
                {
                    Id = 2,
                    Name = "User2",
                    Surname = "Surname2",
                    Birthday = new DateOnly(2001, 2, 2),
                    Email = "user2@example.com"
                };
                var user3 = new User
                {
                    Id = 3,
                    Name = "User3",
                    Surname = "Surname3",
                    Birthday = new DateOnly(2002, 3, 3),
                    Email = "user3@example.com"
                };
                context.User.AddRange(user1, user2, user3);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetPagedAsync(1, 2);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Equal("User1", result.First().Name);
                Assert.Equal("User2", result.Last().Name);
            }
        }

        [Fact]
        public async Task DeleteByCompositeKeyAsync_ThrowsNotImplementedException()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act & Assert
                await Assert.ThrowsAsync<NotImplementedException>(() => repository.DeleteByCompositeKeyAsync(1, 1));
            }
        }

        [Fact]
        public async Task GetByCompositeKeyAsync_ThrowsNotImplementedException()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new UserRepository(context);

                // Act & Assert
                await Assert.ThrowsAsync<NotImplementedException>(() => repository.GetByCompositeKeyAsync(1, 1));
            }
        }
    }
}