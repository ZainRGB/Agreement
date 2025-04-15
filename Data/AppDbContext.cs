using Microsoft.EntityFrameworkCore;
using Agreement.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Agreement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AgreementRecord> Agreements { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Ensure all DateTime properties are saved as UTC in PostgreSQL
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<UtcValueConverter>()
                .HaveColumnType("timestamp with time zone");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Add any custom model configurations here
            base.OnModelCreating(modelBuilder);
        }
    }

    // Helper class to enforce UTC conversion
    public class UtcValueConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcValueConverter() : base(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(), // Convert to UTC on save
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)) // Read as UTC
        { }
    }
}