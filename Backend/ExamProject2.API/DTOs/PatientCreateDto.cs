using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class PatientCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]  
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; }
      
    }
}
