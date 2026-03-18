using ClinicAppointment.API.Constants;
using ClinicAppointment.API.Data;
using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ClinicAppointment.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(DataContext dataContext, JwtSettings jwtSettings, ILogger<AuthService> logger)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        public async Task<string?> RegisterAsync(PatientRegisterDto dto)
        {
            _logger.LogInformation("Register attempt for email {Email}", dto.Email);

            // Prevent future DOB
            if (dto.DateOfBirth >= DateTime.Today)
            {
                _logger.LogWarning("Registration failed due to invalid DOB for email {Email}", dto.Email);
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
                    _logger.LogWarning("Registration attempted for already registered email {Email}", dto.Email);
                    return "Patient is already registered.";
                }

                existing.Password = hasher.HashPassword(existing, dto.Password);
                existing.IsRegistered = true;

                await _dataContext.SaveChangesAsync();
                _logger.LogInformation("Guest patient upgraded to registered account for email {Email}", dto.Email);

                return null;
            }

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                IsRegistered = true,
                Role = Roles.Patient
            };

            patient.Password = hasher.HashPassword(patient, dto.Password);

            _dataContext.Patients.Add(patient);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("New patient registered successfully with email {Email}", dto.Email);

            return null;
        }

        public async Task<AuthLoginResultDto?> LoginAsync(PatientLoginDto dto)
        {
            _logger.LogInformation("Login attempt for email {Email}", dto.Email);

            // Finding patient by email
            var patient = await _dataContext.Patients
             .FirstOrDefaultAsync(p => p.Email.ToLower() == dto.Email.ToLower());

            // Checking patient exists and is registered
            if (patient == null || !patient.IsRegistered)
            {
                // Fall back to Users table (admin accounts)
                var user = await _dataContext.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (user != null)
                {
                    var userHasher = new PasswordHasher<User>();
                    var userResult = userHasher.VerifyHashedPassword(user, user.Password, dto.Password);

                    if (userResult == PasswordVerificationResult.Failed)
                    {
                        _logger.LogWarning("Login failed for admin email {Email}: invalid password", dto.Email);
                        return null;
                    }

                    _logger.LogInformation("Admin login successful for user {UserId}", user.Id);

                    var userClaims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var userKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                    var userCreds = new SigningCredentials(userKey, SecurityAlgorithms.HmacSha256);

                    var userToken = new JwtSecurityToken(
                        issuer: _jwtSettings.Issuer,
                        audience: _jwtSettings.Audience,
                        claims: userClaims,
                        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                        signingCredentials: userCreds
                    );

                    return new AuthLoginResultDto
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(userToken),
                        Role = user.Role
                    };
                }

                _logger.LogWarning("Login failed for email {Email}: patient not found or not registered", dto.Email);
                return null;
            }

            // Ensure password exists (guest users have null)
            if (patient.Password == null)
            {
                _logger.LogWarning("Login failed for email {Email}: password missing", dto.Email);
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
                _logger.LogWarning("Login failed for email {Email}: invalid password", dto.Email);
                return null;
            }

            _logger.LogInformation("Login successful for patient {PatientId}", patient.Id);

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

            _logger.LogInformation("JWT token generated for patient {PatientId}", patient.Id);

            return new AuthLoginResultDto
            {
                Token = tokenString,
                Role = patient.Role
            };
        }
    }
}
