using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class ClinicCreateDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
    }
}
