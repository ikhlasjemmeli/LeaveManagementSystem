using LeaveManagementSystem.Data.Repositories.Interfaces;
using LeaveManagementSystem.Dtos;
using LeaveManagementSystem.Enums;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;


namespace LeaveManagementSystem.Data.Repositories.Implementations
{

    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Retrieve all leave requests
        public async Task<IEnumerable<LeaveRequest>> GetAllLeaveRequests()
        {
            return await _context.LeaveRequests.Include(lr => lr.Employee).ToListAsync();
        }

        // Apply filtering, sorting, and pagination to leave requests
        public async Task<IEnumerable<LeaveRequest>> FilterLeaveRequests(LeaveRequestFilterDto filter)
        {
            var query = _context.LeaveRequests.Include(l => l.Employee).AsQueryable();

            // Filter by employee id

            if (filter.EmployeeId.HasValue)
            {
                query = query.Where(l => l.EmployeeId == filter.EmployeeId);
            }

            // Filter by leave type

            if (!string.IsNullOrWhiteSpace(filter.LeaveType) &&
                Enum.TryParse<LeaveType>(filter.LeaveType, out var leaveType))
            {
                query = query.Where(l => l.LeaveType == leaveType);
            }

            // Filter by leave status

            if (!string.IsNullOrWhiteSpace(filter.Status) &&
                Enum.TryParse<LeaveStatus>(filter.Status, out var status))
            {
                query = query.Where(lr => lr.Status == status);
            }

            // Filter by start date

            if (filter.StartDate.HasValue)
            {
                query = query.Where(l => l.StartDate >= filter.StartDate.Value);
            }

            // Filter by end date

            if (filter.EndDate.HasValue)
            {
                query = query.Where(l => l.EndDate <= filter.EndDate.Value);
            }

            // Filter by keyword in reason

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                query = query.Where(l => l.Reason.Contains(filter.Keyword));
            }

            // Apply dynamic sorting

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var sortOrder = filter.SortOrder?.ToLower() == "desc" ? "descending" : "ascending";
                query = query.OrderBy($"{filter.SortBy} {sortOrder}");
            }

            // Apply pagination
            query = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

            return await query.ToListAsync();
        }

        // Check if the leave period overlaps with any existing leave
        private async Task<bool> IsOverlapping(int employeeId, DateTime start, DateTime end, int? excludeId = null)
        {
            return await _context.LeaveRequests.Where(l => l.EmployeeId == employeeId && l.Id != (excludeId ?? 0))
                                                           .AnyAsync(l =>l.StartDate < end && l.EndDate > start);
        }

        // Get total number of annual leave days taken by an employee in a given year
        private async Task<int> GetAnnualLeaveDays(int employeeId, int year)
        {
            var leaves = await _context.LeaveRequests.Where(l =>l.EmployeeId == employeeId &&
                                                            l.LeaveType == LeaveType.Annual &&
                                                            l.StartDate.Year == year).ToListAsync();

            int totalDays = leaves.Sum(l => (l.EndDate - l.StartDate).Days + 1);

            return totalDays;
        }


        // Validate that a sick leave request includes a reason
        private bool HasReason(string reason)
        {
            return !string.IsNullOrWhiteSpace(reason);
        }

        // Create a new leave request with validation
        public async Task<string?> CreateAsync(LeaveRequest request)
        {
            if (await IsOverlapping(request.EmployeeId, request.StartDate, request.EndDate))
                return "This leave overlaps with existing leave.";

            if (request.LeaveType == LeaveType.Annual)
            {
                var totalDays = await GetAnnualLeaveDays(request.EmployeeId, request.StartDate.Year)
                                 + (request.EndDate - request.StartDate).Days;

                if (totalDays > 20)
                    return "Limit of 20 days annual leave exceeded.";
            }

            if (request.LeaveType == LeaveType.Sick && !HasReason(request.Reason))
                return "A reason must be given for sick leave.";

            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            return null;
        }


        // Generate a leave summary report 
        public async Task<IEnumerable<object>> GetSummaryReport(int year, string? department, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.LeaveRequests.Include(l => l.Employee).AsQueryable();

            // Filter by department

            if (!string.IsNullOrEmpty(department))
                query = query.Where(l => l.Employee.Department == department);

            // Filter by year

            query = query.Where(l => l.StartDate.Year == year);

            // Filter by start date

            if (startDate.HasValue)
                query = query.Where(l => l.StartDate >= startDate.Value);

            // Filter by end date

            if (endDate.HasValue)
                query = query.Where(l => l.EndDate <= endDate.Value);

            var summary = await query
                .GroupBy(l => new { l.EmployeeId, l.Employee.FullName })
                .Select(g => new
                {
                    g.Key.EmployeeId,
                    EmployeeName = g.Key.FullName,
                    TotalLeaves = g.Count(),
                    AnnualLeaves = g.Count(l => l.LeaveType == LeaveType.Annual),
                    SickLeaves = g.Count(l => l.LeaveType == LeaveType.Sick)
                })
                .ToListAsync();

            return summary;
        }


        // Approve a leave request
        public async Task<string?> ApproveAsync(int id)
        {
            var request = await _context.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);

            if (request == null)
                return "Request not found.";

            if (request.Status != LeaveStatus.Pending)
                return "Only pending requests can be approved.";

            request.Status = LeaveStatus.Approved;
            await _context.SaveChangesAsync();

            return null;
        }




    }

}
