## Authentication Flow (JWT)

Client (React / Postman)
        ↓
AuthController
        ↓
AuthService
        ↓
Password Hashing
        ↓
JWT Token Creation
        ↓
Token returned to client
        ↓
Client sends token in Authorization header
        ↓
ASP.NET JWT Middleware (Program.cs)
        ↓
HttpContext.User populated with claims
        ↓
[Authorize] endpoints allowed

1️⃣ AuthController – HTTP Entry Point

File:

AuthController.cs

Responsibility:

Receive login/register requests

Accept DTO input

Call AuthService

Return HTTP responses

Controller does not contain authentication logic.

Register Endpoint
[HttpPost("register")]
public async Task<IActionResult> Register(PatientRegisterDto dto)

Request:

POST /api/Auth/register

Example:

{
  "firstName": "Ranjit",
  "lastName": "Nair",
  "email": "ranjit@test.com",
  "password": "Test123!",
  "dateOfBirth": "1990-11-04"
}

Flow:

Client → Controller → AuthService → Database

Controller logic:

call service
↓
if error → 400
↓
if success → 200

Example response:

{
  "message": "Registration successful."
}
2️⃣ AuthService – Business Logic

File:

AuthService.cs

This file contains authentication logic.

Responsibilities:

Register patient

Verify login credentials

Hash passwords

Generate JWT tokens

Register Logic

Method:

RegisterAsync()

Step-by-step flow:

1️⃣ Validate DOB
if (dto.DateOfBirth >= DateTime.Today)

Prevents future dates.

2️⃣ Check existing email
var existing = await _dataContext.Patients

Ensures unique email.

3️⃣ Guest → Registered upgrade

Your project supports guest patients.

So if a guest exists:

existing.IsRegistered = true
existing.Password = hashedPassword

This converts a guest into a registered user.

Very good design 👍

4️⃣ Hash password
var hasher = new PasswordHasher<Patient>();

Password hashing:

hashedPassword = HashPassword()

Passwords are never stored as plain text.

Database stores something like:

AQAAAAEAACcQAAAAEB...
3️⃣ Login Flow

Controller endpoint:

POST /api/Auth/login

Example request:

{
  "email": "ranjit@test.com",
  "password": "Test123!"
}
Login Service Flow

Inside:

LoginAsync()

Steps:

1️⃣ Find patient by email
FirstOrDefaultAsync()
2️⃣ Check patient exists
if (patient == null || !patient.IsRegistered)

Guest patients cannot login.

3️⃣ Verify password

Password verification:

VerifyHashedPassword()

This compares:

entered password
vs
stored hashed password

If incorrect → return null.

4️⃣ JWT Token Creation

This is the most important part.

Step 1: Create Claims
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
    new Claim(ClaimTypes.Email, patient.Email),
    new Claim(ClaimTypes.Role, patient.Role)
};

Claims store user identity data inside the token.

Example payload:

UserId
Email
Role
Step 2: Create Signing Key
new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
)

The secret key signs the token.

Only the server knows this key.

Step 3: Signing Credentials
new SigningCredentials(key, SecurityAlgorithms.HmacSha256)

This signs the token using SHA256.

Step 4: Create JWT
new JwtSecurityToken(...)

Includes:

Issuer
Audience
Claims
Expiry
Signature
Step 5: Convert Token to String
WriteToken(jwttoken)

Result:

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Login Response

Your API returns:

{
  "token": "eyJhbGc...",
  "role": "Patient"
}

Frontend stores this token.

5️⃣ Client Sends Token

Frontend sends token in header:

Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

Every request to protected endpoints includes this.

6️⃣ ASP.NET JWT Middleware

In Program.cs:

AddAuthentication()
AddJwtBearer()

This middleware:

reads token
↓
validates signature
↓
checks expiry
↓
checks issuer
↓
extracts claims
↓
creates HttpContext.User
7️⃣ Claims Become Available

