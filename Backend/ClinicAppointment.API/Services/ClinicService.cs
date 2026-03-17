using ClinicAppointment.API.Data;
using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.API.Services
{
    public class ClinicService : IClinicService
    {
        private readonly DataContext _dataContext;

        public ClinicService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<ClinicDto>> GetClinicsAsync()
        {
            return await _dataContext.Clinics.Select(c => new ClinicDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
        }

        public async Task<ClinicDto?> CreateClinicAsync(ClinicCreateDto dto)
        {
            var exists = await _dataContext.Clinics
                .AnyAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower());

            if (exists)
                return null;

            var clinic = new Clinic
            {
                Name = dto.Name
            };

            _dataContext.Clinics.Add(clinic);
            await _dataContext.SaveChangesAsync();

            return new ClinicDto
            {
                Id = clinic.Id,
                Name = clinic.Name
            };
        }

        public async Task<ClinicDto?> GetClinicAsync(int id)
        {
            return await _dataContext.Clinics.Select(c => new ClinicDto
            {
                Id = c.Id,
                Name = c.Name
            }).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateClinicAsync(int id, ClinicCreateDto dto)
        {
            var exists = await _dataContext.Clinics
                .AnyAsync(c => c.Id != id &&
                            c.Name.ToLower() == dto.Name.Trim().ToLower());

            if (exists)
                return false;


            var clinic = await _dataContext.Clinics.FindAsync(id);

            if (clinic == null)
            {
                return false;
            }

            clinic.Name = dto.Name;

            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> DeleteClinicAsync(int id)
        {
            var clinic = await _dataContext.Clinics.FindAsync(id);

            if (clinic == null)
                return null;

            // Prevent delete if doctors exist
            var hasDoctors = await _dataContext.Doctors
                .AnyAsync(d => d.ClinicId == id);

            if (hasDoctors)
                return false;

            // Prevent delete if appointments exist
            var hasAppointments = await _dataContext.Appointments
                .AnyAsync(a => a.ClinicId == id);

            if (hasAppointments)
                return false;

            _dataContext.Clinics.Remove(clinic);
            await _dataContext.SaveChangesAsync();

            return true;

        }
    }
}
