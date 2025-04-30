using System.Runtime.InteropServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using studentsapi.Configurations;
using studentsapi.Controllers;
using studentsapi.Data;
using studentsapi.DTO;
using studentsapi.Logging;
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

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>(); // register your mapping profile
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        // Add this using directive to resolve 'UseInMemoryDatabase' extension method.
        public async Task GetStudents_ReturnsAllStudents()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);

            // Act  
            var result = await controller.GetStudents();

            // Assert  
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var students = okResult.Value as IEnumerable<StudentDto>;
            Assert.That(students, Is.Not.Null);
            Assert.That(students.Count(), Is.EqualTo(2));
            Assert.That(students.First().Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetStudents_ReturnsAllStudentsXml()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);

            // Act  
            var result = await controller.GetStudents();

            // Assert  
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var students = okResult.Value as IEnumerable<StudentDto>;
            Assert.That(students, Is.Not.Null);
            Assert.That(students.Count(), Is.EqualTo(2));
            Assert.That(students.First().Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetStudents_ReturnsAllStudentsText()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsControllerInvalidHeaders(_logger, context, _mapper);
            // Ensure this is included for InMemory database support
            // Act  
            var result = await controller.GetStudents();
            // Assert  
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(StatusCodes.Status406NotAcceptable));
        }

        [Test]
        public async Task GetStudentByName()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
            var result = await controller.GetStudentByName("John");
            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var student = okResult.Value as StudentDto;
            Assert.That(student, Is.Not.Null);
            Assert.That(student.Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetStudentById()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
            // Act
            var result = await controller.GetStudent(1);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var student = okResult.Value as StudentDto;
            Assert.That(student, Is.Not.Null);
            Assert.That(student.Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetStudentById_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
            // Act
            var maxId = context.Students.Max(s => s.Id) + 1; // Get a non-existing ID
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
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);

            StudentDto studentDto = TestDataHelper.CreateStudentDto();
            var result = await controller.CreateStudent(studentDto);
            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.That(createdAtRouteResult, Is.Not.Null);
            var createdStudent = createdAtRouteResult.Value as StudentDto;
            Assert.That(createdStudent, Is.Not.Null);
            Assert.That(createdStudent.Name, Is.EqualTo(studentDto.Name));
            Assert.That(createdStudent.Email, Is.EqualTo(studentDto.Email));
            Assert.That(createdStudent.Address, Is.EqualTo(studentDto.Address));
            Assert.That(createdStudent.Id, Is.GreaterThan(0));
            var studentInDb = await context.Students.FindAsync(createdStudent.Id);
            Assert.That(studentInDb, Is.Not.Null);
            Assert.That(studentInDb.Name, Is.EqualTo(studentDto.Name));
            Assert.That(studentInDb.Email, Is.EqualTo(studentDto.Email));
            Assert.That(studentInDb.Address, Is.EqualTo(studentDto.Address));
        }

        [Test]
        public async Task CreateUserTestWithInvalidData()
        {
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var context = GetInMemoryDbContext();
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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
            var controller = TestDataHelper.CreateStudentsController(_logger, context, _mapper);
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


    }
    

    // Mock implementation of IMyLogger for testing purposes  
    public class MockLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
    }

    
}