After validation:

HttpContext.User

Contains claims.

Example usage in controller:

User.FindFirstValue(ClaimTypes.NameIdentifier)

This gives:

PatientId
8️⃣ Authorization

Example:

[Authorize(Roles = "Patient")]

This means:

token must exist
AND
role claim must be Patient

Otherwise:

401 Unauthorized
Example Protected Request
GET /api/Appointment/my

Flow:

Request
↓
JWT middleware validates token
↓
Claims extracted
↓
Controller reads PatientId
↓
Service fetches appointments
↓
Response returned
Interview Questions You Should Know
Q: Why hash passwords?

Passwords must never be stored as plain text. Hashing protects user credentials even if the database is compromised.

Q: What is inside a JWT token?

A JWT contains claims such as user ID, email, and role, along with issuer, audience, expiration time, and a digital signature.

Q: How does ASP.NET validate JWT?

The authentication middleware verifies the token signature, issuer, audience, and expiration using the configured secret key.

Q: How do you get the user ID from the token?

After authentication, claims are available through HttpContext.User. The user ID can be retrieved using User.FindFirstValue(ClaimTypes.NameIdentifier).

Q: Difference between Authentication and Authorization?

Authentication verifies the identity of a user, while authorization determines what the authenticated user is allowed to access.

What You Did Well in This Project

Your authentication system includes:

✔ Password hashing
✔ JWT token generation
✔ Claims-based identity
✔ Role-based authorization
✔ Secure middleware validation

This is exactly how modern APIs secure endpoints.

Interview Answer

If asked:

* “What is JwtSettings?”

You can say:

JwtSettings is a configuration class used to store JWT configuration values such as the secret key, issuer, audience, and expiration time. These values are loaded from appsettings.json and injected into services using dependency injection.

1️⃣ The Class (You Already Have It)

This class is just a configuration model.

It stores JWT configuration values.

Example values:

SecretKey
Issuer
Audience
ExpiryMinutes

Nothing else is required in the class.

2️⃣ appsettings.json (Required)

This is where the actual values come from.

Example:

"JwtSettings": {
  "SecretKey": "THIS_IS_MY_SUPER_SECRET_KEY_123456",
  "Issuer": "ExamProject2API",
  "Audience": "ExamProject2Client",
  "ExpiryMinutes": 60
}

ASP.NET loads this configuration.

3️⃣ Program.cs (Binding Configuration)

This connects appsettings.json to the class.

You already have this:

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

What happens here:

appsettings.json
        ↓
JwtSettings object created
        ↓
values copied into object
        ↓
object registered in Dependency Injection
4️⃣ AuthService Uses It

Then ASP.NET injects it here:

public AuthService(DataContext dataContext, JwtSettings jwtSettings)
{
    _dataContext = dataContext;
    _jwtSettings = jwtSettings;
}

Now your service can use:

_jwtSettings.SecretKey
_jwtSettings.Issuer
_jwtSettings.Audience
_jwtSettings.ExpiryMinutes
5️⃣ Used When Creating Token

Example:

var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
);

This is what signs the JWT token.

So The Full Connection Looks Like This
JwtSettings.cs
      ↓
appsettings.json
      ↓
Program.cs (Bind + DI)
      ↓
AuthService (Inject)
      ↓
JWT token creation
Is Your Implementation Good for Real Jobs?

Yes — this is a very standard pattern in ASP.NET Core.

Companies often store:

JWT
Database connection strings
Email configs
API keys

in configuration classes like this.

Small Optional Improvement (Not Required)

Some developers make properties nullable-safe:

public string SecretKey { get; set; } = null!;

But your version is completely fine for your project.

Interview Question You Might Get
“Where do JWT settings come from?”

Good answer:

JWT configuration such as secret key, issuer, audience, and expiration time is stored in appsettings.json. These values are bound to a JwtSettings configuration class and injected into services using dependency injection.