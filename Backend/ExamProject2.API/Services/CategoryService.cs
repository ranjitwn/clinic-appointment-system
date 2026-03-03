using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamProject2.API.Services
{
    public class CategoryService
    {
        private readonly DataContext _dataContext;

        public CategoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return await _dataContext.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
        }

        public async Task<CategoryDto?> CreateCategoryAsync(CategoryCreateDto dto)
        {
            var exists = await _dataContext.Categories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                return null;


            var category = new Category
            {
                Name = dto.Name
            };

            _dataContext.Categories.Add(category);
            await _dataContext.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto?> GetCategoryAsync(int id)
        {
            return await _dataContext.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto dto)
        {
            var exists = await _dataContext.Categories
                .AnyAsync(c => c.Id != id &&
                            c.Name.ToLower() == dto.Name.Trim().ToLower());

            if (exists)
                return false;

            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            category.Name = dto.Name;
            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> DeleteCategoryAsync(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
                return null;

            var hasAppointments = await _dataContext.Appointments
                .AnyAsync(a => a.CategoryId == id);

            if (hasAppointments)
                return false;

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();

            return true;
        }
    }
}
