🔐 What Are Claims?

A claim is a piece of information about the user stored inside the JWT token.

Examples of claims:

User ID
Email
Role
Name

Claims allow the server to identify the user and control access.

1️⃣ Claims Are Created During Login

In your AuthService you created claims here:

var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
    new Claim(ClaimTypes.Email, patient.Email),
    new Claim(ClaimTypes.Role, patient.Role)
};
What this means

You are storing inside the token:

Claim	Meaning
NameIdentifier	User ID
Email	User email
Role	User role (Patient / Admin)

Example values:

NameIdentifier = 5
Email = ranjit@test.com
Role = Patient
2️⃣ Claims Are Stored Inside the JWT Token

When the token is created:

var jwttoken = new JwtSecurityToken(
    issuer: _jwtSettings.Issuer,
    audience: _jwtSettings.Audience,
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
    signingCredentials: creds
);

The claims become part of the JWT payload.

Example decoded JWT payload:

{
  "nameid": "5",
  "email": "ranjit@test.com",
  "role": "Patient",
  "exp": 1712345678
}
3️⃣ Client Sends JWT With Every Request

After login, the client stores the token.

Every request to protected endpoints includes:

Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

Example request:

GET /api/Appointment/my
Authorization: Bearer TOKEN
4️⃣ ASP.NET JWT Middleware Validates the Token

In Program.cs you configured:

builder.Services
.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

When a request arrives:

Request
↓
JWT Middleware
↓
Validate token signature
↓
Validate expiration
↓
Validate issuer/audience
↓
Extract claims
↓
Create HttpContext.User
5️⃣ Claims Become Available in the Controller

After validation, ASP.NET populates:

HttpContext.User

You used this in your controller:

var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

This retrieves:

PatientId

Example value:

5

Now the controller knows which user made the request.

6️⃣ Role Claim Controls Authorization

You used:

[Authorize(Roles = "Patient")]

or

[Authorize(Roles = "Admin")]

ASP.NET checks the claim:

ClaimTypes.Role

Example token claim:

Role = Patient

If the endpoint requires Admin:

Access denied → 403 Forbidden
7️⃣ Real Example From Your Project

Example endpoint:

[Authorize(Roles = "Patient")]
[HttpGet("my")]
public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments()

Flow:

Client sends token
↓
JWT middleware validates token
↓
Claims extracted
↓
Role checked (Patient)
↓
Controller runs
↓
PatientId extracted from claim
↓
Service fetches appointments
🔎 Visual Flow
Login
↓
AuthService creates claims
↓
JWT token issued
↓
Client stores token
↓
Client sends token in Authorization header
↓
JWT middleware validates token
↓
Claims loaded into HttpContext.User
↓
[Authorize] checks role
↓
Controller reads claims
↓
Service processes request
Most Important Claims in Your Project
Claim	Used For
NameIdentifier	Identify the patient
Email	Optional identity info
Role	Authorization
Interview Questions You May Get
What is a claim?

A claim is a piece of user information stored inside a JWT token, such as user ID, email, or role.

How do you access claims in ASP.NET?

Claims are available through HttpContext.User and can be retrieved using methods like User.FindFirstValue().

How does role-based authorization work?

The JWT contains a role claim. The [Authorize(Roles = "...")] attribute checks that claim before allowing access.

Why use claims instead of database queries?

Claims allow the server to identify the user without querying the database on every request, improving performance.

What You Should Remember for Jobs

You do NOT need to remember JWT code.

But you must understand:

JWT contains claims

Claims identify the user

Claims control authorization

ASP.NET extracts claims from the token

Controllers read claims using User

That is what companies expect.

If You Want to Go One Step Deeper (Very Useful)

The next thing that would really strengthen your understanding is:

🔑 ASP.NET Core Request Pipeline

You already used:

UseAuthentication()
UseAuthorization()
UseMiddleware()

Understanding how a request moves through the pipeline is extremely valuable in interviews.

If you want, I can show you how your entire API processes a request from start to finish using your own project.