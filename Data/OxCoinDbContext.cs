using System.Linq;
using Microsoft.EntityFrameworkCore;
using OxCoin.TransactionGenerator.Data.Entities;

namespace OxCoin.TransactionGenerator.Data
{
    public class OxCoinDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=OxCoin.TransactionGenerator;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                                                       .SelectMany(t => t.GetProperties())
                                                       .Where(p => p.ClrType == typeof(decimal)))
            {
                property.Relational().ColumnType = "decimal(18, 8)";
            }
        }
    }
}
