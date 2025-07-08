using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SaloonOS.Domain.Booking.Entities;
using SaloonOS.Domain.TenantManagement.Entities;

namespace SaloonOS.Infrastructure.Persistence.DbContext;

public class SaloonOSDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public SaloonOSDbContext(DbContextOptions<SaloonOSDbContext> options) : base(options) { }

    // Register our domain entities as DbSets
    public System.Data.Entity.DbSet<Shop> Shops { get; set; }
    public System.Data.Entity.DbSet<Service> Services { get; set; }
    public System.Data.Entity.DbSet<ServiceTranslation> ServiceTranslations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This line scans the assembly for all IEntityTypeConfiguration classes and applies them.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}