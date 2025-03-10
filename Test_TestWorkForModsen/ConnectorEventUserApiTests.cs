using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWork_Events.Controllers;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace Test_TestWorkForModsen
{
    public class ConnectorEventUserApiTests
    {
        private readonly Mock<IConnectorEventUserRepository<ConnectorEventUser>> _mockRepo;
        private readonly ConnectorEventUserController _controller;

        public ConnectorEventUserApiTests()
        {
            _mockRepo = new Mock<IConnectorEventUserRepository<ConnectorEventUser>>();
            _controller = new ConnectorEventUserController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsAllRecords()
        {
            // Arrange
            var records = new List<ConnectorEventUser>
        {
            new ConnectorEventUser { EventId = 1, UserId = 1 },
            new ConnectorEventUser { EventId = 2, UserId = 2 }
        };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(records);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecords = Assert.IsType<List<ConnectorEventUser>>(okResult.Value);
            Assert.Equal(2, returnedRecords.Count);
        }

        [Fact]
        public async Task GetByCompositeKey_ReturnsRecord_WhenRecordExists()
        {
            // Arrange
            var record = new ConnectorEventUser { EventId = 1, UserId = 1 };
            _mockRepo.Setup(repo => repo.GetByCompositeKeyAsync(1, 1)).ReturnsAsync(record);

            // Act
            var result = await _controller.GetByCompositeKey(1, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecord = Assert.IsType<ConnectorEventUser>(okResult.Value);
            Assert.Equal(1, returnedRecord.EventId);
            Assert.Equal(1, returnedRecord.UserId);
        }

        [Fact]
        public async Task GetByCompositeKey_ReturnsNotFound_WhenRecordDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByCompositeKeyAsync(1, 1)).ReturnsAsync((ConnectorEventUser)null);

            // Act
            var result = await _controller.GetByCompositeKey(1, 1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllByUserId_ReturnsRecords_WhenRecordsExist()
        {
            // Arrange
            var records = new List<ConnectorEventUser>
        {
            new ConnectorEventUser { EventId = 1, UserId = 1 },
            new ConnectorEventUser { EventId = 2, UserId = 1 }
        };
            _mockRepo.Setup(repo => repo.GetAllByUserIdAsync(1)).ReturnsAsync(records);

            // Act
            var result = await _controller.GetAllByUserId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecords = Assert.IsType<List<ConnectorEventUser>>(okResult.Value);
            Assert.Equal(2, returnedRecords.Count);
        }

        [Fact]
        public async Task GetAllByEventId_ReturnsRecords_WhenRecordsExist()
        {
            // Arrange
            var records = new List<ConnectorEventUser>
            {
                new ConnectorEventUser { EventId = 1, UserId = 1 },
                new ConnectorEventUser { EventId = 1, UserId = 2 }
            };
            _mockRepo.Setup(repo => repo.GetAllByEventIdAsync(1)).ReturnsAsync(records);

            // Act
            var result = await _controller.GetAllByEventId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecords = Assert.IsType<List<ConnectorEventUser>>(okResult.Value);
            Assert.Equal(2, returnedRecords.Count);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenRecordIsCreated()
        {
            // Arrange
            var record = new ConnectorEventUser { EventId = 1, UserId = 1 };
            _mockRepo.Setup(repo => repo.AddAsync(record)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(record);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetByCompositeKey", createdAtActionResult.ActionName);
            Assert.Equal(1, ((ConnectorEventUser)createdAtActionResult.Value).EventId);
            Assert.Equal(1, ((ConnectorEventUser)createdAtActionResult.Value).UserId);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenRecordIsUpdated()
        {
            // Arrange
            var record = new ConnectorEventUser { EventId = 1, UserId = 1 };
            _mockRepo.Setup(repo => repo.UpdateAsync(record)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, 1, record);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenCompositeKeyDoesNotMatch()
        {
            // Arrange
            var record = new ConnectorEventUser { EventId = 2, UserId = 2 };

            // Act
            var result = await _controller.Update(1, 1, record);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenRecordIsDeleted()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteByCompositeKeyAsync(1, 1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1, 1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetPaged_ReturnsPagedRecords_WhenPageNumberAndSizeAreValid()
        {
            // Arrange
            var records = new List<ConnectorEventUser>
            {
                new ConnectorEventUser { EventId = 1, UserId = 1 },
                new ConnectorEventUser { EventId = 2, UserId = 2 },
                new ConnectorEventUser { EventId = 3, UserId = 3 }
            };
            _mockRepo.Setup(repo => repo.GetPagedAsync(1, 2)).ReturnsAsync(records.Take(2).ToList());

            // Act
            var result = await _controller.GetPaged(1, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRecords = Assert.IsType<List<ConnectorEventUser>>(okResult.Value);
            Assert.Equal(2, returnedRecords.Count);
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
