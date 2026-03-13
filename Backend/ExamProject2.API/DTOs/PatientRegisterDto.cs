using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class PatientRegisterDto
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        // Additional data for registered patients
        public string? Gender { get; set; }
    }
}
