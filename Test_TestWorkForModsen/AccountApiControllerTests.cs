using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Principal;
using TestWork_Events.Controllers;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace Test_TestWorkForModsen
{
    public class AccountApiControllerTests
    {
        private readonly Mock<IRepository<Account>> _mockRepo;
        private readonly AccountApiController _controller;

        public AccountApiControllerTests()
        {
            _mockRepo = new Mock<IRepository<Account>>();
            _controller = new AccountApiController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsAllAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
            new Account { Id = 1, Email = "test1@example.com" },
            new Account { Id = 2, Email = "test2@example.com" }
        };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(accounts);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAccounts = Assert.IsType<List<Account>>(okResult.Value);
            Assert.Equal(2, returnedAccounts.Count);
        }

        [Fact]
        public async Task GetById_ReturnsAccount_WhenAccountExists()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test@example.com" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(account);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(1, returnedAccount.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Account)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetByEmail_ReturnsAccount_WhenAccountExists()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test@example.com" };
            _mockRepo.Setup(repo => repo.GetByEmailAsync("test@example.com")).ReturnsAsync(account);

            // Act
            var result = await _controller.GetByEmail("test@example.com");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal("test@example.com", returnedAccount.Email);
        }

        [Fact]
        public async Task GetByEmail_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByEmailAsync("nonexistent@example.com")).ReturnsAsync((Account)null);

            // Act
            var result = await _controller.GetByEmail("nonexistent@example.com");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenAccountIsCreated()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test@example.com" };
            _mockRepo.Setup(repo => repo.AddAsync(account)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(account);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetById", createdAtActionResult.ActionName);
            Assert.Equal(1, ((Account)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenAccountIsUpdated()
        {
            // Arrange
            var account = new Account { Id = 1, Email = "test@example.com" };
            _mockRepo.Setup(repo => repo.UpdateAsync(account)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, account);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var account = new Account { Id = 2, Email = "test@example.com" };

            // Act
            var result = await _controller.Update(1, account);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAccountIsDeleted()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetPaged_ReturnsPagedAccounts_WhenPageNumberAndSizeAreValid()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = 1, Email = "test1@example.com" },
                new Account { Id = 2, Email = "test2@example.com" },
                new Account { Id = 3, Email = "test3@example.com" }
            };
            _mockRepo.Setup(repo => repo.GetPagedAsync(1, 2)).ReturnsAsync(accounts.Take(2).ToList());

            // Act
            var result = await _controller.GetPaged(1, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAccounts = Assert.IsType<List<Account>>(okResult.Value);
            Assert.Equal(2, returnedAccounts.Count);
        }

        [Fact]
        public async Task GetPaged_ReturnsBadRequest_WhenPageNumberOrSizeIsInvalid()
        {
            // Act
            var result = await _controller.GetPaged(0, 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}