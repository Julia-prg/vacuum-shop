using Microsoft.EntityFrameworkCore;
using VacuumPro.Api.Models;
namespace VacuumPro.Api.Data;
public class VacuumDbContext : DbContext
{
    public VacuumDbContext(DbContextOptions<VacuumDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vacuum> Vacuums => Set<Vacuum>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vacuum>()
            .Property(v => v.Price)
            .HasPrecision(10, 2);
    }
}