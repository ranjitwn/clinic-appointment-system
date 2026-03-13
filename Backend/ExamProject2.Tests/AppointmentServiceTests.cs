using ExamProject2.API.Data;
using ExamProject2.API.DTOs;
using ExamProject2.API.Models;
using ExamProject2.API.Services;
using Microsoft.EntityFrameworkCore;
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
        return new PatientService(db);
    }

    [Fact]
    public async Task CreateAppointment_ShouldFail_WhenWeekend()
    {
        var db = GetDb();

        db.Clinics.Add(new Clinic { Id = 1, Name = "Clinic A" });
        db.Doctors.Add(new Doctor { Id = 1, FirstName = "Doc", LastName = "One", ClinicId = 1, SpecialityId = 1 });
        db.Categories.Add(new Category { Id = 1, Name = "General" });

        await db.SaveChangesAsync();

        var service = new AppointmentService(db, GetPatientService(db));

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
            DateOfBirth = new DateTime(1990,1,1)
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

        var service = new AppointmentService(db, GetPatientService(db));

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
            DateOfBirth = new DateTime(1990,1,1)
        };

        var result = await service.CreateAppointmentAsync(dto, null);

        Assert.Equal("Invalid duration.", result);
    }

    [Fact]
    public async Task DeleteAppointment_ShouldReturnFalse_WhenNotFound()
    {
        var db = GetDb();
        var service = new AppointmentService(db, GetPatientService(db));

        var result = await service.DeleteAppointmentAsync(1, 1);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAvailableSlots_ShouldReturnSlots()
    {
        var db = GetDb();
        var service = new AppointmentService(db, GetPatientService(db));

        var slots = await service.GetAvailableSlotsAsync(
            doctorId: 1,
            date: new DateTime(2030, 6, 3),
            slotDuration: 30
        );

        Assert.NotEmpty(slots);
    }
}