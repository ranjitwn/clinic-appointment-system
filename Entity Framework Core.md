What Entity Framework Core Actually Is

Entity Framework Core (EF Core) is an ORM (Object Relational Mapper).

ORM means:

C# Objects  ↔  Database Tables

Example in your project:

C# Model	Database Table
Patient	Patients
Doctor	Doctors
Clinic	Clinics
Appointment	Appointments

So instead of writing SQL like:

SELECT * FROM Appointments

you write:

_dataContext.Appointments.ToListAsync();

EF Core converts this to SQL automatically.

1️⃣ DbContext – The Heart of EF Core

Your file:

public class DataContext : DbContext

Think of DbContext as a database session.

Responsibilities:

Manage database connection

Track entity changes

Execute queries

Save data

Example in your project:

private readonly DataContext _dataContext;

Services use this to talk to the database.

2️⃣ DbSet – Represents a Table

In your DataContext:

public DbSet<Clinic> Clinics { get; set; }
public DbSet<Doctor> Doctors { get; set; }
public DbSet<Patient> Patients { get; set; }
public DbSet<Appointment> Appointments { get; set; }

Each DbSet represents a table.

Example:

_dataContext.Appointments

means:

Appointments table
3️⃣ LINQ → SQL Translation

Your code:

var appointments = await _dataContext.Appointments
    .Where(a => a.PatientId == patientId)
    .ToListAsync();

EF Core converts this to SQL.

Generated SQL:

SELECT *
FROM Appointments
WHERE PatientId = 5

LINQ allows writing database queries in C#.

4️⃣ Include() – Loading Related Data

Your service used:

.Include(a => a.Clinic)
.Include(a => a.Doctor)
.Include(a => a.Category)

Without this:

Clinic = null
Doctor = null

Because EF Core does lazy loading disabled by default.

With Include():

EF Core generates SQL with JOINs.

Example SQL:

SELECT *
FROM Appointments
JOIN Clinics ON Clinics.Id = Appointments.ClinicId
JOIN Doctors ON Doctors.Id = Appointments.DoctorId
JOIN Categories ON Categories.Id = Appointments.CategoryId

This is called Eager Loading.

5️⃣ Change Tracking

EF Core tracks entities automatically.

Example:

var appointment = await _dataContext.Appointments.FirstAsync();
appointment.DurationMinutes = 60;

await _dataContext.SaveChangesAsync();

EF Core detects:

DurationMinutes changed

Then generates SQL:

UPDATE Appointments
SET DurationMinutes = 60
WHERE Id = 1

You don’t write the SQL.

6️⃣ SaveChangesAsync()

This method:

await _dataContext.SaveChangesAsync();

commits changes to the database.

EF Core collects all changes:

INSERT
UPDATE
DELETE

and sends SQL commands.

Example From Your Code

Create appointment:

_dataContext.Appointments.Add(appointment);
await _dataContext.SaveChangesAsync();

Generated SQL:

INSERT INTO Appointments (...)
VALUES (...)
7️⃣ Add / Update / Remove
Insert
_dataContext.Appointments.Add(appointment);

SQL:

INSERT INTO Appointments
Update
appointment.DurationMinutes = 45;

SQL:

UPDATE Appointments
SET DurationMinutes = 45
Delete
_dataContext.Appointments.Remove(appointment);

SQL:

DELETE FROM Appointments
WHERE Id = 5
8️⃣ Async Database Queries

Your code uses:

ToListAsync()
FirstOrDefaultAsync()
AnyAsync()

This prevents blocking the thread.

Example:

await _dataContext.Appointments.ToListAsync();

Without async:

thread blocked while DB responds

With async:

thread freed → better scalability

This is critical for web APIs.

9️⃣ Navigation Properties

Your model:

public int DoctorId { get; set; }
public Doctor? Doctor { get; set; }

This allows:

appointment.Doctor.FirstName

instead of manual joins.

EF Core manages relationships automatically.

🔗 Relationships in Your DataContext

You configured:

modelBuilder.Entity<Doctor>()
    .HasMany(d => d.Appointments)
    .WithOne(a => a.Doctor)
    .HasForeignKey(a => a.DoctorId);

This defines:

Doctor
   ↓
many
Appointments

Database result:

DoctorId FOREIGN KEY
10️⃣ EF Core Query Example From Your Project

Your code:

var appointments = await _dataContext.Appointments
    .Where(a => a.ClinicId == clinicId)
    .Include(a => a.Clinic)
    .Include(a => a.Doctor)
    .Include(a => a.Category)
    .OrderBy(a => a.AppointmentDate)
    .ToListAsync();

EF Core roughly generates:

SELECT *
FROM Appointments
JOIN Clinics
JOIN Doctors
JOIN Categories
WHERE ClinicId = 2
ORDER BY AppointmentDate
Most Important EF Core Interview Questions
What is DbContext?

DbContext represents a session with the database and is responsible for querying and saving data.

What is DbSet?

DbSet represents a table in the database and allows querying and manipulating entities.

What is LINQ?

LINQ is a C# query language used to retrieve data from collections and databases.

What does Include() do?

Include() loads related entities from the database using eager loading.

What does SaveChangesAsync() do?

It commits tracked changes (insert, update, delete) to the database.

Why use async database calls?

Async queries prevent blocking threads and improve scalability in web applications.

Important Real-World Concepts You Should Know Next

Now that you understand EF Core basics, the next deeper topics companies expect are:

🔹 Tracking vs NoTracking
🔹 Lazy vs Eager Loading
🔹 N+1 Query Problem
🔹 Query Performance
🔹 Migrations
🔹 Transactions

Interview Answer (Good One)

If someone asks:

“What is Entity Framework Core?”

You can answer:

Entity Framework Core is an ORM for .NET that allows developers to interact with databases using C# objects and LINQ queries. It translates LINQ queries into SQL, maps database tables to C# classes, and tracks changes to entities so they can be persisted to the database.

That answer is exactly what companies want.

What is the N+1 query problem?

Answer:

The N+1 query problem occurs when an application executes one query to load a collection of entities and then executes additional queries for each related entity. This results in many database calls and poor performance.

How do you solve it in EF Core?

Answer:

By using eager loading with the Include() method to load related entities in a single query.