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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConnectorEventUser>()
                .HasKey(e => new { e.EventId, e.UserId }); 

            modelBuilder.Entity<ConnectorEventUser>()
                .HasOne(e => e.Event)
                .WithMany(s => s.ConnectorEventUser)
                .HasForeignKey(e => e.EventId);

            modelBuilder.Entity<ConnectorEventUser>()
                .HasOne(e => e.User)
                .WithMany(c => c.ConnectorEventUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Account)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(rt => rt.AccountId);
        }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
