using studentsapi.DTO;

namespace studentsapi.Data.Repository
{
    public interface IStudentRepository
    {
        Task <List<Student>> GetAllAsync();
        Task <Student> GetByIdAsync(int id);
        Task<Student> GetByName(string name);
        Task<int> CreateAsync(Student student);
        Task<int> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);

        Task<Student> Exists(StudentDto studentDto);
        Task<int> IncreaseId();

    }
}
