using AutoMapper;
using studentsapi.Data.Configs;
using studentsapi.DTO;
using studentsapi.Model;
using studentsapi.Data;

namespace studentsapi.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // CreateMap<Source, Destination>();
            // Add your mappings here
            CreateMap<StudentDto, Data.Student>().ReverseMap();
            CreateMap<Data.Student, StudentDto>();
            CreateMap<Model.Student, StudentDto>().ReverseMap();
            CreateMap<StudentDto, Model.Student>().ReverseMap();



        }
    }
}

