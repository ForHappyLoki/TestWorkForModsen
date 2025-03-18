using Microsoft.EntityFrameworkCore;
using System;
using TestWork_Events.Models;

namespace TestWork_Events.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ConnectorEventUser> ConnectorEventUser { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка сущности ConnectorEventUser
            modelBuilder.Entity<ConnectorEventUser>()
                .HasKey(ceu => new { ceu.EventId, ceu.UserId }); 

            // Настройка связей для ConnectorEventUser
            modelBuilder.Entity<ConnectorEventUser>()
                .HasOne(ceu => ceu.Event)
                .WithMany(e => e.ConnectorEventUser)
                .HasForeignKey(ceu => ceu.EventId);

            modelBuilder.Entity<ConnectorEventUser>()
                .HasOne(ceu => ceu.User)
                .WithMany(u => u.ConnectorEventUser)
                .HasForeignKey(ceu => ceu.UserId);

            // Настройка сущности Account
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Account>(a => a.UserId);

            // Настройка сущности RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Account)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(rt => rt.AccountId);

            // Настройка всех строковых свойств на тип "varchar"
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        property.SetColumnType("varchar");
                    }
                }
            }
        }
    }
}