using AutoMapper;
using studentsapi.Data.Configs;
using studentsapi.DTO;
using studentsapi.Data;

namespace studentsapi.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // CreateMap<Source, Destination>();
            // Add your mappings here
            CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
            .ReverseMap()
            .ForMember(dest => dest.Department, opt => opt.Ignore());
            CreateMap<Model.Student, StudentDto>().ReverseMap();
        } 
    }
}

