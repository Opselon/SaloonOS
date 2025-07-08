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
    public Microsoft.EntityFrameworkCore.DbSet<Shop> Shops { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<Service> Services { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<ServiceTranslation> ServiceTranslations { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<StaffMember> StaffMembers { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<Appointment> Appointments { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<Customer> Customers { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<WorkSchedule> WorkSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This line scans the assembly for all IEntityTypeConfiguration classes and applies them.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}