using LeaveManagementSystem.Dtos;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Data.Repositories.Interfaces
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequest>> GetAllLeaveRequests();
        Task<IEnumerable<LeaveRequest>> FilterLeaveRequests(LeaveRequestFilterDto filter);
        Task<string?> CreateAsync(LeaveRequest request);
        Task<IEnumerable<object>> GetSummaryReport(int year, string? department, DateTime? startDate, DateTime? endDate);
        Task<string?> ApproveAsync(int id);

    }
}
