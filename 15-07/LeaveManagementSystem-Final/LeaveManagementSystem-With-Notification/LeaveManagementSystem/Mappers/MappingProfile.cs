using AutoMapper;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;

namespace LeaveManagementSystem.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<LeaveTypeDto, LeaveType>();
            CreateMap<LeaveType, LeaveTypeResponseDto>();

            CreateMap<LeaveRequestDto, LeaveRequest>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<LeaveRequest, LeaveRequestResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name))
                .ForMember(dest => dest.ReviewedByName, opt => opt.MapFrom(src => src.ReviewedBy != null ? src.ReviewedBy.Username : null));

            // LeaveBalance Mappings
            CreateMap<LeaveBalance, LeaveBalanceResponseDto>()
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.Name));

            // UserLeaveBalance Mappings
            CreateMap<User, UserLeaveBalanceResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.LeaveBalances, opt => opt.MapFrom(src => src.LeaveBalances));
                
            // UserLeaveBalance for a particular type
            CreateMap<User, UserLeaveBalanceForTypeResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));

            CreateMap<User, UserDetailDto>();
            
        }
    }
}
