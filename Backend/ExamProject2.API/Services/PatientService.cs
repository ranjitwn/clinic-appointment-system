using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamProject2.API.Services
{
    public class PatientService
    {
        private readonly DataContext _dataContext;

        public PatientService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<PatientDto>> GetPatientsAsync()
        {
            return await _dataContext.Patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                IsRegistered = p.IsRegistered,
                Role = p.Role,
                Gender = p.Gender
            }).ToListAsync();
        }

        public async Task<string?> CreatePatientAsync(PatientCreateDto dto)
        {
            // Validate Date of Birth FIRST
            var dob = dto.DateOfBirth;

            if (dob == default)
                return "Date of birth is required.";                

            if (dob > DateTime.Today)
                return "Date of birth cannot be in the future.";

            if (dob < DateTime.Today.AddYears(-120))
                return "Invalid date of birth.";

            // Check if patient already exists by email
            var existingPatient = await _dataContext.Patients
                .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower());

            if (existingPatient != null && existingPatient.IsRegistered)
                return "A registered patient already exists with this email.";

             if (existingPatient != null)
                return null; // guest already exists OK    

            // Create new guest patient
            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                IsRegistered = false
            };

            _dataContext.Patients.Add(patient);
            await _dataContext.SaveChangesAsync();

            return null; // success
        }
        public async Task<PatientDto?> GetPatientAsync(int id)
        {
            return await _dataContext.Patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                IsRegistered = p.IsRegistered,
                Role = p.Role
            }).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientCreateDto dto)
        {
            var patient = await _dataContext.Patients.FindAsync(id);

            if (patient == null)
                return false;

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Email = dto.Email;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = dto.Gender;

            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool?> DeletePatientAsync(int id)
        {
            var patient = await _dataContext.Patients.FindAsync(id);

            if (patient == null)
                return null;

            // Prevent deletion if appointments exist
            var hasAppointments = await _dataContext.Appointments
                .AnyAsync(a => a.PatientId == id);

            if (hasAppointments)
                return false;

            _dataContext.Patients.Remove(patient);
            await _dataContext.SaveChangesAsync();

            return true;
        }
    }
}
