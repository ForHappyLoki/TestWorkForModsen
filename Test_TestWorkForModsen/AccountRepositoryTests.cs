using Microsoft.EntityFrameworkCore;
using TestWork_Events.Data;
using TestWork_Events.Models;
using TestWork_Events.Repository;
using Xunit;

namespace TestWork_Events.Tests
{
    public class AccountRepositoryTests
    {
        private readonly DatabaseContext _context;
        private readonly AccountRepository _repository;
        private User user;
        private Account account;

        public AccountRepositoryTests()
        {
            // Создаем опции для использования InMemoryDatabase
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DatabaseContext(options);
            _repository = new AccountRepository(_context);
            user = new User
            {
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test@gmail.com"
            };
            account = new Account
            {
                Email = "Test@gmail.com",
                Role = "User",
                Password = "Test",
                UserId = 1,
                User = user,
            };
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user },
                new Account { Id = 2, Email = "test2@example.com", Role = "Admin", Password = "password2", UserId = 2, User = user }
            };

            _context.Account.AddRange(accounts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectAccount()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user };
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test1@example.com", result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnCorrectAccount()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1 };
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("test1@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAccount()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user };

            // Act
            await _repository.AddAsync(account);
            var result = await _context.Account.FindAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test1@example.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAccount()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user };
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            account.Email = "updated@example.com";

            // Act
            await _repository.UpdateAsync(account);
            var result = await _context.Account.FindAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", result.Email);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAccount()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user }; _context.Account.Add(account);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);
            var result = await _context.Account.FindAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user },
                new Account { Id = 2, Email = "test2@example.com", Role = "Admin", Password = "password2", UserId = 2, User = user },
                new Account { Id = 3, Email = "test3@example.com", Role = "User", Password = "password3", UserId = 3, User = user }
            };

            _context.Account.AddRange(accounts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetPagedAsync(2, 1);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result.First().Id);
        }
    }
}