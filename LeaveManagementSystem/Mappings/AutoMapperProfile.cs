using AutoMapper;
using LeaveManagementSystem.Dtos;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            
            CreateMap<LeaveRequest, LeaveRequestDto>()
                     .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                     .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()))
                     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateLeaveRequestDto, LeaveRequest>();

            CreateMap<UpdateLeaveRequestDto, LeaveRequest>();
        }
    }
}
