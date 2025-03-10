using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWork_Events.Data;
using TestWork_Events.Models;
using TestWork_Events.Repository;
using System.Threading.Tasks;
using Xunit;

namespace Test_TestWorkForModsen
{
    public class AccountTests
    {
        private readonly DatabaseContext _context;
        private readonly IRepository<Account> _repository;

        public AccountTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new DatabaseContext(options);
            _repository = new AccountRepository(_context);
        }

        [Fact]
        public async Task GetAccountByEmailAsync_ShouldReturnAccount_WhenAccountExists()
        {
            // Arrange
            var account = new Account { Email = "test@example.com", Password = "password", Role = "User", UserId = 1 };
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetAccountByEmailAsync_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            // Act
            var result = await _repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveAccountAsync_ShouldSaveAccount()
        {
            // Arrange
            var account = new Account { Email = "new@example.com", Password = "password", Role = "User", UserId = 2 };

            // Act
            await _repository.AddAsync(account);

            // Assert
            var savedAccount = await _context.Account.FirstOrDefaultAsync(a => a.Email == "new@example.com");
            Assert.NotNull(savedAccount);
            Assert.Equal("new@example.com", savedAccount.Email);
        }
    }
}
