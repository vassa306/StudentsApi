using System.ComponentModel.DataAnnotations;
using studentsapi.Data;

namespace studentsapi.DTO
{
    public class StudentDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }

        public int? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
    }
}
