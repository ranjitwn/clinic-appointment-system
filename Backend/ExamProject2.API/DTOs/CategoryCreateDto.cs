using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
    }
}
