using LeaveManagementSystem.Enums;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    FullName = "Sarah Messaoudi",
                    Department = "RH",
                    JoiningDate = new DateTime(2020, 5, 1)
                },
                new Employee
                {
                    Id = 2,
                    FullName = "Yassine Ben Ali",
                    Department = "IT",
                    JoiningDate = new DateTime(2019, 3, 15)
                }
            );

            modelBuilder.Entity<LeaveRequest>().HasData(
                new LeaveRequest
                {
                    Id = 1,
                    EmployeeId = 1,
                    LeaveType = LeaveType.Annual,
                    Status = LeaveStatus.Pending,
                    StartDate = new DateTime(2025, 5, 10),
                    EndDate = new DateTime(2025, 5, 15),
                    Reason = "Vacances en famille",
                    CreatedAt = DateTime.Now
                }
            );
        }


    }
}
