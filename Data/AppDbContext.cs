using Microsoft.EntityFrameworkCore;
using Agreement.Models;

namespace Agreement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AgreementRecord> Agreements { get; set; }
    }
}