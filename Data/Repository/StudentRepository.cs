
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using studentsapi.DTO;

namespace studentsapi.Data.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly CollegeDBContext _context;
        public StudentRepository(CollegeDBContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students.Where(student => student.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Student> GetByName(string name)
        {
            return await _context.Students.Where(student => student.Name.ToLower().Equals(name.ToLower())).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(Student student)
        {
            var studentToUpdate = await _context.Students.Where(student => student.Id == student.Id).FirstOrDefaultAsync();
            if (studentToUpdate == null)
            {
                throw new ArgumentNullException($"No Student not found with {student.Id}");
            }
            _context.Update(student);
            await _context.SaveChangesAsync();
            return studentToUpdate.Id;
        }

        public async Task<int> CreateAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student.Id;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var studentToDelete = await _context.Students.Where(student => student.Id == student.Id).FirstOrDefaultAsync();
            if (studentToDelete == null)
            {
                throw new ArgumentNullException($"No Student not found with {id}");
            }
            _context.Students.Remove(studentToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Student> Exists(StudentDto studentDto)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(student => student.Email == studentDto.Email);
            if (student == null)
            {
                throw new ArgumentNullException($"No Student found");
            }
            return student;
        }
        public async Task<int> IncreaseId()
        {
            var newId = await _context.Students.MaxAsync(s => s.Id) + 1; ;
            if (newId == null)
            {
                return 1; // If no students exist, start with ID 1
            }
            return newId; // Increment the highest ID by 1
        }
    }
}
