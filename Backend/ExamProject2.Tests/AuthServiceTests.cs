using System;
using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using ExamProject2.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExamProject2.Tests;

public class AuthServiceTests
{
    private DataContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private JwtSettings GetJwtSettings()
    {
        return new JwtSettings
        {
            SecretKey = "TestSecretKey123456789012345678901234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        };
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenDateOfBirthIsInFuture()
    {
        var db = GetInMemoryDb();
        var jwt = GetJwtSettings();
        var service = new AuthService(db, jwt);

        var dto = new PatientRegisterDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddDays(1)
        };

        var result = await service.RegisterAsync(dto);

        Assert.Equal("Date of birth cannot be in the future.", result);
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenPatientAlreadyRegistered()
    {
        var db = GetInMemoryDb();
        var jwt = GetJwtSettings();

        db.Patients.Add(new Patient
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@test.com",
            IsRegistered = true,
            Role = "Patient"
        });

        await db.SaveChangesAsync();

        var service = new AuthService(db, jwt);

        var dto = new PatientRegisterDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@test.com",
            Password = "Password123!",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = await service.RegisterAsync(dto);

        Assert.Equal("Patient is already registered.", result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var db = GetInMemoryDb();
        var jwt = GetJwtSettings();

        var patient = new Patient
        {
            FirstName = "Alex",
            LastName = "Smith",
            Email = "alex@test.com",
            IsRegistered = true,
            Role = "Patient"
        };

        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Patient>();
        patient.Password = hasher.HashPassword(patient, "Password123!");

        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        var service = new AuthService(db, jwt);

        var dto = new PatientLoginDto
        {
            Email = "alex@test.com",
            Password = "Password123!"
        };

        var result = await service.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal("Patient", result.Role);
    }
}