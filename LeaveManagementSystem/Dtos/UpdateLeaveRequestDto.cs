﻿namespace LeaveManagementSystem.Dtos
{
    public class UpdateLeaveRequestDto
    {
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }

}
