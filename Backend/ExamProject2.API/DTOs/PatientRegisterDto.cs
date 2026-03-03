using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class PatientRegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        // Additional data for registered patients
        public string? Gender { get; set; }
    }
}
