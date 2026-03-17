using ClinicAppointment.API.Data;
using ClinicAppointment.API.DTOs;
using ClinicAppointment.API.Models;
using ClinicAppointment.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ExamProject2.Tests;

public class AppointmentServiceTests
{
    private DataContext GetDb()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private PatientService GetPatientService(DataContext db)
    {
        return new PatientService(db, NullLogger<PatientService>.Instance);
    }

    private AppointmentService GetService(DataContext db)
    {
        return new AppointmentService(
            db,
            GetPatientService(db),
            NullLogger<AppointmentService>.Instance
        );
    }

    [Fact]
    public async Task CreateAppointment_ShouldFail_WhenWeekend()
    {
        var db = GetDb();

        db.Clinics.Add(new Clinic { Id = 1, Name = "Clinic A" });
        db.Doctors.Add(new Doctor { Id = 1, FirstName = "Doc", LastName = "One", ClinicId = 1, SpecialityId = 1 });
        db.Categories.Add(new Category { Id = 1, Name = "General" });

        await db.SaveChangesAsync();

        var service = GetService(db);

        var dto = new AppointmentCreateDto
        {
            AppointmentDate = new DateTime(2030, 6, 1, 10, 0, 0), // Saturday
            DurationMinutes = 30,
            ClinicId = 1,
            DoctorId = 1,
            CategoryId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = await service.CreateAppointmentAsync(dto, null);

        Assert.Equal("Appointments cannot be booked on weekends.", result);
    }

    [Fact]
    public async Task CreateAppointment_ShouldFail_WhenInvalidDuration()
    {
        var db = GetDb();

        db.Clinics.Add(new Clinic { Id = 1, Name = "Clinic A" });
        db.Doctors.Add(new Doctor { Id = 1, FirstName = "Doc", LastName = "One", ClinicId = 1, SpecialityId = 1 });
        db.Categories.Add(new Category { Id = 1, Name = "General" });

        await db.SaveChangesAsync();

        var service = GetService(db);

        var dto = new AppointmentCreateDto
        {
            AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0),
            DurationMinutes = 20,
            ClinicId = 1,
            DoctorId = 1,
            CategoryId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = await service.CreateAppointmentAsync(dto, null);

        Assert.Equal("Invalid duration.", result);
    }

    [Fact]
    public async Task DeleteAppointment_ShouldReturnFalse_WhenNotFound()
    {
        var db = GetDb();
        var service = GetService(db);

        var result = await service.DeleteAppointmentAsync(1, 1);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAvailableSlots_ShouldReturnSlots()
    {
        var db = GetDb();
        var service = GetService(db);

        var slots = await service.GetAvailableSlotsAsync(
            doctorId: 1,
            date: new DateTime(2030, 6, 3),
            slotDuration: 30
        );

        Assert.NotEmpty(slots);
    }

    // --- Conflict detection tests ---
    // 2030-06-03 is a Monday (2030-06-01 is Saturday per test above)

    private async Task SeedBaseDataAsync(DataContext db)
    {
        db.Clinics.Add(new Clinic { Id = 1, Name = "Clinic A" });
        db.Specialities.Add(new Speciality { Id = 1, Name = "General" });
        db.Doctors.Add(new Doctor { Id = 1, FirstName = "Doc", LastName = "One", ClinicId = 1, SpecialityId = 1 });
        db.Doctors.Add(new Doctor { Id = 2, FirstName = "Doc", LastName = "Two", ClinicId = 1, SpecialityId = 1 });
        db.Categories.Add(new Category { Id = 1, Name = "Consultation" });
        db.Patients.Add(new Patient { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", DateOfBirth = new DateTime(1990, 1, 1), IsRegistered = true, Role = "Patient" });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAppointment_ShouldFail_WhenDoctorAlreadyBookedAtExactSameTime()
    {
        var db = GetDb();
        await SeedBaseDataAsync(db);

        // Existing appointment: Doctor 1, 10:00–10:30
        db.Appointments.Add(new Appointment { Id = 1, AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0), DurationMinutes = 30, ClinicId = 1, DoctorId = 1, PatientId = 1, CategoryId = 1 });
        await db.SaveChangesAsync();

        var service = GetService(db);

        // New appointment: same doctor, same slot
        var dto = new AppointmentCreateDto
        {
            AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0),
            DurationMinutes = 30,
            ClinicId = 1,
            DoctorId = 1,
            CategoryId = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            DateOfBirth = new DateTime(1992, 5, 10)
        };

        var result = await service.CreateAppointmentAsync(dto, null);

        Assert.Equal("Doctor is already booked at this time.", result);
    }

    [Fact]
    public async Task CreateAppointment_ShouldFail_WhenNewAppointmentPartiallyOverlapsDoctorSlot()
    {
        var db = GetDb();
        await SeedBaseDataAsync(db);

        // Existing appointment: Doctor 1, 10:00–10:30
        db.Appointments.Add(new Appointment { Id = 1, AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0), DurationMinutes = 30, ClinicId = 1, DoctorId = 1, PatientId = 1, CategoryId = 1 });
        await db.SaveChangesAsync();

        var service = GetService(db);

        // New appointment: 10:15–10:45, starts mid-existing
        var dto = new AppointmentCreateDto
        {
            AppointmentDate = new DateTime(2030, 6, 3, 10, 15, 0),
            DurationMinutes = 30,
            ClinicId = 1,
            DoctorId = 1,
            CategoryId = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            DateOfBirth = new DateTime(1992, 5, 10)
        };

        var result = await service.CreateAppointmentAsync(dto, null);

        Assert.Equal("Doctor is already booked at this time.", result);
    }

    [Fact]
    public async Task CreateAppointment_ShouldSucceed_WhenNewAppointmentStartsExactlyWhenExistingEnds()
    {
        var db = GetDb();
        await SeedBaseDataAsync(db);

        // Existing appointment: Doctor 1, 10:00–10:30
        db.Appointments.Add(new Appointment { Id = 1, AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0), DurationMinutes = 30, ClinicId = 1, DoctorId = 1, PatientId = 1, CategoryId = 1 });
        await db.SaveChangesAsync();

        var service = GetService(db);

        // New appointment: 10:30–11:00, adjacent — no overlap
        var dto = new AppointmentCreateDto
        {
            AppointmentDate = new DateTime(2030, 6, 3, 10, 30, 0),
            DurationMinutes = 30,
            ClinicId = 1,
            DoctorId = 1,
            CategoryId = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            DateOfBirth = new DateTime(1992, 5, 10)
        };

        var result = await service.CreateAppointmentAsync(dto, null);

        Assert.Null(result); // null = success
    }

    [Fact]
    public async Task CreateAppointment_ShouldFail_WhenPatientAlreadyHasAppointmentAtSameTime()
    {
        var db = GetDb();
        await SeedBaseDataAsync(db);

        // Patient 1 already has an appointment with Doctor 1 at 10:00
        db.Appointments.Add(new Appointment { Id = 1, AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0), DurationMinutes = 30, ClinicId = 1, DoctorId = 1, PatientId = 1, CategoryId = 1 });
        await db.SaveChangesAsync();

        var service = GetService(db);

        // Same patient tries to book Doctor 2 at the same time
        var dto = new AppointmentCreateDto
        {
            AppointmentDate = new DateTime(2030, 6, 3, 10, 0, 0),
            DurationMinutes = 30,
            ClinicId = 1,
            DoctorId = 2,
            CategoryId = 1
        };

        var result = await service.CreateAppointmentAsync(dto, patientId: 1);

        Assert.Equal("Patient already has an appointment at this time.", result);
    }
}