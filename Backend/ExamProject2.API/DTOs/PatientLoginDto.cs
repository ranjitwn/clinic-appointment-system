using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class PatientLoginDto
    {
        [Required] 
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }
}
