using Microsoft.EntityFrameworkCore;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Data.Repository;
using Xunit;

namespace Test_TestWorkForModsen
{
    public class AccountRepositoryTests
    {
        private readonly DbContextOptions<DatabaseContext> _options;
        private readonly DatabaseContext _context;
        private readonly AccountRepository _repository;
        private readonly User user;
        private readonly Account account;

        public AccountRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new DatabaseContext(_options);
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

        private async Task ClearDatabaseAsync()
        {
            _context.Account.RemoveRange(_context.Account);
            _context.User.RemoveRange(_context.User);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAccounts()
        {
            await ClearDatabaseAsync();
            // Arrange
            var user1 = new User
            {
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test1@gmail.com"
            };
            var user2 = new User
            {
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test2@gmail.com"
            };
            _context.User.Add(user1);
            _context.User.Add(user2);
            await _context.SaveChangesAsync();
            var account1 = new Account
            {
                 Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user1
            };
            var account2 = new Account
            {
                 Id = 2, Email = "test2@example.com", Role = "Admin", Password = "password2", UserId = 2, User = user2
            };

            _context.Account.Add(account1);
            _context.Account.Add(account2);
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
            await ClearDatabaseAsync();
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
            await ClearDatabaseAsync();
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
            await ClearDatabaseAsync();
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
            await ClearDatabaseAsync();
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
            await ClearDatabaseAsync();
            var account = new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user };
            _context.Account.Add(account);
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
            await ClearDatabaseAsync(); var user1 = new User
            {
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test1@gmail.com"
            };
            var user2 = new User
            {
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test2@gmail.com"
            };
            var user3 = new User
            {
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test3@gmail.com"
            };
            _context.User.Add(user1);
            _context.User.Add(user2);
            _context.User.Add(user3);
            await _context.SaveChangesAsync();
            var accounts = new List<Account>
            {
                new Account { Id = 1, Email = "test1@example.com", Role = "User", Password = "password1", UserId = 1, User = user1 },
                new Account { Id = 2, Email = "test2@example.com", Role = "Admin", Password = "password2", UserId = 2, User = user2 },
                new Account { Id = 3, Email = "test3@example.com", Role = "User", Password = "password3", UserId = 3, User = user3 }
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