﻿namespace LeaveManagementSystem.Dtos
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
