using Microsoft.EntityFrameworkCore;
using System;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Models;

namespace TestWorkForModsen.Data.Data
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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ConnectorEventUserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
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