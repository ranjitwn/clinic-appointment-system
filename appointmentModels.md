📘 File: Appointment.cs
Responsibility

The Appointment model represents the Appointments table in the database.

It defines:

The structure of the appointment record

Relationships to other entities

Foreign keys to related tables

In EF Core Code-First, this class is used to generate the database schema.

Structure of the Model
public class Appointment
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public int DurationMinutes { get; set; }

    public int ClinicId { get; set; }
    public Clinic? Clinic { get; set; }

    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
What Each Property Does
Primary Key
public int Id { get; set; }

EF Core automatically treats Id as the primary key.

Database result:

Appointments
------------
Id (PK)
Appointment Data Fields
public DateTime AppointmentDate { get; set; }
public int DurationMinutes { get; set; }

These represent appointment details.

Database columns:

AppointmentDate
DurationMinutes
Foreign Keys

These properties define relationships.

Example:

public int ClinicId { get; set; }

This creates a foreign key column in the database.

ClinicId → references Clinics table

Same pattern applies to:

DoctorId
PatientId
CategoryId
Navigation Properties

Example:

public Clinic? Clinic { get; set; }

This allows EF Core to navigate relationships.

Example usage in queries:

.Include(a => a.Clinic)

This loads the related clinic.

Navigation properties represent the object relationship, not the column.

Why Foreign Key + Navigation Property Both Exist

Example:

public int ClinicId { get; set; }
public Clinic? Clinic { get; set; }

They serve different purposes.

ClinicId → database column
Clinic → object relationship

EF Core uses both together to manage relationships.

Database Relationship Result

Your model creates relationships like this:

Appointments
   │
   ├── ClinicId → Clinics
   ├── DoctorId → Doctors
   ├── PatientId → Patients
   └── CategoryId → Categories

This means:

One appointment belongs to:

One clinic

One doctor

One patient

One category

But each of those can have many appointments.

Example:

Doctor 1 → many appointments

This is a one-to-many relationship.

Why Navigation Properties Are Nullable
Clinic?
Doctor?
Patient?
Category?

The ? means nullable reference type.

This avoids null reference exceptions when:

Data is not loaded

.Include() was not used

Lazy loading is disabled

What This Model Does NOT Do

Models should not contain:

Business logic

Validation logic

API formatting

HTTP behavior

Those belong in:

Controllers
Services
DTOs

Interview Questions & Answers
Q: What is an EF Core entity?

An entity is a class that represents a table in the database. Each property becomes a column, and relationships are defined using foreign keys and navigation properties.

Q: What is the difference between a foreign key and a navigation property?

A foreign key represents the database column that stores the relationship ID. A navigation property represents the object relationship that allows EF Core to load related entities.

Q: What relationship does Appointment have with Doctor?

It is a many-to-one relationship. Many appointments can belong to one doctor.

Q: Why not return this model directly from the API?

Because EF models represent database structure and may expose unnecessary or sensitive fields. DTOs are used to control what data is returned to clients.

Q: Why are navigation properties nullable?

Because EF Core may not always load related data unless .Include() is used.

Self-Check

You understand this model if you can answer:

What creates the primary key?

What creates the foreign keys?

What does the navigation property allow EF to do?

What relationship exists between Doctor and Appointment?

Next Step in the Cycle

Now we move to the file that registers this model with EF Core:

📘 DataContext.cs

Because DataContext tells EF:

“Track Appointment as a database table.”

Paste your DataContext.cs next.