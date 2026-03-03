using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExamProject2.API.Services
{
    public class AuthService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;

        public AuthService(DataContext dataContext, JwtSettings jwtSettings)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
        }

        public async Task<string?> RegisterAsync(PatientRegisterDto dto)
        {
            // Prevent future DOB
            if (dto.DateOfBirth >= DateTime.Today)
            {
                return "Date of birth cannot be in the future.";

            }    

            var hasher = new PasswordHasher<Patient>();

            var existing = await _dataContext.Patients
            .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower());

            // Guest patient exists → upgrade to registered
            if (existing != null)
            {
                if (existing.IsRegistered)
                {
                    return "Patient is already registered.";
                }

                existing.Password = hasher.HashPassword(existing, dto.Password);
                existing.IsRegistered = true;

                await _dataContext.SaveChangesAsync();
                return null;
            }

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                IsRegistered = true
            };

            patient.Password = hasher.HashPassword(patient, dto.Password);

            _dataContext.Patients.Add(patient);
            await _dataContext.SaveChangesAsync();

            return null;
        }

         public async Task<AuthLoginResultDto?> LoginAsync(PatientLoginDto dto)
        {
            // Finding patient by email
           var patient = await _dataContext.Patients
            .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower());

            // Checking patient exists and is registered
            if (patient == null || !patient.IsRegistered)
            {
                return null;
            }

            // Ensure password exists (guest users have null)
            if (patient.Password == null)
            {
                return null;
            }

            // Verify hashed password
            var passwordHasher = new PasswordHasher<Patient>();
            var result = passwordHasher.VerifyHashedPassword(
                patient,
                patient.Password,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // Generate JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
                new Claim(ClaimTypes.Email, patient.Email),
                new Claim(ClaimTypes.Role, patient.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwttoken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwttoken);

            return new AuthLoginResultDto
            {
                Token = tokenString,
                Role = patient.Role
            };
        }
    }
}
