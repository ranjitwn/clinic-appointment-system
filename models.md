📘 Topic: Models in EF Core (Code-First)
What Is a Model?

A model (also called an Entity) represents a table in the database.

Each:

Property → becomes a column

Class → becomes a table

Relationship → becomes a foreign key relationship

In EF Core Code-First, models define the database structure.

Why Do We Define Models First?

Because in Code-First approach:

Models define the structure of the database.

DataContext registers those models.

Migrations generate the database from those models.

Without models, EF Core has nothing to build.

What Does a Model Do in My Project?

In this project, models:

Define database tables (Patient, Doctor, Appointment, etc.)

Define relationships (e.g., Appointment → Patient)

Define foreign keys

Define navigation properties

Example conceptually:

public class Appointment
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }
}

This creates:

Appointments table

PatientId foreign key column

Relationship between Appointment and Patient

Key Concepts in Models
1️⃣ Primary Key

Usually:

public int Id { get; set; }

EF Core automatically treats Id as primary key.

2️⃣ Foreign Key

Example:

public int PatientId { get; set; }

This becomes a foreign key column in database.

3️⃣ Navigation Property

Example:

public Patient Patient { get; set; }

This allows:

Loading related data

Using .Include() in queries

Object-based relationship navigation

Why Models Are Important

Models:

Define the structure of the database

Define relationships

Control how EF Core tracks data

Serve as the domain representation of business entities

What Models Do NOT Do

Models:

Do NOT handle business logic

Do NOT validate requests

Do NOT control API responses

That is the job of:

Services

DTOs

Controllers

Interview Answer (Short Version)

In EF Core Code-First, models represent database tables. Each class becomes a table, properties become columns, and navigation properties define relationships. We define models first, then register them in DbContext, and generate migrations to create the database.