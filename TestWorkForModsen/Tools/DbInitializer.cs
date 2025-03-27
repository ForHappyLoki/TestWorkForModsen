using System;
using System.Collections.Generic;
using System.Linq;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Tools
{
    public static class DbInitializer
    {
        public static void Initialize(DatabaseContext context)
        {
            try
            {
                if (context.User.Any() || context.Account.Any() || context.Event.Any() || context.ConnectorEventUser.Any())
                {
                    Console.WriteLine("База данных уже инициализирована.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке базы данных: {ex.Message}");
                throw;
            }

            // Генерация событий (Event)
            var events = new List<Event>();
            for (int i = 1; i <= 100; i++)
            {
                events.Add(new Event
                {
                    Name = $"Событие {i}",
                    Description = $"Описание события {i}",
                    DateTime = DateTime.UtcNow.AddDays(i),
                    Category = i % 4 == 0 ? "Спорт" : i % 3 == 0 ? "Музыка" : i % 2 == 0 ? "Образование" : "Бизнес",
                    MaxParticipants = (i * 10).ToString(),
                    Image = GenerateRandomImageBytes() // Генерация случайных байтов для изображения
                });
            }
            context.Event.AddRange(events);
            context.SaveChanges();

            // Генерация пользователей (User)
            var users = new List<User>();
            for (int i = 1; i <= 100; i++)
            {
                users.Add(new User
                {
                    Name = $"Имя_{i}",
                    Surname = $"Фамилия_{i}",
                    Birthday = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20).AddDays(i)),
                    Email = $"user{i}@example.com"
                });
            }
            context.User.AddRange(users);
            context.SaveChanges();

            // Генерация аккаунтов (Account)
            var accounts = new List<Account>();
            var random = new Random();
            foreach (var user in users)
            {
                accounts.Add(new Account
                {
                    Email = user.Email,
                    Role = random.Next(2) == 0 ? "User" : "Admin",
                    Password = $"password{user.Id}",
                    UserId = user.Id
                });
            }
            context.Account.AddRange(accounts);
            context.SaveChanges();

            // Генерация связей между событиями и пользователями (ConnectorEventUser)
            var connectorEventUsers = new List<ConnectorEventUser>();
            foreach (var ev in events)
            {
                // Случайное количество участников для каждого события (от 1 до 10)
                int participantCount = random.Next(1, 11);

                // Выбор случайных пользователей для участия в событии
                var selectedUsers = users.OrderBy(u => random.Next()).Take(participantCount).ToList();

                foreach (var user in selectedUsers)
                {
                    // Проверка, чтобы избежать дубликатов связей
                    if (!connectorEventUsers.Any(ceu => ceu.EventId == ev.Id && ceu.UserId == user.Id))
                    {
                        connectorEventUsers.Add(new ConnectorEventUser
                        {
                            EventId = ev.Id,
                            UserId = user.Id,
                            AdditionTime = DateTime.UtcNow.AddDays(-random.Next(1, 30)) // Случайное время добавления
                        });
                    }
                }
            }
            context.ConnectorEventUser.AddRange(connectorEventUsers);
            context.SaveChanges();
        }

        // Генерация случайных байтов для имитации изображения
        private static byte[] GenerateRandomImageBytes()
        {
            var random = new Random();
            var width = 200;
            var height = 300;
            var bytesPerPixel = 3;
            var imageSize = width * height * bytesPerPixel;

            var imageBytes = new byte[imageSize];
            random.NextBytes(imageBytes); // Заполнение массива случайными байтами

            return imageBytes;
        }
    }
}