using studentsapi.Controllers;
using studentsapi.Data;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using studentsapi.DTO;
using AutoMapper;
using studentsapi.Data.Repository;
using Moq;
using System.Net;
using System.Xml.Linq;
using System.Linq.Expressions;


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

            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .Returns((Expression<Func<Student, bool>> expr) =>
            Task.FromResult(students.AsQueryable().FirstOrDefault(expr.Compile())));

            mockRepository.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Student, bool>>>()))
                .ReturnsAsync(false);

            mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Student>()))
            .Callback<Student>(student =>
            {
                student.Id = students.Max(s => s.Id) + 1;
                students.Add(student);
            })
            .ReturnsAsync((Student student) => student);
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

        internal static StudentDto CreateStudentDtoWithExistingEmail(string name = "Vasik", string email = "john@email.com", string address = "123 Main St")
        {
            return new StudentDto
            {
                
                Name = name,
                Email = email,
                Address = address
            };
        }

        internal static StudentDto? GetExistingDto(int id = 1 ,string name = "John", string email = "john@email.com", string address = "123 Main St" )
        {
            return new StudentDto
            {
                Id = id,
                Name = name,
                Email = email,
                Address = address
            };
        }
    }
}
