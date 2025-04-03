using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Repository;
using Xunit;

namespace Test_TestWorkForModsen
{
    public class EventRepositoryTests
    {
        private readonly DbContextOptions<DatabaseContext> _options;

        public EventRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEvents()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 }
                };
                var event2 = new Event
                {
                    Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    DateTime = DateTime.UtcNow,
                    Category = "Category2",
                    MaxParticipants = "200",
                    Image = new byte[] { 0x04, 0x05, 0x06 }
                };
                context.Event.AddRange(event1, event2);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new EventRepository(context);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEvent_WhenExists()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 }
                };
                context.Event.Add(event1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new EventRepository(context);

                // Act
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Event1", result.Name);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEventDoesNotExist()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new EventRepository(context);

                // Act
                var result = await repository.GetByIdAsync(999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_AddsEventToDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new EventRepository(context);
                var newEvent = new Event
                {
                    Id = 1,
                    Name = "NewEvent",
                    Description = "NewDescription",
                    DateTime = DateTime.UtcNow,
                    Category = "NewCategory",
                    MaxParticipants = "300",
                    Image = new byte[] { 0x07, 0x08, 0x09 }
                };

                // Act
                await repository.AddAsync(newEvent);

                // Assert
                var result = await context.Event.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("NewEvent", result.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEventInDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 }
                };
                context.Event.Add(event1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new EventRepository(context);
                var updatedEvent = new Event
                {
                    Id = 1,
                    Name = "UpdatedEvent",
                    Description = "UpdatedDescription",
                    DateTime = DateTime.UtcNow,
                    Category = "UpdatedCategory",
                    MaxParticipants = "200",
                    Image = new byte[] { 0x04, 0x05, 0x06 }
                };

                // Act
                await repository.UpdateAsync(updatedEvent);

                // Assert
                var result = await context.Event.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("UpdatedEvent", result.Name);
                Assert.Equal("UpdatedDescription", result.Description);
            }
        }
        [Fact]
        public async Task GetPagedAsync_ReturnsPagedEvents()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 }
                };
                var event2 = new Event
                {
                    Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    DateTime = DateTime.UtcNow,
                    Category = "Category2",
                    MaxParticipants = "200",
                    Image = new byte[] { 0x04, 0x05, 0x06 }
                };
                var event3 = new Event
                {
                    Id = 3,
                    Name = "Event3",
                    Description = "Description3",
                    DateTime = DateTime.UtcNow,
                    Category = "Category3",
                    MaxParticipants = "300",
                    Image = new byte[] { 0x07, 0x08, 0x09 }
                };
                context.Event.AddRange(event1, event2, event3);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new EventRepository(context);

                // Act
                var result = await repository.GetPagedAsync(1, 2);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Equal("Event1", result.First().Name);
                Assert.Equal("Event2", result.Last().Name);
            }
        }
    }
}