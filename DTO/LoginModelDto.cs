using System.ComponentModel.DataAnnotations;

namespace studentsapi.DTO
{
    public class LoginModelDto
    {
        public string Policy { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
