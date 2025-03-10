using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWork_Events.Data;
using TestWork_Events.Models;
using TestWork_Events.Repository;

namespace Test_TestWorkForModsen
{
    public class EventApiTests
    {
        [Fact]
        public void Event_Should_Set_Properties_Correctly()
        {
            // Arrange
            var testEvent = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                DateTime = DateTime.Now,
                Category = "Test Category",
                MaxParticipants = "100",
                Image = new byte[] { 0x01, 0x02, 0x03 }
            };

            // Assert
            Assert.Equal(1, testEvent.Id);
            Assert.Equal("Test Event", testEvent.Name);
            Assert.Equal("Test Description", testEvent.Description);
            Assert.Equal("Test Category", testEvent.Category);
            Assert.Equal("100", testEvent.MaxParticipants);
            Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, testEvent.Image);
        }
    }

    public class EventRepositoryTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Event>>();
            var testEvent = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                DateTime = DateTime.Now,
                Category = "Test Category",
                MaxParticipants = "100",
                Image = new byte[] { 0x01, 0x02, 0x03 }
            };
            mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(testEvent);

            // Act
            var result = await mockRepository.Object.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testEvent.Id, result.Id);
            Assert.Equal(testEvent.Name, result.Name);
            Assert.Equal(testEvent.Description, result.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Event)null);

            // Act
            var result = await mockRepository.Object.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEvent_WhenEventIsValid()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Event>>();
            var testEvent = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                DateTime = DateTime.Now,
                Category = "Test Category",
                MaxParticipants = "100",
                Image = new byte[] { 0x01, 0x02, 0x03 }
            };

            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Event>()))
                .Callback<Event>(e =>
                {
                    Assert.Equal(testEvent.Id, e.Id);
                    Assert.Equal(testEvent.Name, e.Name);
                    Assert.Equal(testEvent.Description, e.Description);
                })
                .Returns(Task.CompletedTask);

            // Act
            await mockRepository.Object.AddAsync(testEvent);

            // Assert
            mockRepository.Verify(repo => repo.AddAsync(testEvent), Times.Once);
        }
    }
}