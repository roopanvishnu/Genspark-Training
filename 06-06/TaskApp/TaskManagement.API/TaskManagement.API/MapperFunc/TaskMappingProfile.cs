using TaskManagement.API.DTOs.TaskDtos;
using AutoMapper;
using TaskManagement.API.Models;

namespace TaskManagement.API.MapperFunc;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee != null ? src.Assignee.FullName : null))
            .ForMember(dest => dest.AttachmentFileName, opt => opt.MapFrom(src => src.Attachments.FirstOrDefault()!.FileName));
    }
}