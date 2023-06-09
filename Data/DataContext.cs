using Microsoft.EntityFrameworkCore;
using PriseChecker.Models;

namespace PriseChecker.Data
{

    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=price_checker.db");
        }
    }
}