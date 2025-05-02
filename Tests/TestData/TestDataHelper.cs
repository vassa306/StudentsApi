using studentsapi.Controllers;
using studentsapi.Data;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using studentsapi.DTO;
using AutoMapper;
using studentsapi.Data.Repository;
using Moq;

namespace studentsapi.Tests.TestData
{
    public static class TestDataHelper
    {
        public static void SeedStudents(CollegeDBContext context)
        {
            if (context.Students.Any())
            {
                Log.Information("Database already seeded.");
                return;
                // Seed data
            }
            context.Students.AddRange(
                new Student { Id = 1, Name = "John", Email = "john@email.com", Address = "123 Main St" },
                new Student { Id = 2, Name = "Kate", Email = "kate@email.com", Address = "456 Elm St" });
            context.SaveChanges();
            Log.Information("Database seeded with test data.");
        }

        public static void SeedStudentsMock(Mock<IStudentRepository> mockRepository)
        {
            var students = new List<Student>
        {
            new Student { Id = 1, Name = "John", Email = "john@email.com", Address = "123 Main St" },
            new Student { Id = 2, Name = "Kate", Email = "kate@email.com", Address = "456 Elm St" }
        };
            mockRepository.Setup(repo => repo.GetAllAsync())
          .ReturnsAsync(students);

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(students.FirstOrDefault(s => s.Id == id)));

            mockRepository.Setup(repo => repo.GetByName(It.IsAny<string>()))
                .Returns((string name) => Task.FromResult(students.FirstOrDefault(s => s.Name == name)));

            mockRepository.Setup(repo => repo.Exists(It.IsAny<StudentDto>()))
                .Returns((StudentDto dto) => Task.FromResult(students.FirstOrDefault(s => s.Email == dto.Email)));

            mockRepository.Setup(repo => repo.IncreaseId())
                .ReturnsAsync(students.Max(s => s.Id) + 1);

            mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Student>()))
                .Callback<Student>(student => students.Add(student))
                .ReturnsAsync((Student student) => student.Id);
        }
        

        public static StudentsController CreateStudentsController(ILogger<StudentsController> logger, IMapper mapper, Mock<IStudentRepository> mockRepository)
        {
            var controller = new StudentsController(logger, mapper, mockRepository.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/json";

            return controller;
        }

        public static StudentsController CreateStudentsControllerInvalidHeaders(ILogger<StudentsController> logger,IMapper mapper, Mock<IStudentRepository> mockRepository)
            
        {
            var controller = new StudentsController(logger, mapper, mockRepository.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "text/plain";

            return controller;
        }
        public static StudentDto CreateStudentDto(int id = 3, string name = "Vasik", string email = "vasik@gmail.com", string address = "123 Main St")
        {
            return new StudentDto
            {
                Name = name,
                Email = email,
                Address = address
            };
        }

        public static StudentDto CreateStudentDtoInvalid(int id = 3, string email = "vasik@gmail.com", string address = "123 Main St")
        {
            return new StudentDto
            {
                Email = email,
                Address = address
            };
        }

        public static StudentDto CreateStudentDtoInvalidEmail(string email = "martingmail.com", string address = "124 Main St")
        {
            return new StudentDto
            {
                Name = "Martin",
                Email = email,
                Address = address
            };
        }

        public static StudentDto CreateStudentDtoInvalidName(string name= "a", string email = "jan@gmail.com", string address = "125 Main St")
        {
            return new StudentDto
            {
                Name = name,
                Email = email,
                Address = address
            };
        }

        public static StudentDto CreateStudentDtoMissingEmail(string name = "Tom", string address = "123 Main St")
        {
            return new StudentDto
            {
                Name = name,
                Address = address
            };
        }

        internal static StudentDto CreateStudentDtoMissingAddress(string name = "Jan", string email = "jan@gmail.com")
        {
            return new StudentDto
            {
                Name = name,
                Email = email
            };
        }

        public static StudentDto? CreateStudentDtoNull()
        {
            return null;
        }
    }
}
