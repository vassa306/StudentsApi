﻿using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using studentsapi.Configurations;
using studentsapi.Controllers;
using studentsapi.Data;
using studentsapi.Data.Repository;
using studentsapi.DTO;
using studentsapi.Model;
using studentsapi.Tests.Helpers;
using studentsapi.Tests.TestData;


namespace studentsapi.Tests

{
    [TestFixture]
    public class StudentTests : DBTestBase
    {
        private readonly ILogger<StudentsController> _logger = new MockLogger<StudentsController>();

        private IMapper _mapper;

        private Mock<IStudentRepository> _studentRepoMock;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>(); // register your mapping profile
            });

            _mapper = config.CreateMapper();
            _studentRepoMock = new Mock<IStudentRepository>();
            TestDataHelper.SeedStudentsMock(_studentRepoMock);
        }

        [Test]
        // Add this using directive to resolve 'UseInMemoryDatabase' extension method.
        public async Task GetStudents_ReturnsAllStudentsTest()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);

            // Act  
            var result = await controller.GetStudents();
            
            // Assert  
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var apiResponse = okResult.Value as ApiResponse;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Status, Is.True);
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var students = apiResponse.Data as IEnumerable<StudentDto>;
            Assert.That(students, Is.Not.Null);
            Assert.That(students.Count(), Is.EqualTo(2));

            _studentRepoMock.Verify(repo => repo.GetAllAsync(s => ((Data.Student)(object)s).Department), Times.Once);
        }

        [Test]
        public async Task GetStudents_ReturnsAllStudentsXml()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);

            // Act  
            var result = await controller.GetStudents();

            // Assert  
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Status, Is.True);
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var students = apiResponse.Data as IEnumerable<StudentDto>;
            Assert.That(students, Is.Not.Null);
            Assert.That(students.Count(), Is.EqualTo(2));
            Assert.That(students.First().Id, Is.EqualTo(1));
            _studentRepoMock.Verify(repo => repo.GetAllAsync(s => ((Data.Student)(object)s).Department), Times.Once);
        }

        [Test]
        public async Task GetStudents_ReturnsAllStudentsText()
        {
            var controller = TestDataHelper.CreateStudentsControllerInvalidHeaders(_logger, _mapper, _studentRepoMock);
            // Ensure this is included for InMemory database support
            // Act  
            var result = await controller.GetStudents();
            // Assert  
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(StatusCodes.Status406NotAcceptable));
            Assert.That(statusCodeResult.Value, Is.EqualTo("Media type is not supported"));

        }

        [Test]
        public async Task GetStudentByName()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var result = await controller.GetStudentByName("John");
            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Status, Is.True);
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var student = apiResponse.Data as StudentDto;
            _studentRepoMock.Verify(repo => repo.GetAsync(It.IsAny<Expression<Func<Data.Student, bool>>>()), Times.Once);
            Assert.That(student, Is.Not.Null);
            Assert.That(student.Name, Is.EqualTo("John"));
            Assert.That(student.DepartmentName, Is.EqualTo("IT"));
            Console.WriteLine($"Student: {student.Name}, {student.Address}, {student.DepartmentName}");

        }

        [Test]
        public async Task GetStudentById()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            // Act
            var result = await controller.GetStudent(1);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse;
            _studentRepoMock.Verify(
            r => r.GetAsync(It.IsAny<Expression<Func<Data.Student, bool>>>()),
            Times.Once);
            var student = apiResponse.Data as StudentDto;
            Assert.That(student, Is.Not.Null);
            Assert.That(student.Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetStudentById_ReturnsNotFound()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            // Act
            var maxId = 999; // Get a non-existing ID
            Console.WriteLine($"MaxId: {maxId}");
            var result = await controller.GetStudent(maxId);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.EqualTo("No students found."));
        }

        [Test]
        public async Task GetStudentById_ReturnsBadRequest()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            // Act
            var result = await controller.GetStudent(0);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid student ID."));
        }
        [Test]
        public async Task GetStudentById_ReturnsBadRequestNegativeId()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            // Act
            var result = await controller.GetStudent(-1);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid student ID."));
        }
        [Test]
        public async Task CreateUserTest()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);

            StudentDto studentDto = TestDataHelper.CreateStudentDto();
            var result = await controller.CreateStudent(studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());

            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.That(createdAtRouteResult, Is.Not.Null);

            var apiResponse = createdAtRouteResult.Value as ApiResponse;

            var createdStudent = apiResponse.Data as StudentDto;
            Assert.That(createdStudent, Is.Not.Null);
            Assert.That(createdStudent.Name, Is.EqualTo(studentDto.Name));
            Assert.That(createdStudent.Email, Is.EqualTo(studentDto.Email));
            Assert.That(createdStudent.Address, Is.EqualTo(studentDto.Address));
            Assert.That(createdStudent.Id, Is.GreaterThan(0));

            Console.WriteLine($"Created student ID: {createdStudent.Id} , {createdStudent.Name}, {createdStudent.Address}, {createdStudent.DepartmentId}");
            // Optional: ověření, že mock skutečně zavolal CreateAsync
            _studentRepoMock.Verify(r => r.CreateAsync(It.Is<Data.Student>(
                s => s.Email == studentDto.Email && s.Name == studentDto.Name)), Times.Once);
        }


        [Test]
        public async Task CreateUserTestWithInvalidData()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoInvalid();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            var result = await controller.CreateStudent(studentDto);
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            var errors = badRequestResult.Value as SerializableError;
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors.ContainsKey("Name"));

            var errorMessages = errors["Name"] as string[];
            Assert.That(errorMessages, Is.Not.Null.And.Not.Empty);
            Assert.That(errorMessages[0], Is.EqualTo("The Name field is required."));
        }

        [Test]
        public async Task CreateInvalidUserEmail()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoInvalidEmail();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            var result = await controller.CreateStudent(studentDto);
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            var errors = badRequestResult.Value as SerializableError;
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors.ContainsKey("Email"));

            var errorMessages = errors["Email"] as string[];
            Assert.That(errorMessages, Is.Not.Null.And.Not.Empty);
            Assert.That(errorMessages[0], Is.EqualTo("The Email field is not a valid e-mail address."));
        }
        [Test]
        public async Task CreateInvalidUserName()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoInvalidName();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            var result = await controller.CreateStudent(studentDto);
            // Assert

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");

            // Safe cast
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult was unexpectedly null.");
            _logger.LogInformation("BadRequestObjectResult: {0}", badRequestResult.Value);
            // Check model state errors
            var errors = badRequestResult.Value as SerializableError;
            Assert.That(errors, Is.Not.Null, "ModelState was not returned.");
            Assert.That(errors.ContainsKey("Name"), "Missing validation error for Name.");
        }

        [Test]
        public async Task CreateUserwithoutEmail()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoMissingEmail();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            var result = await controller.CreateStudent(studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");
            // Safe cast
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult was unexpectedly null.");
            _logger.LogInformation("BadRequestObjectResult: {0}", badRequestResult.Value);
            // Check model state errors
            var errors = badRequestResult.Value as SerializableError;
            Assert.That(errors, Is.Not.Null, "ModelState was not returned.");
            Assert.That(errors.ContainsKey("Email"), "Missing validation error for Email.");
            var errorMessages = errors["Email"] as string[];
            Assert.That(errorMessages, Is.Not.Null.And.Not.Empty, "Error messages for Email were unexpectedly null or empty.");
            Assert.That(errorMessages[0], Is.EqualTo("The Email field is required."), "Unexpected error message for Email.");
        }
        [Test]
        public async Task CreateUserwithoutAddress()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoMissingAddress();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            var result = await controller.CreateStudent(studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");
            // Safe cast
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult was unexpectedly null.");
            _logger.LogInformation("BadRequestObjectResult: {0}", badRequestResult.Value);
            // Check model state errors
            var errors = badRequestResult.Value as SerializableError;
            Assert.That(errors, Is.Not.Null, "ModelState was not returned.");
            Assert.That(errors.ContainsKey("Address"), "Missing validation error for Address.");
            var errorMessages = errors["Address"] as string[];
            Assert.That(errorMessages, Is.Not.Null.And.Not.Empty, "Error messages for Address were unexpectedly null or empty.");
            Assert.That(errorMessages[0], Is.EqualTo("The Address field is required."), "Unexpected error message for Address.");
        }

        [Test]
        public async Task CreateUserNull()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoNull();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            var result = await controller.CreateStudent(studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequestObjectResult.");
            // Safe cast
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "BadRequestObjectResult was unexpectedly null.");
            _logger.LogInformation("BadRequestObjectResult: {0}", badRequestResult.Value);
        }

        [Test]
        public async Task CreateUserWithExistingEmail()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.CreateStudentDtoWithExistingEmail();
            ModelValidationHelper.ValidateModelState(controller, studentDto);
            _studentRepoMock.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Data.Student, bool>>>()))
                .ReturnsAsync(true); // Simulate existing email
            var result = await controller.CreateStudent(studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<ConflictObjectResult>(), "Expected ConflictObjectResult");
            // Safe cast
            var ConflictResult = result.Result as ConflictObjectResult;
            Assert.That(ConflictResult, Is.Not.Null, "BadRequestObjectResult was unexpectedly null.");
            _logger.LogInformation("ConflictResult: {0}", ConflictResult.Value);

        }
        [Test]
        public async Task UpdateUserTest()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.GetExistingDto();
            var result = await controller.UpdateStudent(studentDto.Id, studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<NoContentResult>(), "Expected NoContentResult");

            var noContentResult = result.Result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null, "NoContentResult was unexpectedly null.");

            _studentRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Data.Student>(
                s => s.Id == studentDto.Id &&
                     s.Name == studentDto.Name &&
                     s.Email == studentDto.Email &&
                     s.Address == studentDto.Address
            )), Times.Once);
        }



        [Test]
        public async Task UpdateUserTestWithInvalidData()
        {
            var controller = TestDataHelper.CreateStudentsController(_logger, _mapper, _studentRepoMock);
            var studentDto = TestDataHelper.GetExistingDto();
            studentDto.Name = null; // invalid input

            ModelValidationHelper.ValidateModelState(controller, studentDto);

            // Act
            var result = await controller.UpdateStudent(studentDto.Id, studentDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>(), "Expected BadRequest due to invalid model.");

            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);

            var errors = badRequest.Value as SerializableError;
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors.ContainsKey("Name"), "Missing validation error for Name.");

            var errorMessages = errors["Name"] as string[];
            Assert.That(errorMessages, Is.Not.Null.And.Not.Empty);
            Assert.That(errorMessages[0], Is.EqualTo("The Name field is required."));

            // Ensure repository method was not called
            _studentRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Data.Student>()), Times.Never);

        }
    }
    

    // Mock implementation of IMyLogger for testing purposes  
    public class MockLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
    }

    
}
