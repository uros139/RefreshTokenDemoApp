using AutoMapper;
using UserManagement.Data.Models.Entities;

namespace UserManagement.Core.Dto;

public class StudentDto
{
    public string FirstName { get; set; }
    public string LastName {  get; set; }
    public DateTimeOffset BirthDate { get; set; }

    public class StudentProfile : Profile
    {
        protected StudentProfile()
        {
            CreateMap<StudentDto, Student>();
        }
    }
}
