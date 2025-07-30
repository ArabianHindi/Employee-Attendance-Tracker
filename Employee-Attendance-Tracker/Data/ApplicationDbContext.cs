using Employee_Attendance_Tracker.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Employee_Attendance_Tracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(d => d.Code)
                    .IsRequired()
                    .HasMaxLength(4);
                entity.Property(d => d.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                // Unique constraints
                entity.HasIndex(d => d.Name).IsUnique();
                entity.HasIndex(d => d.Code).IsUnique();
            });

            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeCode)
                    .IsRequired();
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // Unique constraints
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Foreign key relationship
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attendance configuration
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Date)
                    .IsRequired();
                entity.Property(a => a.IsPresent)
                    .IsRequired();

                // Unique constraint for employee-date combination
                entity.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();

                // Foreign key relationship
                entity.HasOne(a => a.Employee)
                    .WithMany(e => e.Attendances)
                    .HasForeignKey(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
