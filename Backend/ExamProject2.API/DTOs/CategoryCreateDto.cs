using System.ComponentModel.DataAnnotations;

namespace ExamProject2.API.DTOs
{
    #nullable disable
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
