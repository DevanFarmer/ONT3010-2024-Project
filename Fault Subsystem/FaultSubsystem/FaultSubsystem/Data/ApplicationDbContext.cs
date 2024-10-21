using FaultSubsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FaultSubsystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<FaultReport> FaultReport { get; set; }
        public DbSet<FaultStatus> FaultStatus { get; set; }
        public DbSet<Fridge> Fridge { get; set; }
        public DbSet<FridgeAllocation> FridgeAllocation { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");         // Table Name
                entity.HasKey(c => c.CustomerID);   // Primary Key
                // Columns
                entity.Property(c => c.UserID)
                .HasMaxLength(100)
                .IsRequired();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");         // Table Name
                entity.HasKey(e => e.EmployeeID);   // Primary Key
                // Columns
                entity.Property(e => e.UserID)
                .HasMaxLength(100)
                .IsRequired();
                entity.Property(e => e.RoleID)
                .HasMaxLength(100)
                .IsRequired();
            });

            modelBuilder.Entity<FaultReport>(entity =>
            {
                entity.ToTable("FaultReport");         // Table Name
                entity.HasKey(f => f.FaultID);   // Primary Key
                // Columns
                entity.Property(f => f.AllocationID)
                .IsRequired();
                entity.Property(f => f.EmployeeID);
                entity.Property(f => f.FaultStatusID)
                .IsRequired();
                entity.Property(f => f.FaultDescription)
                .HasMaxLength(255)
                .IsRequired();
                entity.Property(f => f.ReportDate)
                .IsRequired();
                entity.Property(f => f.ResolutionDate);
                entity.Property(f => f.AssignedDate);
                entity.Property(f => f.ScheduledRepairDate);
                entity.Property(f => f.Diagnosis)
                .HasMaxLength(255);
                entity.Property(f => f.Notes)
                .HasMaxLength(255);
            });

            modelBuilder.Entity<FaultStatus>(entity =>
            {
                entity.ToTable("FaultStatus");         // Table Name
                entity.HasKey(f => f.FaultStatusID);   // Primary Key
                // Columns
                entity.Property(f => f.StatusName)
                .HasMaxLength(100)
                .IsRequired();
            });

            modelBuilder.Entity<Fridge>(entity =>
            {
                entity.ToTable("Fridge");         // Table Name
                entity.HasKey(f => f.FridgeID);   // Primary Key
                // Columns
                entity.Property(f => f.FridgeTypeID)
                .IsRequired();
                entity.Property(f => f.StatusID)
                .IsRequired();
                entity.Property(f => f.LocationID)
                .IsRequired();
                entity.Property(f => f.SerialNumber)
                .HasMaxLength(50)
                .IsRequired();
                entity.Property(f => f.DateAcquired)
                .IsRequired();
            });

            modelBuilder.Entity<FridgeAllocation>(entity =>
            {
                entity.ToTable("FridgeAllocation");         // Table Name
                entity.HasKey(f => f.AllocationID);   // Primary Key
                // Columns
                entity.Property(f => f.FridgeID)
                .HasMaxLength(100)
                .IsRequired();
                entity.Property(f => f.CustomerID)
                .HasMaxLength(100)
                .IsRequired();
                entity.Property(f => f.AllocationDate)
                .IsRequired();
                entity.Property(f => f.ReturnDate);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("Inventory");         // Table Name
                entity.HasKey(f => f.FridgeTypeID);   // Primary Key
                // Columns
                entity.Property(f => f.SupplierID)
                .IsRequired();
                entity.Property(f => f.FridgeModel)
                .HasMaxLength(50)
                .IsRequired();
                entity.Property(f => f.Quantity)
                .IsRequired();
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");         // Table Name
                entity.HasKey(l => l.LocationID);   // Primary Key
                // Columns
                entity.Property(l => l.AddressLine1)
                .HasMaxLength(100)
                .IsRequired();
                entity.Property(l => l.AddressLine2)
                .HasMaxLength(100);
                entity.Property(l => l.City)
                .HasMaxLength(100)
                .IsRequired();
                entity.Property(l => l.PostalCode)
                .HasMaxLength(100);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");         // Table Name
                entity.HasKey(r => r.RoleID);   // Primary Key
                // Columns
                entity.Property(r => r.RoleName)
                .HasMaxLength(100)
                .IsRequired();
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("FridgeStatus");         // Table Name
                entity.HasKey(s => s.FridgeStatusID);   // Primary Key
                // Columns
                entity.Property(s => s.StatusName)
                .HasMaxLength(100)
                .IsRequired();
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");         // Table Name
                entity.HasKey(s => s.SupplierID);   // Primary Key
                // Columns
                entity.Property(s => s.SupplierName)
                .HasMaxLength(50)
                .IsRequired();
                entity.Property(s => s.SupplierPhoneNumber)
                .HasMaxLength(50)
                .IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");         // Table Name
                entity.HasKey(u => u.UserID);   // Primary Key
                // Columns
                entity.Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();
                entity.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();
                entity.Property(u => u.Password)
                .HasMaxLength(100)
                .IsRequired();
                entity.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired();
                entity.Property(u => u.PhoneNumber)
                .HasMaxLength(50);
                entity.Property(u => u.AccountActive)
                .IsRequired();
            });

            // Relationships
            modelBuilder.Entity<FaultReport>()
                .HasOne(fr => fr.FridgeAllocation)
                .WithMany(fa => fa.FaultReport)
                .HasForeignKey(fr => fr.AllocationID);

            modelBuilder.Entity<Fridge>()
                .HasOne(f => f.Inventory)
                .WithMany(i => i.Fridge)
                .HasForeignKey(f => f.FridgeTypeID);
            //modelBuilder.Entity<Customer>()
            //    .HasOne(c => c.User)
            //    .WithOne(u => u.Customer)
            //    .HasForeignKey<User>(u => u.UserID);

            //modelBuilder.Entity<Employee>()
            //    .HasOne(e => e.User)
            //    .WithOne(u => u.Employee)
            //    .HasForeignKey<User>(u => u.UserID);
            //modelBuilder.Entity<Employee>()
            //    .HasOne(e => e.Role)
            //    .WithMany(r => r.Employees)
            //    .HasForeignKey(e => e.RoleID);

            //modelBuilder.Entity<Employee>()
            //    .HasOne(e => e.User)
            //    .WithOne(u => u.Employee)
            //    .HasForeignKey<User>(u => u.UserID);
            //modelBuilder.Entity<Employee>()
            //    .HasOne(e => e.Role)
            //    .WithMany(r => r.Employees)
            //    .HasForeignKey(e => e.RoleID);
        }
    }
}
