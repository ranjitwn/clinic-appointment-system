using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class PatientLoginDto
    {
         #nullable disable

        [Required] 
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
