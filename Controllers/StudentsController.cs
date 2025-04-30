using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using studentsapi.Data;
using studentsapi.DTO;
using studentsapi.Logging;
using studentsapi.Model;

namespace studentsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentsController : ControllerBase
    {
        private readonly CollegeDBContext _context;
        private readonly ILogger<StudentsController> _logger;
        private readonly IMapper _mapper;
        
        public StudentsController(ILogger<StudentsController> logger, CollegeDBContext context, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
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
            var students = await _context.Students.ToListAsync();
            var studentDtos = _mapper.Map<List<StudentDto>>(students);

            if (!students.Any())
            {
                return NotFound("No students found.");
            }
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
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
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
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Name == name);
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
        public async Task <ActionResult<StudentDto>> CreateStudent([FromBody]StudentDto model)
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
            var existing = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == model.Email);


            if (existing != null)
            {
                _logger.LogWarning("Student with email {Email} already exists.", model.Email);
                return Conflict(new { message = "A student with this email already exists." });
            }

            int newId = await _context.Students.MaxAsync(s => s.Id) + 1; // Generate new ID

            Data.Student student = _mapper.Map<Data.Student>(model);

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            model.Id = newId;

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
                BadRequest();
            }
            var existingStudent = await _context.Students.Where(s => s.Id == model.Id).FirstOrDefaultAsync();
            if (existingStudent == null)
            {
                _logger.LogCritical($"Student not found with ID {model.Id}");
                return NotFound();
            }
            existingStudent.Name = model.Name;
            existingStudent.Email = model.Email;
            existingStudent.Address = model.Address;

            await _context.SaveChangesAsync();
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
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (existingStudent == null)
            {
                _logger.LogCritical($"Student not found with ID {id}");
                return NotFound();
            }
            var studentDto = _mapper.Map<StudentDto>(existingStudent);
            patchDoc.ApplyTo(studentDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(studentDto, existingStudent);
            await _context.SaveChangesAsync();
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
            var stud = await _context.Students.Where(n => n.Id == id).FirstOrDefaultAsync();
            _context.Students.Remove(stud);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
