using Microsoft.EntityFrameworkCore;
using StaffManagement.SharedLib.Models;

namespace StaffManagement.APP.Logic.DBContext
{
    /// <summary>
    /// Entity Framework DbContext for Staff Management with SQLite database
    /// </summary>
    public class StaffManagementDBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the StaffManagementDBContext
        /// </summary>
        /// <param name="options">Database context options</param>
        public StaffManagementDBContext(DbContextOptions<StaffManagementDBContext> options) : base(options)
        {
        }

        /// <summary>
        /// Staff entity DbSet
        /// </summary>
        public DbSet<Staff> Staff { get; set; }

        /// <summary>
        /// Configures the database schema and seeds initial data
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Staff entity
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Salary).HasPrecision(18, 2);
                entity.Property(e => e.HireDate).IsRequired();
                entity.Property(e => e.IsActive);
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Seeds initial data for testing and demonstration
        /// </summary>
        /// <param name="modelBuilder">Model builder instance</param>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            var staffMembers = new List<Staff>
            {
                new()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@company.com",
                    Phone = "+1-555-0101",
                    Department = "Engineering",
                    Position = "Senior Software Engineer",
                    Salary = 95000,
                    HireDate = new DateTime(2020, 1, 15),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@company.com",
                    Phone = "+1-555-0102",
                    Department = "Human Resources",
                    Position = "HR Manager",
                    Salary = 75000,
                    HireDate = new DateTime(2019, 6, 10),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 3,
                    FirstName = "Michael",
                    LastName = "Johnson",
                    Email = "michael.johnson@company.com",
                    Phone = "+1-555-0103",
                    Department = "Finance",
                    Position = "Financial Analyst",
                    Salary = 65000,
                    HireDate = new DateTime(2021, 3, 22),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 4,
                    FirstName = "Sarah",
                    LastName = "Williams",
                    Email = "sarah.williams@company.com",
                    Phone = "+1-555-0104",
                    Department = "Marketing",
                    Position = "Marketing Specialist",
                    Salary = 58000,
                    HireDate = new DateTime(2022, 8, 5),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 5,
                    FirstName = "David",
                    LastName = "Brown",
                    Email = "david.brown@company.com",
                    Phone = "+1-555-0105",
                    Department = "Engineering",
                    Position = "DevOps Engineer",
                    Salary = 88000,
                    HireDate = new DateTime(2021, 11, 12),
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<Staff>().HasData(staffMembers);
        }
    }
}
