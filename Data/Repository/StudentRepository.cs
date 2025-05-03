
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using studentsapi.DTO;

namespace studentsapi.Data.Repository
{
    public class StudentRepository : CollegeRepository<Student>, IStudentRepository
    {
        private readonly CollegeDBContext _context;

        // Constructor for dependency injection
        public StudentRepository(CollegeDBContext context) : base(context)
        {
            _context = context;
        }

        // Override GetByIdAsync to match the nullability of the interface
       
    }

}
