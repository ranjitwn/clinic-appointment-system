📘 File: AppointmentService.cs
Responsibility

AppointmentService contains the business logic for managing appointments.

It is responsible for:

Creating appointments

Updating appointments

Deleting appointments

Retrieving appointments

Validating appointment rules

Preventing scheduling conflicts

It sits between:

Controller → Service → DataContext → Database

Controllers call this service to perform operations.

Dependency Injection
private readonly DataContext _dataContext;
private readonly PatientService _patientService;
Why these are injected

DataContext

Used to access the database through EF Core.

Example:

Appointments
Doctors
Patients
Categories
Clinics

PatientService

Used to create guest patients when a booking is made without login.

This keeps logic reusable instead of duplicating it.

Constructor
public AppointmentService(DataContext dataContext, PatientService patientService)
{
    _dataContext = dataContext;
    _patientService = patientService;
}

ASP.NET Core injects dependencies automatically through Dependency Injection.

Configured in Program.cs:

builder.Services.AddScoped<AppointmentService>();
DTO Mapping
private static AppointmentDto MapToDto(Appointment a)

Purpose:

Convert database entities into DTO objects returned to the API client.

Example transformation:

Appointment entity
↓
AppointmentDto

Why this matters:

Prevent exposing full database model

Format data for API responses

Combine related data like DoctorName

Example:

Doctor.FirstName + Doctor.LastName → DoctorName
GetMyAppointmentsAsync
public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(int patientId)

Purpose:

Returns appointments for the logged-in patient.

Steps:

Query appointments

Filter by patientId

Load related entities using .Include()

Sort by appointment date

Convert entities to DTOs

Example query:

_dataContext.Appointments
    .Where(a => a.PatientId == patientId)

.Include() loads related tables:

Clinic
Doctor
Category

Without Include, navigation properties would be null.

CreateAppointmentAsync
public async Task<string?> CreateAppointmentAsync(...)

Purpose:

Creates a new appointment with extensive validation rules.

Returns:

null → success
string → validation error
Guest Booking Logic

If the user is not logged in:

patientId == null

The system:

Validates guest data

Creates a temporary patient

Retrieves the created patient ID

This allows guest appointment booking.

Duration Validation

Allowed durations:

15
30
45
60 minutes

Prevents unrealistic values.

Foreign Key Validation

Ensures referenced records exist.

Checks:

Clinic exists
Doctor belongs to clinic
Category exists

Example:

Doctors.AnyAsync(d => d.Id == dto.DoctorId && d.ClinicId == dto.ClinicId)

This prevents invalid database relationships.

Appointment Time Rules

Your service enforces real-world constraints:

No weekend bookings
Saturday
Sunday
No past appointments
AppointmentDate < DateTime.Now
Clinic working hours
08:00 – 18:00
Appointment must finish before closing
EndTime <= 18:00
15-minute time intervals

Allowed start minutes:

00
15
30
45
Doctor Conflict Detection

Prevents double booking of doctors.

Logic:

new appointment start < existing appointment end
AND
new appointment end > existing appointment start

This checks time overlap.

Patient Conflict Detection

Ensures a patient cannot book two appointments at the same time.

Same overlap logic used.

Saving the Appointment

If all validations pass:

_dataContext.Appointments.Add(appointment);
await _dataContext.SaveChangesAsync();

EF Core inserts the record into the database.

UpdateAppointmentAsync

Purpose:

Allow patients to modify an existing appointment.

Validation includes:

Ownership check

Duration validation

Weekend restriction

Clinic hours restriction

Doctor conflict detection

Patient conflict detection

If validation passes:

appointment.AppointmentDate = dto.AppointmentDate;
appointment.DurationMinutes = dto.DurationMinutes;

await _dataContext.SaveChangesAsync();

EF Core updates the database record.

DeleteAppointmentAsync

Purpose:

Delete an appointment.

Steps:

Verify appointment belongs to the patient

Remove record

Save changes

Returns:

true → deleted
false → not found
GetAppointmentsByClinicAsync

Returns appointments filtered by clinic.

