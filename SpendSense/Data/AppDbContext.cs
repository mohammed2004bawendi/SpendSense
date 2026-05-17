using Microsoft.EntityFrameworkCore;
using SpendSense.Models;

namespace SpendSense.Data
{
    public class AppDbContext : DbContext
    {
        // Passes EF Core options (connection string, provider) to the base DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Account> Accounts { get; set; }

        // Configures the Transaction -> Account relationship to use NoAction on delete,
        // preventing cascade deletes that would remove transactions when an account is deleted
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
