using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamProject2.API.Services
{
    public class DoctorService
    {
        private readonly DataContext _dataContext;

        public DoctorService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<DoctorDto>> GetDoctorsAsync()
        {
            return await _dataContext.Doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                ClinicId = d.ClinicId,
                SpecialityId = d.SpecialityId
            }).ToListAsync();
        }

        public async Task<DoctorDto?> CreateDoctorAsync(DoctorCreateDto dto)
        {
            var exists = await _dataContext.Doctors.AnyAsync(d =>
                d.FirstName.ToLower() == dto.FirstName.Trim().ToLower() &&
                d.LastName.ToLower() == dto.LastName.Trim().ToLower() &&
                d.ClinicId == dto.ClinicId
            );

            if (exists)
                return null;

            var doctor = new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ClinicId = dto.ClinicId,
                SpecialityId = dto.SpecialityId
            };

            _dataContext.Doctors.Add(doctor);
            await _dataContext.SaveChangesAsync();

            return new DoctorDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                ClinicId = doctor.ClinicId,
                SpecialityId = doctor.SpecialityId
            };
        }

        public async Task<DoctorDto?> GetDoctorAsync(int id)
        {
            return await _dataContext.Doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                ClinicId = d.ClinicId,
                SpecialityId = d.SpecialityId
            }).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<DoctorDto>> GetDoctorsByClinicAsync(int clinicId)
        {
            return await _dataContext.Doctors
                .Where(d => d.ClinicId == clinicId)
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    ClinicId = d.ClinicId,
                    SpecialityId = d.SpecialityId
                })
                .ToListAsync();
        }

        public async Task<List<DoctorDto>> GetDoctorsBySpecialityAsync(int specialityId)
        {
            return await _dataContext.Doctors
                .Where(d => d.SpecialityId == specialityId)
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    ClinicId = d.ClinicId,
                    SpecialityId = d.SpecialityId
                })
                .ToListAsync();
        }

        public async Task<List<DoctorSearchDto>> SearchDoctorsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<DoctorSearchDto>();

            var q = query.Trim().ToLower();

            return await _dataContext.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .Where(d =>
                    d.FirstName.ToLower().Contains(q) ||
                    d.LastName.ToLower().Contains(q))
                .Select(d => new DoctorSearchDto
                {
                    DoctorId = d.Id,
                    FullName = d.FirstName + " " + d.LastName,
                    Clinic = d.Clinic != null ? d.Clinic.Name : "",
                    Speciality = d.Speciality != null ? d.Speciality.Name : ""
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateDoctorAsync(int id, DoctorCreateDto dto)
        {
            var exists = await _dataContext.Doctors.AnyAsync(d =>
                d.Id != id &&
                d.FirstName.ToLower() == dto.FirstName.Trim().ToLower() &&
                d.LastName.ToLower() == dto.LastName.Trim().ToLower() &&
                d.ClinicId == dto.ClinicId
            );

            if (exists)
                return false;

            var doctor = await _dataContext.Doctors.FindAsync(id);

            if (doctor == null)
                return false;

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.ClinicId = dto.ClinicId;
            doctor.SpecialityId = dto.SpecialityId;

            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> DeleteDoctorAsync(int id)
        {
            var doctor = await _dataContext.Doctors.FindAsync(id);

            if (doctor == null)
                return null;

            var hasAppointments = await _dataContext.Appointments
                .AnyAsync(a => a.DoctorId == id);

            if (hasAppointments)
                return false;

            _dataContext.Doctors.Remove(doctor);
            await _dataContext.SaveChangesAsync();

            return true;

        }

    }
}
