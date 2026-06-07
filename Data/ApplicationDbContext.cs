using EmployeeLeaveManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Data;

public class ApplicationDbContext: DbContext 
{ 
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Employee> Employees { get; set; }

         public DbSet<Leave> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId);

            base.OnModelCreating(modelBuilder);
        }
    }

