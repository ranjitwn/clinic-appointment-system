using ExamProject2.API.Data;
using Microsoft.EntityFrameworkCore;
using ExamProject2.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ExamProject2.API.Services;
using ExamProject2.API.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySQL(connectionString);
});

// CORS configuration to allow frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173","https://app.ranjitnair.dev") // React Vite default and Azure Static Web Apps URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddMySql(
        connectionString,
        name: "mysql",
        timeout: TimeSpan.FromSeconds(5));


builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Clinic Appointment Booking API",
        Description = "API for managing clinics, doctors, patients, and appointments."
    });

    // Enable XML comments (same as class)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory, xmlFilename)
    );

    // JWT Bearer configuration for Swagger)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT Authentication Configuration
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings configuration missing.");

builder.Services.AddSingleton(jwtSettings);

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<AppointmentService>();

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<PatientService>();

builder.Services.AddScoped<DoctorService>();

builder.Services.AddScoped<ClinicService>();

builder.Services.AddScoped<CategoryService>();

builder.Services.AddScoped<SpecialityService>();



var app = builder.Build();

var forwardOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
};

app.UseForwardedHeaders(forwardOptions);

// Seed Admin User 
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    var adminEmail = builder.Configuration["SeedAdmin:Email"];
    var adminPassword = builder.Configuration["SeedAdmin:Password"];

    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPassword) &&
        !context.Patients.Any(p => p.Role == "Admin"))
    {
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Patient>();

        var admin = new Patient
        {
            FirstName = "Admin",
            LastName = "User",
            Email = adminEmail,
            DateOfBirth = new DateTime(2000, 1, 1),
            IsRegistered = true,
            Role = "Admin"
        };

        admin.Password = hasher.HashPassword(admin, adminPassword);

        context.Patients.Add(admin);
        context.SaveChanges();
    }
}



// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExamProject2.API v1");
    c.RoutePrefix = "doc";
});


app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        name = "Clinic Appointment Booking API",
        version = "1.0",
        environment = "Production",
        documentation = "https://api.ranjitnair.dev/doc",
        repository = "https://github.com/ranjitwn/clinic-appointment-system",
        status = "Running",
        timestamp = DateTime.UtcNow
    });
})
.WithTags("System")
.WithSummary("API service information")
.WithDescription("Returns basic information about the Clinic Appointment Booking API service.");

app.MapHealthChecks("/health");

app.Run();
