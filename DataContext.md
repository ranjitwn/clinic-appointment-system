📘 File: DataContext.cs
Responsibility

DataContext is the EF Core database context.

It is responsible for:

Connecting the application to the database

Tracking entity changes

Mapping models to database tables

Configuring relationships between entities

Executing database queries through EF Core

It acts as the bridge between the application and the database.

How It Is Defined
public class DataContext : DbContext

DbContext is the core EF Core class that manages database access.

Your DataContext inherits from it to define your application’s database structure.

Constructor
public DataContext(DbContextOptions<DataContext> options)
    : base(options)
{
}
What This Does

This allows ASP.NET Core to inject database configuration.

The configuration comes from Program.cs:

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});

This means:

EF Core connects to MySQL

Connection string comes from appsettings.json

DbSet Properties

These lines define the tables EF Core will create and track.

public DbSet<Clinic> Clinics { get; set; }
public DbSet<Doctor> Doctors { get; set; }
public DbSet<Speciality> Specialities { get; set; }
public DbSet<Patient> Patients { get; set; }
public DbSet<Category> Categories { get; set; }
public DbSet<Appointment> Appointments { get; set; }
What DbSet Does

A DbSet<T> represents a database table.

Example:

public DbSet<Appointment> Appointments

Creates a table:

Appointments

And allows queries like:

_dataContext.Appointments.ToListAsync();
Relationship Configuration

Relationships are defined inside:

protected override void OnModelCreating(ModelBuilder modelBuilder)

This method allows Fluent API configuration.

Fluent API is used when relationships need to be explicitly defined.

Example Relationship
modelBuilder.Entity<Clinic>()
    .HasMany(c => c.Doctors)
    .WithOne(d => d.Clinic)
    .HasForeignKey(d => d.ClinicId);
What This Means

One clinic can have many doctors.

Database result:

Clinics
   │
   └── Doctors
        └── ClinicId (FK)

So:

1 Clinic → many Doctors
Another Example
modelBuilder.Entity<Doctor>()
    .HasMany(d => d.Appointments)
    .WithOne(a => a.Doctor)
    .HasForeignKey(a => a.DoctorId);

This means:

1 Doctor → many Appointments

Each appointment belongs to one doctor.

Full Relationship Map

Your database relationships look like this:

Clinic
   │
   ├── Doctors
   │      └── Appointments
   │
   └── Appointments

Patient
   └── Appointments

Category
   └── Appointments

Speciality
   └── Doctors

This is a well-structured relational model.

Why Fluent API Is Used Here

Relationships could sometimes be inferred automatically.

But Fluent API ensures:

Clear relationship definitions

Explicit foreign keys

Predictable migrations

Better control over database schema

How DataContext Is Used In Services

Example:

private readonly DataContext _dataContext;

Injected into services.

Example usage:

var appointments = await _dataContext.Appointments
    .Include(a => a.Doctor)
    .Include(a => a.Patient)
    .ToListAsync();

This queries the database using EF Core.

What DataContext Does NOT Do

DataContext does not contain:

Business logic

API responses

Validation logic

Those belong in:

Services

Controllers

DTOs

Interview Questions & Answers
Q: What is DbContext?

DbContext is the main EF Core class responsible for managing database connections, tracking entities, and executing queries.

Q: What does DbSet represent?

A DbSet represents a table in the database and allows querying and saving instances of that entity.

Q: Why inject DbContext instead of creating it manually?

Because ASP.NET Core manages it through dependency injection, ensuring proper lifetime management and database connection handling.

Q: What is OnModelCreating used for?

It is used to configure entity relationships, constraints, and database schema using EF Core’s Fluent API.

Q: Why are your services Scoped?

Because DbContext is scoped per HTTP request, and services that depend on it must share the same lifetime.

Self-Check

You understand DataContext if you can explain:

What DbContext does

What DbSet represents

Why OnModelCreating exists

How EF Core builds database tables from models

Next Step in the Learning Cycle

Now we complete the first full backend cycle:

Model → DataContext → Service → Controller