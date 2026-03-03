using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamProject2.API.Services
{
    public class SpecialityService
    {
        private readonly DataContext _dataContext;

        public SpecialityService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<SpecialityDto>> GetSpecialitiesAsync()
        {
            return await _dataContext.Specialities.Select(s => new SpecialityDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToListAsync();
        }

        public async Task<SpecialityDto?> CreateSpecialityAsync(SpecialityCreateDto dto)
        {
            var exists = await _dataContext.Specialities
                .AnyAsync(s => s.Name.ToLower() == dto.Name.Trim().ToLower());

            if (exists)
                return null;

            var speciality = new Speciality
            {
                Name = dto.Name
            };

            _dataContext.Specialities.Add(speciality);
            await _dataContext.SaveChangesAsync();

            return new SpecialityDto
            {
                Id = speciality.Id,
                Name = speciality.Name
            };
        }

        public async Task<SpecialityDto?> GetSpecialityAsync(int id)
        {
            return await _dataContext.Specialities.Select(s => new SpecialityDto
            {
                Id = s.Id,
                Name = s.Name
            }).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> UpdateSpecialityAsync(int id, SpecialityCreateDto dto)
        {
            var exists = await _dataContext.Specialities
                .AnyAsync(s => s.Id != id &&
                            s.Name.ToLower() == dto.Name.Trim().ToLower());

            if (exists)
                return false;

            var speciality = await _dataContext.Specialities.FindAsync(id);

            if (speciality == null)
            {
                return false;
            }

            speciality.Name = dto.Name;

            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> DeleteSpecialityAsync(int id)
        {
            var speciality = await _dataContext.Specialities.FindAsync(id);

            if (speciality == null)
                return null;

            var hasDoctors = await _dataContext.Doctors
                .AnyAsync(d => d.SpecialityId == id);

            if (hasDoctors)
                return false;

            _dataContext.Specialities.Remove(speciality);
            await _dataContext.SaveChangesAsync();

            return true;

        }
    }
}
