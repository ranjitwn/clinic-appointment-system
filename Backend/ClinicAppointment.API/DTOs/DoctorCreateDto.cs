using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class DoctorCreateDto
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ClinicId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SpecialityId { get; set; }
    }
}
