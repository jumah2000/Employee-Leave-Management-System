using EmployeeLeaveManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Data;

public class ApplicationDbContext: DbContext 
{ 
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Employee> Employees { get; set; }

         public DbSet<Leave> Leaves { get; set; }
         
         public DbSet<LeaveApproval> LeaveApprovals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.Leaves)
                .HasForeignKey(l => l.EmployeeId);

            modelBuilder.Entity<Leave>()
                .HasMany(l => l.Approvals)
                .WithOne(a => a.Leave)
                .HasForeignKey(a => a.LeaveId);
        }
    }

