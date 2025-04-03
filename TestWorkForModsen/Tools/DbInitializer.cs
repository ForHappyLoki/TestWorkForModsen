using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using TestWorkForModsen.Data;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Tools
{
    public static class DbInitializer
    {
        public static void Initialize(DatabaseContext context, IPasswordHasher<User> passwordHasher)
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
            var events = GenerateEvents(100);
            context.Event.AddRange(events);
            context.SaveChanges();

            // Генерация пользователей (User)
            var users = GenerateUsers(100);
            context.User.AddRange(users);
            context.SaveChanges();

            // Генерация аккаунтов (Account)
            var accounts = GenerateAccounts(users, passwordHasher);
            context.Account.AddRange(accounts);
            context.SaveChanges();

            // Генерация связей
            var connectors = GenerateEventUserConnectors(events, users);
            context.ConnectorEventUser.AddRange(connectors);
            context.SaveChanges();
        }

        private static List<Event> GenerateEvents(int count)
        {
            var events = new List<Event>();
            var random = new Random();

            for (int i = 1; i <= count; i++)
            {
                events.Add(new Event
                {
                    Name = $"Событие {i}",
                    Description = $"Описание события {i}",
                    DateTime = DateTime.UtcNow.AddDays(i),
                    Category = GetRandomCategory(i),
                    MaxParticipants = (i * 10).ToString(),
                    Image = GenerateRandomImageBytes()
                });
            }
            return events;
        }

        private static string GetRandomCategory(int index)
        {
            return index % 4 == 0 ? "Спорт" :
                   index % 3 == 0 ? "Музыка" :
                   index % 2 == 0 ? "Образование" : "Бизнес";
        }

        private static List<User> GenerateUsers(int count)
        {
            var users = new List<User>();

            for (int i = 1; i <= count; i++)
            {
                users.Add(new User
                {
                    Name = $"Имя_{i}",
                    Surname = $"Фамилия_{i}",
                    Birthday = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20).AddDays(i)),
                    Email = $"user{i}@example.com"
                });
            }
            return users;
        }

        private static List<Account> GenerateAccounts(List<User> users, IPasswordHasher<User> passwordHasher)
        {
            var accounts = new List<Account>();
            var random = new Random();

            foreach (var user in users)
            {
                var account = new Account
                {
                    Email = user.Email,
                    Role = accounts.Count == 0 ? "Admin" : (random.Next(2) == 0 ? "User" : "Admin"),
                    UserId = user.Id,
                    User = user
                };
                account.Password = passwordHasher.HashPassword(user, $"password{user.Id}");

                accounts.Add(account);
            }
            return accounts;
        }

        private static List<ConnectorEventUser> GenerateEventUserConnectors(List<Event> events, List<User> users)
        {
            var connectors = new List<ConnectorEventUser>();
            var random = new Random();

            foreach (var ev in events)
            {
                int participantCount = random.Next(1, 11);
                var selectedUsers = users.OrderBy(u => random.Next()).Take(participantCount).ToList();

                foreach (var user in selectedUsers)
                {
                    if (!connectors.Any(ceu => ceu.EventId == ev.Id && ceu.UserId == user.Id))
                    {
                        connectors.Add(new ConnectorEventUser
                        {
                            EventId = ev.Id,
                            UserId = user.Id,
                            AdditionTime = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                        });
                    }
                }
            }
            return connectors;
        }

        private static byte[] GenerateRandomImageBytes()
        {
            var random = new Random();
            var bytes = new byte[200 * 300 * 3]; // Размер изображения 200x300px (RGB)
            random.NextBytes(bytes);
            return bytes;
        }
    }
}