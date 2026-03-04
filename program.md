📘 FILE: Program.cs
1️⃣ Responsibility (What This File Does)

Program.cs is responsible for:

Bootstrapping the ASP.NET Core application

Registering services (Dependency Injection)

Configuring authentication & authorization

Configuring middleware pipeline

Seeding initial data

Starting the web server

It wires everything together.

It does not contain business logic.

2️⃣ How It Works In Your Project (Flow)

Let’s follow it top to bottom.

🔹 A. Database Configuration
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});
What happens:

Registers EF Core

Uses MySQL provider

Reads connection string from appsettings.json

Why Scoped?

DbContext is created once per HTTP request.

Interview line:

DbContext is scoped because it tracks changes per request and must not be shared between users.

🔹 B. CORS Configuration
builder.Services.AddCors(...)

Allows:

http://localhost:5173

Which is your React Vite frontend.

Without this:
👉 Browser blocks requests (CORS error)

Interview line:

CORS is enforced by browsers to prevent cross-origin security issues.

🔹 C. Controllers
builder.Services.AddControllers();

Enables:

[ApiController]

Routing

Model binding

JSON serialization

🔹 D. Swagger + JWT Support

You did something good here.

You:

Enabled XML comments

Added JWT security definition

Configured Swagger UI at /doc

This shows:
You understand API documentation.

Interview line:

Swagger helps document and test APIs, and I configured JWT support to test secured endpoints.

You should remember this conceptually:

AddSwaggerGen() → enables Swagger

SwaggerDoc() → sets title & version

IncludeXmlComments() → shows method summaries

AddSecurityDefinition() → enables JWT support

AddSecurityRequirement() → forces JWT for protected routes

Interview-Level Explanation

If asked:

How did you document your API?

You say:

I used Swagger with OpenAPI. I enabled XML comments for method documentation and configured JWT Bearer authentication so I could test secured endpoints directly in Swagger.

🔐 E. JWT Configuration

This is important.

1️⃣ Bind settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

You manually bind configuration to a class and register it as Singleton.

Why Singleton?
Because:

It’s static configuration.

Same for entire application.

No need to recreate per request.

Good decision.

2️⃣ Authentication Setup
builder.Services.AddAuthentication(...)

You configured:

ValidateIssuer

ValidateAudience

ValidateLifetime

ValidateIssuerSigningKey

This ensures:

Token must be from your system

Token must be intended for your API

Token must not be expired

Signature must match secret key

That is production-level thinking.

🔒 F. Authorization
builder.Services.AddAuthorization();

Enables:

[Authorize]

Role checks

🔹 G. Service Registration
builder.Services.AddScoped<AppointmentService>();
...

You are using:

Scoped lifetime (correct for services using DbContext)

Why Scoped?
Because:

They depend on DbContext

DbContext is Scoped

Scoped service cannot depend on Transient incorrectly

Interview line:

Services are scoped to match DbContext lifetime and avoid shared state between requests.

🔹 H. Admin Seeding

Very good feature.

You:

Create scope manually

Resolve DataContext

Check if admin exists

Hash password

Insert admin if missing

This shows:

Understanding of DI scope

Understanding of PasswordHasher

Understanding of startup data seeding

Interview gold.

If asked:

How did you seed admin user?

You can explain this confidently.

🔹 I. Middleware Pipeline (VERY IMPORTANT)

Order matters.

app.UseSwagger();
app.UseSwaggerUI();

Swagger middleware.

app.UseHttpsRedirection();

Redirect HTTP → HTTPS.

app.UseMiddleware<GlobalExceptionMiddleware>();

This is important.

Your custom middleware:

Catches unhandled exceptions

Returns structured JSON error

Very professional touch.

app.UseCors("AllowFrontend");

Must come before controllers.

app.UseAuthentication();
app.UseAuthorization();

Correct order.

Authentication → builds identity
Authorization → checks permissions

app.MapControllers();

Maps attribute-based routes.

app.Run();

Starts server.

3️⃣ What Breaks If Removed?
Removed	Result
AddDbContext	No DB access
CORS	Browser blocks frontend
AddAuthentication	JWT not validated
UseAuthentication	HttpContext.User always empty
AddAuthorization	[Authorize] ignored
GlobalExceptionMiddleware	Raw 500 errors exposed
AddScoped Services	Controllers fail to resolve dependencies
Admin seeding	No initial admin account

