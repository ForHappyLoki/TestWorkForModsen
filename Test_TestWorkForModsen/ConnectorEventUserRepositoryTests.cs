using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWork_Events.Data;
using TestWork_Events.Models;
using TestWork_Events.Repository;
using Xunit;

namespace Test_TestWorkForModsen
{
    public class ConnectorEventUserRepositoryTests
    {
        private readonly DbContextOptions<DatabaseContext> _options;

        private User user;
        private Account account;

        public ConnectorEventUserRepositoryTests()
        {
            // Используем In-Memory Database для тестов
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя базы данных для каждого теста
                .Options;

            user = new User
            {
                Id = 1,
                Name = "Test",
                Surname = "Test",
                Birthday = new DateOnly(2000, 1, 1),
                Email = "Test@gmail.com"
            };

            account = new Account
            {
                Id = 1,
                Email = "Test@gmail.com",
                Role = "User",
                Password = "Test",
                UserId = 1,
                User = user
            };
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllConnectorEventUsers()
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
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var connector = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user };

                context.User.Add(user);
                context.Event.Add(event1);
                context.ConnectorEventUser.Add(connector);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                var result = await repository.GetAllAsync();

                // Assert
                Assert.Single(result); // Проверяем, что возвращена одна запись
                var firstResult = result.First();
                Assert.Equal(1, firstResult.EventId);
                Assert.Equal(1, firstResult.UserId);
                Assert.NotNull(firstResult.Event);
                Assert.NotNull(firstResult.User);
            }
        }

        [Fact]
        public async Task GetByCompositeKeyAsync_ReturnsConnector_WhenExists()
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
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var connector = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user };

                context.User.Add(user);
                context.Event.Add(event1);
                context.ConnectorEventUser.Add(connector);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                var result = await repository.GetByCompositeKeyAsync(1, 1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1, result.EventId);
                Assert.Equal(1, result.UserId);
                Assert.NotNull(result.Event);
                Assert.NotNull(result.User);
            }
        }

        [Fact]
        public async Task GetByCompositeKeyAsync_ReturnsNull_WhenDoesNotExist()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                var result = await repository.GetByCompositeKeyAsync(999, 999);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllByUserIdAsync_ReturnsConnectors_ForUser()
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
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var event2 = new Event
                {
                    Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    DateTime = DateTime.UtcNow,
                    Category = "Category2",
                    MaxParticipants = "200",
                    Image = new byte[] { 0x04, 0x05, 0x06 } // Пример массива байтов
                };
                var connector1 = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user };
                var connector2 = new ConnectorEventUser { EventId = 2, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event2, User = user };

                context.User.Add(user);
                context.Event.AddRange(event1, event2);
                context.ConnectorEventUser.AddRange(connector1, connector2);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                var result = await repository.GetAllByUserIdAsync(1);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, ceu => Assert.Equal(1, ceu.UserId));
            }
        }

        [Fact]
        public async Task GetAllByEventIdAsync_ReturnsConnectors_ForEvent()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user1 = new User { Id = 1, Name = "User1", Surname = "Test", Email = "user1@example.com", Birthday = new DateOnly(2000, 1, 1) };
                var user2 = new User { Id = 2, Name = "User2", Surname = "Test", Email = "user2@example.com", Birthday = new DateOnly(2000, 1, 1) };
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var connector1 = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user1 };
                var connector2 = new ConnectorEventUser { EventId = 1, UserId = 2, AdditionTime = DateTime.UtcNow, Event = event1, User = user2 };

                context.User.AddRange(user1, user2);
                context.Event.Add(event1);
                context.ConnectorEventUser.AddRange(connector1, connector2);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                var result = await repository.GetAllByEventIdAsync(1);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, ceu => Assert.Equal(1, ceu.EventId));
            }
        }

        [Fact]
        public async Task AddAsync_AddsConnectorToDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user = new User { Id = 1, Name = "User1", Surname = "Test", Email = "user1@example.com", Birthday = new DateOnly(2000, 1, 1) };
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                context.User.Add(user);
                context.Event.Add(event1);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);
                var connector = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow };

                // Act
                await repository.AddAsync(connector);

                // Assert
                var result = await context.ConnectorEventUser.FirstOrDefaultAsync(ceu => ceu.EventId == 1 && ceu.UserId == 1);
                Assert.NotNull(result);
                Assert.Equal(1, result.EventId);
                Assert.Equal(1, result.UserId);
            }
        }

        [Fact]
        public async Task UpdateAsync_UpdatesConnectorInDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user = new User { Id = 1, Name = "User1", Surname = "Test", Email = "user1@example.com" };
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var connector = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user };

                context.User.Add(user);
                context.Event.Add(event1);
                context.ConnectorEventUser.Add(connector);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);
                var updatedConnector = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow.AddDays(1) };

                // Act
                await repository.UpdateAsync(updatedConnector);

                // Assert
                var result = await context.ConnectorEventUser.FirstOrDefaultAsync(ceu => ceu.EventId == 1 && ceu.UserId == 1);
                Assert.NotNull(result);
                Assert.Equal(DateTime.UtcNow.AddDays(1).Date, result.AdditionTime.Date);
            }
        }

        [Fact]
        public async Task DeleteByCompositeKeyAsync_RemovesConnectorFromDatabase()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user = new User { Id = 1, Name = "User1", Surname = "Test", Email = "user1@example.com" };
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var connector = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user };

                context.User.Add(user);
                context.Event.Add(event1);
                context.ConnectorEventUser.Add(connector);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                await repository.DeleteByCompositeKeyAsync(1, 1);

                // Assert
                var result = await context.ConnectorEventUser.FirstOrDefaultAsync(ceu => ceu.EventId == 1 && ceu.UserId == 1);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsPagedConnectors()
        {
            // Arrange
            using (var context = new DatabaseContext(_options))
            {
                var user = new User { Id = 1, Name = "User1", Surname = "Test", Email = "user1@example.com" };
                var event1 = new Event
                {
                    Id = 1,
                    Name = "Event1",
                    Description = "Description1",
                    DateTime = DateTime.UtcNow,
                    Category = "Category1",
                    MaxParticipants = "100",
                    Image = new byte[] { 0x01, 0x02, 0x03 } // Пример массива байтов
                };
                var event2 = new Event
                {
                    Id = 2,
                    Name = "Event2",
                    Description = "Description2",
                    DateTime = DateTime.UtcNow,
                    Category = "Category2",
                    MaxParticipants = "200",
                    Image = new byte[] { 0x04, 0x05, 0x06 } // Пример массива байтов
                };
                var connector1 = new ConnectorEventUser { EventId = 1, UserId = 1, AdditionTime = DateTime.UtcNow, Event = event1, User = user };
                var connector2 = new ConnectorEventUser { EventId = 2, UserId = 1, AdditionTime = DateTime.UtcNow.AddDays(1), Event = event2, User = user };

                context.User.Add(user);
                context.Event.AddRange(event1, event2);
                context.ConnectorEventUser.AddRange(connector1, connector2);
                context.SaveChanges();
            }

            using (var context = new DatabaseContext(_options))
            {
                var repository = new ConnectorEventUserRepository(context);

                // Act
                var result = await repository.GetPagedAsync(1, 1);

                // Assert
                Assert.Single(result);
                var firstResult = result.First();
                Assert.Equal(1, firstResult.EventId);
                Assert.Equal(1, firstResult.UserId);
            }
        }
    }
}