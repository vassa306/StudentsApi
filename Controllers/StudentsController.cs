using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using studentsapi.Data;
using studentsapi.Data.Repository;
using studentsapi.DTO;
using studentsapi.Logging;
using studentsapi.Model;
using Student = studentsapi.Data.Student;

namespace studentsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentsController : ControllerBase
    {
        
        private readonly ILogger<StudentsController> _logger;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;

        public StudentsController(ILogger<StudentsController> logger, IMapper mapper, IStudentRepository studentRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _studentRepository = studentRepository;
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public async Task <ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var acceptHeaders = Request.Headers.Accept;
            if (!StringValues.IsNullOrEmpty(acceptHeaders) && acceptHeaders.Contains("text/plain"))
            {
                // Client wants XML
                return StatusCode(StatusCodes.Status406NotAcceptable, "Media type is not supported");
            }
            _logger.LogInformation("GetStudents method called.");
            var students = await _studentRepository.GetAllAsync(s => ((Student)(object)s).Department);
            var studentDtos = _mapper.Map<List<StudentDto>>(students);
            return Ok(studentDtos);
        }

        [HttpGet]
        [Route("{id:int}", Name="GetStudentById")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.Student), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task  <ActionResult<IEnumerable<StudentDto>>> GetStudent(int id)
        {
            _logger.LogInformation("GetStudent method called.");
            if (id <= 0)
            {
                _logger.LogCritical($"Student not found with {id}");
                return BadRequest("Invalid student ID.");
            }
            var student = await _studentRepository.GetAsync(student => student.Id == id);
            if (student == null)
            {
                _logger.LogCritical($"Student not found with {id}");
                return NotFound("No students found.");
            }
            var studentDto = _mapper.Map<StudentDto>(student);
            return Ok(studentDto);
        }

        [HttpGet("{name:alpha}", Name = "GetStudentByName")]
        public async Task <ActionResult<IEnumerable<Model.Student>>> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Invalid student name.");
            }
            var student = await _studentRepository.GetAsync(student => student.Name == name);
            var studentDto = _mapper.Map<StudentDto>(student);
            
            if (string.IsNullOrEmpty(studentDto.Name) || string.IsNullOrEmpty(studentDto.Email) || string.IsNullOrEmpty(studentDto.Address))
            {
                return BadRequest("Student data is incomplete.");
            }
            return Ok(studentDto);
        }


        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] StudentDto model)
        {
            _logger.LogInformation("CreateStudent method called.");

            if (!ModelState.IsValid)
            {
                _logger.LogCritical($"Model state is invalid {model}");
                return BadRequest(ModelState);
            }

            if (model == null)
            {
                _logger.LogCritical("Model is null");
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Name is required.");
            }

            // Check if student already exists
            var exists = await _studentRepository.Exists(s => s.Email == model.Email);
            if (exists == true)
            {
                return Conflict(new { message = "A student with this email already exists." });
            }

            var student = _mapper.Map<Student>(model);
            await _studentRepository.CreateAsync(student);

            model.Id = student.Id; // EF will populate this after SaveChangesAsync

            return CreatedAtRoute("GetStudentById", new { id = model.Id }, model);
        }

            [HttpPut]
        [Route("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task <ActionResult> UpdateStudent(int v, [FromBody]StudentDto model)
        {
            _logger.LogInformation("UpdateStudent method called.");
            if (model == null || model.Id <= 0)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                _logger.LogCritical($"Model state is invalid {model}");
                return BadRequest(ModelState);
            }
            var existingStudent = await _studentRepository.GetAsync(student => student.Id == model.Id);
            if (existingStudent == null)
            {
                _logger.LogCritical($"Student not found with ID {model.Id}");
                return NotFound();
            }
            var newRecord = _mapper.Map(model, existingStudent);
            await _studentRepository.UpdateAsync(newRecord);
            return NoContent();
        }

        [HttpPatch]
        [Route("update/{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStudentPartialAsync(int id, [FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<StudentDto> patchDoc)
        {
            _logger.LogInformation("UpdateStudentPartial method called.");
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var existingStudent = await _studentRepository.GetAsync(student => student.Id == id);
            if (existingStudent == null)
            {
                _logger.LogCritical($"Student not found with ID {id}");
                return NotFound();
            }

            var studentDto = _mapper.Map<StudentDto>(existingStudent);
            patchDoc.ApplyTo(studentDto, ModelState);
            existingStudent = _mapper.Map<Student>(studentDto);
            await _studentRepository.UpdateAsync(existingStudent);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}", Name = "DeleteStudentById")]
        public async Task<IActionResult> DeleteStudentAsync(int id)
        {
            _logger.LogInformation("DeleteStudent method called.");
            if (id <= 0)
            {
                _logger.LogCritical("Invalid Student ID");
                return BadRequest("Invalid student ID.");
            }
            var stud = await _studentRepository.GetAsync(student => student.Id == id);
            await _studentRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
