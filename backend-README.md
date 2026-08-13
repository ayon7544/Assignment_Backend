# Assignment & Submission Management System — Backend

A role-based school management API for teachers, students, and admins. Built with ASP.NET Core 8, PostgreSQL, and JWT authentication.

## Features

- **Admin**: Manage users, classes, subjects, teacher assignments, and student enrolments
- **Teacher**: Create and publish assignments (draft/published), view submissions, grade and give feedback
- **Student**: View published assignments for their class, submit answers, update submissions before deadline, view marks and feedback
- **JWT authentication** with role-based authorization enforced on every endpoint
- **FluentValidation** for input validation
- **Global exception middleware** with consistent error responses
- **Unit tests** covering auth, assignment, and submission business rules

## Technology Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 |
| Framework | ASP.NET Core Web API |
| Language | C# |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL |
| Auth | JWT Bearer tokens |
| Validation | FluentValidation |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit, Moq, FluentAssertions |
| Deployment | Docker, Render |

## Project Structure

```
Assignment_Backend/
├── src/
│   └── AssignmentSystem.API/
│       ├── Controllers/          # AdminController, TeacherController, StudentController, AuthController
│       ├── Data/                 # ApplicationDbContext, DatabaseSeeder
│       ├── DTOs/                 # Request and response records
│       ├── Middleware/           # GlobalExceptionMiddleware, custom exceptions
│       ├── Migrations/           # EF Core migration files
│       ├── Models/               # Domain entities
│       ├── Repositories/         # Data access layer with interfaces
│       ├── Services/             # Business logic layer with interfaces
│       ├── Validation/           # FluentValidation validators
│       ├── Program.cs
│       └── appsettings.json
├── tests/
│   └── AssignmentSystem.Tests/
│       ├── AssignmentServiceTests.cs
│       ├── AuthServiceTests.cs
│       ├── SubmissionServiceTests.cs
│       └── Helpers/TestDataBuilder.cs
├── Dockerfile
└── .env.example
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

## Local Setup

### 1. Clone the repository

```bash
git clone https://github.com/ayon7544/Assignment_Backend.git
cd Assignment_Backend
```

### 2. Configure environment variables

Copy the example environment file and fill in your values:

```bash
cp .env.example .env
```

Or set values directly in `src/AssignmentSystem.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=assignment_system;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Secret": "your-secret-key-must-be-at-least-32-characters"
  },
  "AllowedOrigins": ["http://localhost:3000"]
}
```

### 3. Set up the database

The application automatically runs migrations and seeds sample data on first startup in Development mode. Just ensure PostgreSQL is running and the connection string is correct.

```bash
# Optional: run migrations manually
cd src/AssignmentSystem.API
dotnet ef database update
```

### 4. Run the API

```bash
cd src/AssignmentSystem.API
dotnet run
```

The API will start at `http://localhost:5062`.  
Swagger UI is available at `http://localhost:5062/swagger`.

## Running the Tests

```bash
cd tests/AssignmentSystem.Tests
dotnet test
```

Tests cover:
- Auth: invalid credentials, inactive account
- Assignment: teacher authorization, deadline validation, publish/delete rules
- Submission: deadline enforcement, allow-late logic, duplicate prevention, grading rules

## API Endpoints

| Group | Method | Endpoint | Description |
|---|---|---|---|
| Auth | POST | `/api/auth/login` | Login and receive JWT |
| Admin | GET/POST/PUT/DELETE | `/api/admin/users` | Manage users |
| Admin | GET/POST/PUT/DELETE | `/api/admin/classes` | Manage classes |
| Admin | GET/POST/PUT/DELETE | `/api/admin/subjects` | Manage subjects |
| Admin | POST/DELETE | `/api/admin/teacher-subjects` | Assign teachers to subjects |
| Admin | POST/DELETE | `/api/admin/student-classes` | Enrol students in classes |
| Admin | GET | `/api/admin/assignments` | View all assignments |
| Admin | GET | `/api/admin/submissions` | View all submissions |
| Teacher | GET | `/api/teacher/subjects` | Get own subjects |
| Teacher | GET/POST/PUT/DELETE | `/api/teacher/assignments` | Manage own assignments |
| Teacher | PATCH | `/api/teacher/assignments/{id}/publish` | Publish an assignment |
| Teacher | GET | `/api/teacher/assignments/{id}/submissions` | View submissions for assignment |
| Teacher | PATCH | `/api/teacher/submissions/{id}/grade` | Grade a submission |
| Student | GET | `/api/student/assignments` | View published assignments |
| Student | GET | `/api/student/assignments/{id}` | View assignment details |
| Student | POST | `/api/student/assignments/{id}/submit` | Submit an answer |
| Student | PUT | `/api/student/submissions/{id}` | Update submission before deadline |
| Student | GET | `/api/student/submissions` | View own submissions with marks |

Full interactive documentation is available at the Swagger UI.

## Database Setup

Migrations are included in the repository under `src/AssignmentSystem.API/Migrations/`. EF Core applies them automatically in Development. For a manual setup:

```bash
# Apply migrations
dotnet ef database update --project src/AssignmentSystem.API

# Or generate a SQL script from migrations
dotnet ef migrations script --project src/AssignmentSystem.API --output db-schema.sql
```

Sample data is seeded automatically by `DatabaseSeeder.cs` when the database is empty.

## Demo Credentials

| Role | Email | Password |
|---|---|---|
| Admin | admin@school.edu | Admin@123 |
| Teacher | teacher1@school.edu | Teacher@123 |
| Teacher 2 | teacher2@school.edu | Teacher@123 |
| Student | student1@school.edu | Student@123 |
| Student 2 | student2@school.edu | Student@123 |

## Live URLs

- **Frontend**: https://assignment-frontend-three-tau.vercel.app
- **Backend API**: https://assignment-backend-b924.onrender.com
- **Swagger UI**: https://assignment-backend-b924.onrender.com/swagger

## Docker

```bash
docker build -t assignment-backend .
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;..." \
  -e Jwt__Secret="your-secret" \
  -e Jwt__Issuer="assignment-system" \
  -e Jwt__Audience="assignment-system-users" \
  -e AllowedOrigins__0="http://localhost:3000" \
  assignment-backend
```

## Assumptions

- A teacher can only create assignments for subjects they are assigned to by an admin.
- Students can only see published assignments for their enrolled class.
- Submissions can be updated before the deadline unless the teacher has disabled late submissions.
- Deleting an assignment is only allowed while it is in Draft status.
- The `AllowLate` flag on an assignment controls whether submissions are accepted after the deadline; late submissions are marked with `IsLate = true` and status `Late`.
- Admin cannot log in as Teacher or Student; each role has distinct permissions enforced at the API level.

## Known Limitations

- No email notifications (would require an SMTP or email service integration).
- No file upload for submissions; file URLs can be provided as text.
- Refresh token is returned in the login response but token refresh endpoint is not implemented; users must log in again after expiry.
- Swagger is enabled in Development only by default. See Render environment variables to enable it in production.
