using EmployeeRegistration.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRegistration.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<State> States => Set<State>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(e =>
        {
            e.ToTable("Country_Mst");
            e.HasKey(x => x.CountryId);
            e.Property(x => x.CountryName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<State>(e =>
        {
            e.ToTable("State_Mst");
            e.HasKey(x => x.StateId);
            e.Property(x => x.StateName).HasMaxLength(100).IsRequired();

            e.HasOne(x => x.Country)
             .WithMany(x => x.States)
             .HasForeignKey(x => x.CountryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.ToTable("Employee_Mst");
            e.HasKey(x => x.EmployeeId);

            e.Property(x => x.EmployeeId).ValueGeneratedOnAdd();
            e.Property(x => x.EmployeeName).HasMaxLength(30).IsRequired();
            e.Property(x => x.MobileNum).HasMaxLength(10).IsRequired();
            e.Property(x => x.Pincode).HasMaxLength(6).IsRequired();
            e.Property(x => x.DOB).HasColumnType("datetime2").IsRequired(false);
            e.Property(x => x.AddressLine1).HasMaxLength(250).IsRequired();
            e.Property(x => x.AddressLine2).HasMaxLength(250).IsRequired(false);

            e.HasIndex(x => x.MobileNum).IsUnique();

            e.HasOne(x => x.State)
             .WithMany(x => x.Employees)
             .HasForeignKey(x => x.StateId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Country)
             .WithMany(x => x.Employees)
             .HasForeignKey(x => x.CountryId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
