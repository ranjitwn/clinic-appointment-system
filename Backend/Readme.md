# Clinic Appointment Backend API

Backend REST API for the **Clinic Appointment Booking System**, built with **ASP.NET Core (.NET 9)** and **Entity Framework Core** using **MySQL** as the database.

The API manages appointment scheduling, patient authentication, clinic administration, and doctor management while enforcing scheduling validation rules and secure role-based access control.

---

# API Documentation

Swagger API documentation is available when the API is running:

```
http://localhost:5108/doc
```

Live production API documentation:

```
https://api.ranjitnair.dev/doc
```

---

# Technology Stack

### Backend Framework

* ASP.NET Core (.NET 9)
* Entity Framework Core (Code-First)
* MySQL database

### Security

* JWT Authentication
* Role-based authorization
* Password hashing

### API Infrastructure

* Swagger / OpenAPI documentation
* Global exception handling middleware
* DTO-based API contracts

### Testing

* xUnit
* Entity Framework Core InMemory database provider

---

# Running the API Locally

Navigate to the backend project:

```bash
cd Backend/ExamProject2.API
```

Restore dependencies:

```bash
dotnet restore
```

Apply database migrations:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

The API will start on:

```
http://localhost:5108
```

Swagger documentation:

```
http://localhost:5108/doc
```

---

# Configuration

Sensitive configuration values are managed through **environment variables**.

Examples include:

* Database connection string
* JWT signing key
* Admin seed credentials

In production these values are stored securely in **Azure App Service configuration**.

Example configuration keys:

```
JwtSettings__SecretKey
JwtSettings__Issuer
JwtSettings__Audience
JwtSettings__ExpiryMinutes
SeedAdmin__Email
SeedAdmin__Password
```

This prevents secrets from being stored inside the repository.

---

# Project Architecture

The backend follows a layered architecture separating controllers, business logic, and data access.

```
ExamProject2.API
│
├── Controllers
├── Services
├── DTOs
├── Models
├── Data
├── Middleware
└── Migrations
```

### Controllers

Handle HTTP requests and responses.

### Services

Contain business logic and application rules.

### DTOs

Define structured request and response contracts.

### Models

Entity models mapped to database tables using EF Core.

### DataContext

Entity Framework database context responsible for database access.

### Middleware

Centralized error handling using `GlobalExceptionMiddleware`.

---

# Authentication & Authorization

Authentication is implemented using **JWT tokens**.

Registered patients can log in and receive a token containing:

* Patient ID
* Email
* Role claim

Example authorization attribute:

```csharp
[Authorize(Roles = "Admin")]
```

Roles implemented:

```
Patient
Admin
```

Admin users have permissions to manage clinics, doctors, categories, and specialities.

---

# Appointment Booking Logic

The system enforces multiple validation rules to ensure realistic scheduling.

### Booking Validation

The API prevents:

* overlapping appointments for the same doctor
* overlapping appointments for the same patient
* bookings outside clinic working hours
* past appointments

### Scheduling Rules

Appointments must:

* occur between **08:00 and 18:00**
* avoid weekends
* follow valid durations

Allowed durations:

```
15 minutes
30 minutes
45 minutes
60 minutes
```

Time slots align to:

```
:00
:15
:30
:45
```

---

# Available Appointment Slots

An endpoint is available to generate available booking slots based on:

* clinic opening hours
* selected duration
* existing appointments

This assists the frontend in displaying available appointment times while keeping scheduling validation inside the backend.

---

# Global Exception Middleware

A custom `GlobalExceptionMiddleware` provides:

* consistent JSON error responses
* centralized error handling
* simplified controller logic

This ensures predictable API responses and improved debugging.

---

# Admin User Seeding

An administrator account is automatically created during application startup if one does not already exist.

The admin user is stored in the existing **Patients table** and assigned the role:

```
Admin
```

Credentials are loaded from environment configuration and the password is securely hashed before storage.

This allows immediate administrative access without manual database setup.

---

# Data Validation

Server-side validation is implemented using **DataAnnotations** on DTO models.

Examples include:

* required fields
* email format validation
* appointment duration rules
* registration validation

This ensures data integrity before persistence.

---

# System Endpoints

Additional endpoints are available for service monitoring.

API root:

```
GET /
https://api.ranjitnair.dev
```

Health check endpoint:

```
GET /health
https://api.ranjitnair.dev/health
```

These endpoints allow infrastructure or monitoring systems to verify that the service is operational.

---

# Automated Unit Testing

The backend includes unit tests for core business logic implemented in the service layer.

Testing is performed using:

* **xUnit**
* **Entity Framework Core InMemory database provider**

The InMemory provider allows tests to run without connecting to the real MySQL database.

This enables fast and isolated validation of:

* appointment scheduling logic
* authentication behavior
* business rules enforced in services

Example tested service:

```
AppointmentService
```

Tests validate rules such as:

* preventing double bookings
* preventing past appointments
* enforcing clinic working hours
* validating appointment duration

Run tests locally:

```bash
dotnet test
```

Tests are also executed automatically through **GitHub Actions CI/CD pipelines** whenever changes are pushed to the repository.

---

# Summary

The backend API provides:

* RESTful clinic appointment scheduling system
* guest and registered patient booking workflows
* JWT authentication
* role-based authorization
* DTO-based API contracts
* service-layer architecture
* scheduling conflict validation
* centralized error handling
* Swagger API documentation
* MySQL database using EF Core migrations
* cloud deployment support for Azure
