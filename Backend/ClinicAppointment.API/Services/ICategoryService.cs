using ClinicAppointment.API.DTOs;

namespace ClinicAppointment.API.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> CreateCategoryAsync(CategoryCreateDto dto);
        Task<CategoryDto?> GetCategoryAsync(int id);
        Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto dto);
        Task<bool?> DeleteCategoryAsync(int id);
    }
}
