using System.Collections.Generic;
using JustBugs.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace JustBugs.Database
{
    public class JustBugsContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=95.111.238.69;Database=JustBugs;Username=root;Password=root");
            base.OnConfiguring(optionsBuilder);
        }
    }
}