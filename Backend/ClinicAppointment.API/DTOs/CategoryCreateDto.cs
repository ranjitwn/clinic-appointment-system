using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.API.DTOs
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
    }
}