Used for admin or clinic views.

GetAppointmentsByDoctorAsync

Returns appointments for a specific doctor.

Used for doctor schedules.

GetAvailableSlotsAsync

Calculates available time slots.

Steps:

Prevent weekend slots

Prevent past dates

Define clinic hours (08–18)

Load existing appointments

Generate time slots

Remove conflicting slots

Example output:

09:00
09:30
10:00
11:15

Used by frontend to display available booking times.

Key Architectural Concepts Demonstrated

This service demonstrates:

• Separation of concerns
• Business rule enforcement
• Database querying with EF Core
• DTO mapping
• Conflict detection algorithms
• Guest vs registered user workflows

Interview Questions
Q: What is the role of a service layer?

Service layer contains the business logic of the application and separates controllers from data access logic.

Q: Why not place this logic in controllers?

Controllers should only handle HTTP concerns. Business rules belong in services to keep controllers clean and reusable.

Q: Why convert entities to DTOs?

DTOs prevent exposing database structure and allow control over API response shape.

Q: How do you prevent double booking?

By checking for overlapping appointment time ranges using start and end comparisons.

Q: Why use .Include() in queries?

To load related entities such as Doctor or Clinic that are referenced by navigation properties.

Self-Check

You understand this service if you can explain:

Why services exist

How EF Core queries work

How appointment conflicts are detected

Why DTO mapping is necessary


📘 Return Types in Async Service Methods

In your service you used:

Task<bool>
Task<string?>
Task<List<T>>

These represent asynchronous return types.

1️⃣ Why Task Is Used

In ASP.NET Core, database operations are asynchronous.

Example:

await _dataContext.Appointments.ToListAsync();

The database query takes time.
Instead of blocking the thread, ASP.NET uses async/await.

Because of that, the method must return a Task.

Example:

public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(...)

Meaning:

"This method runs asynchronously and will eventually return a List of AppointmentDto."

2️⃣ Task<List<T>>

Example from your code:

Task<List<AppointmentDto>>

Meaning:

The method runs asynchronously

When finished, it returns a list of DTO objects

Example method:

public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(int patientId)

Returned value example:

[
  { appointment1 },
  { appointment2 },
  { appointment3 }
]
3️⃣ Task<bool>

Example:

public async Task<bool> DeleteAppointmentAsync(int id, int patientId)

Meaning:

Method runs asynchronously

Returns true or false

Example logic:

true → appointment deleted
false → appointment not found

This is commonly used for success/failure operations.

4️⃣ Task<string?>

Example:

public async Task<string?> CreateAppointmentAsync(...)

Meaning:

Method runs asynchronously

Returns either:

null → success
string → error message

Example:

null

means success.

Example error:

"Doctor is already booked at this time."

The ? means the value can be nullable.

Why This Pattern Was Used in Your Project

Your project uses this pattern:

Success → null
Error → message

This keeps the service simple.

Controller then decides how to respond.

Example:

if (error != null)
    return BadRequest(error);
Alternative Patterns (You Might See in Jobs)

Some companies use different patterns.

Example:

Option 1
Task<Result>

Where Result contains:

Success
ErrorMessage
Data
Option 2
Task<IActionResult>

But that is usually used in controllers, not services.

Interview Question You Might Get
Q: Why do service methods return Task?

Answer:

Because database operations are asynchronous. Using Task allows the thread to be released while waiting for the database, improving scalability and performance in web applications.

Q: When would you return bool vs a DTO?

Answer:

A boolean is used when only success or failure matters, such as deleting a record. A DTO is returned when the client needs actual data.

Q: Why use nullable string for errors?

Answer:

It allows the method to return either an error message or null for success without creating complex response objects.

Simple Mental Model
Task<T>

means:

“This method will eventually return T.”

Examples:

Task<List<AppointmentDto>> → returns list
Task<bool> → returns success/failure
Task<string?> → returns error message or null
Self Check

You understand this if you can answer:

1️⃣ Why async methods return Task
2️⃣ Why database queries use await
3️⃣ When to return bool, DTO, or error string