This shows architectural understanding.

4️⃣ Interview Answers You Can Use
Q: Explain ASP.NET Core request lifecycle.

Request enters Kestrel, goes through middleware pipeline (CORS, authentication, authorization), then model binding maps request to controller, service executes business logic, EF Core interacts with database, and response is serialized to JSON.

Q: Why use middleware for exception handling?

It centralizes error handling, ensures consistent JSON responses, and prevents exposing internal stack traces.

Q: Why Scoped services?

Because they depend on DbContext, which is scoped per request. This avoids concurrency and shared state issues.

Q: How does JWT validation work?

The JwtBearer middleware validates signature, expiration, issuer, and audience, then builds a ClaimsPrincipal assigned to HttpContext.User.

5️⃣ Mini Self-Check

You should now be able to answer:

Why is CORS needed?

Why is JwtSettings Singleton?

Why services are Scoped?

Why Authentication must come before Authorization?

What GlobalExceptionMiddleware does?

If yes → Program.cs understood.

Interview Questions – Program.cs (Based on Your Project)
1️⃣ Q: What is the purpose of Program.cs in ASP.NET Core?

Answer:

Program.cs is the entry point of the ASP.NET Core application. It configures services using dependency injection, sets up authentication and authorization, configures middleware, and starts the web server. It essentially wires the entire application together.

2️⃣ Q: What happens when you call builder.Build()?

Answer:

builder.Build() finalizes the service registrations and constructs the dependency injection container. It prepares the middleware pipeline and creates the WebApplication instance. After this point, services cannot be modified.

3️⃣ Q: Why did you use AddScoped for your services?

Answer:

I used scoped services because they depend on DbContext, which is also scoped per HTTP request. This ensures that each request gets its own instance and prevents shared state between users.

4️⃣ Q: What is the difference between Singleton, Scoped, and Transient lifetimes?

Answer:

Singleton is created once for the entire application lifetime.
Scoped is created once per HTTP request.
Transient is created every time it is requested.
In my project, services are scoped because they depend on DbContext.

5️⃣ Q: Why is CORS configured in Program.cs?

Answer:

CORS is required because the frontend runs on a different origin than the backend. Browsers block cross-origin requests by default, so I configured a policy to allow requests from my React frontend.

6️⃣ Q: Why must UseAuthentication() come before UseAuthorization()?

Answer:

Authentication validates the JWT and builds the user identity. Authorization checks permissions based on that identity. If authorization runs first, there is no identity, and all protected endpoints return 401.

7️⃣ Q: What would happen if you removed AddAuthentication() but kept [Authorize]?

Answer:

The application would not know how to validate JWT tokens. Protected endpoints would fail because no authentication scheme is configured, and requests would return 401 Unauthorized.

8️⃣ Q: Why did you create a custom GlobalExceptionMiddleware?

Answer:

To centralize error handling and ensure consistent JSON error responses. It prevents exposing stack traces and avoids repeating try-catch blocks in controllers.

9️⃣ Q: Why did you register JwtSettings as Singleton?

Answer:

JwtSettings contains static configuration values like secret key, issuer, and audience. These do not change per request, so Singleton is appropriate and efficient.

🔟 Q: What is middleware in ASP.NET Core?

Answer:

Middleware are components that form a request pipeline. Each middleware can inspect, modify, or short-circuit the request and response. Examples in my project include authentication, CORS, and exception handling.

1️⃣1️⃣ Q: Why did you seed the Admin user in Program.cs?

Answer:

I seeded an admin user at startup to ensure there is always at least one administrator account available. I used a scoped service provider to access the database safely during application startup.

This answer shows maturity.

1️⃣2️⃣ Q: If Swagger was removed, would your API still work?

Answer:

Yes. Swagger is only for documentation and testing. The API functionality would still work, but we would lose interactive documentation and the ability to test endpoints through the Swagger UI.

That’s a good conceptual understanding check.

🧠 If You Can Answer These Calmly…

You are fully solid on Program.cs.

Not memorizing code.

Understanding architecture.

That is what interviewers look for.