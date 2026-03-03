using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    #nullable disable
    public class DoctorCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]  
        public string LastName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ClinicId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int SpecialityId { get; set; }
    }
}
