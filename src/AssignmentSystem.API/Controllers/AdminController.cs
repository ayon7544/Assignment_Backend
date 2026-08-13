using AssignmentSystem.API.Data;
using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;
using AssignmentSystem.API.Middleware;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using AssignmentSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/admin")]
[Tags("Admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IClassRepository _classes;
    private readonly ISubjectRepository _subjects;
    private readonly ITeacherSubjectRepository _teacherSubjects;
    private readonly IStudentClassRepository _studentClasses;
    private readonly IAssignmentService _assignmentService;
    private readonly ISubmissionService _submissionService;
    private readonly ApplicationDbContext _db;

    public AdminController(IUserRepository users, IClassRepository classes, ISubjectRepository subjects, ITeacherSubjectRepository teacherSubjects, IStudentClassRepository studentClasses, IAssignmentService assignmentService, ISubmissionService submissionService, ApplicationDbContext db)
    { _users = users; _classes = classes; _subjects = subjects; _teacherSubjects = teacherSubjects; _studentClasses = studentClasses; _assignmentService = assignmentService; _submissionService = submissionService; _db = db; }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (users, total) = await _users.GetAllAsync(role, page, pageSize);
        var data = users.Select(u => new UserResponse(u.Id, u.FullName, u.Email, u.Role.ToString(), u.IsActive, u.CreatedAt)).ToList();
        var paged = new PagedResponse<UserResponse>(data, total, page, pageSize, (int)Math.Ceiling((double)total / pageSize));
        return Ok(ApiResponse<PagedResponse<UserResponse>>.Ok(paged));
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req)
    {
        var existing = await _users.GetByEmailAsync(req.Email);
        if (existing != null) throw new DuplicateException("A user with this email already exists");
        if (!Enum.TryParse<UserRole>(req.Role, true, out var role)) throw new BusinessRuleException("Invalid role");
        var user = new User { FullName = req.FullName, Email = req.Email.ToLower(), PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password), Role = role };
        var created = await _users.CreateAsync(user);
        var resp = new UserResponse(created.Id, created.FullName, created.Email, created.Role.ToString(), created.IsActive, created.CreatedAt);
        return StatusCode(201, ApiResponse<UserResponse>.Created(resp, "User created successfully"));
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest req)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
        user.FullName = req.FullName; user.Email = req.Email.ToLower();
        await _users.UpdateAsync(user);
        return Ok(ApiResponse<UserResponse>.Ok(new UserResponse(user.Id, user.FullName, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAt), "User updated"));
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
        user.IsActive = false; await _users.UpdateAsync(user);
        return Ok(ApiResponse.OkNoData("User deactivated"));
    }

    [HttpGet("classes")]
    public async Task<IActionResult> GetClasses()
    {
        var classes = await _classes.GetAllAsync();
        var data = classes.Select(c => new ClassResponse(c.Id, c.Name, c.Description, c.IsActive, c.Subjects?.Count ?? 0, c.StudentClasses?.Count ?? 0, c.CreatedAt)).ToList();
        return Ok(ApiResponse<List<ClassResponse>>.Ok(data));
    }

    [HttpPost("classes")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest req)
    {
        var cls = await _classes.CreateAsync(new Class { Name = req.Name, Description = req.Description });
        return StatusCode(201, ApiResponse<ClassResponse>.Created(new ClassResponse(cls.Id, cls.Name, cls.Description, cls.IsActive, 0, 0, cls.CreatedAt)));
    }

    [HttpPut("classes/{id}")]
    public async Task<IActionResult> UpdateClass(Guid id, [FromBody] CreateClassRequest req)
    {
        var cls = await _classes.GetByIdAsync(id) ?? throw new NotFoundException("Class not found");
        cls.Name = req.Name; cls.Description = req.Description;
        await _classes.UpdateAsync(cls);
        return Ok(ApiResponse.OkNoData("Class updated"));
    }

    [HttpDelete("classes/{id}")]
    public async Task<IActionResult> DeleteClass(Guid id)
    {
        var cls = await _classes.GetByIdAsync(id) ?? throw new NotFoundException("Class not found");
        cls.IsActive = false; await _classes.UpdateAsync(cls);
        return Ok(ApiResponse.OkNoData("Class deactivated"));
    }

    [HttpGet("subjects")]
    public async Task<IActionResult> GetSubjects([FromQuery] Guid? classId)
    {
        if (classId.HasValue)
        {
            var subjects = await _subjects.GetByClassIdAsync(classId.Value);
            var cls = await _classes.GetByIdAsync(classId.Value);
            var data = subjects.Select(s => new SubjectResponse(s.Id, s.Name, s.ClassId, cls?.Name ?? "", s.CreatedAt)).ToList();
            return Ok(ApiResponse<List<SubjectResponse>>.Ok(data));
        }
        var all = await _db.Subjects.Include(s => s.Class).ToListAsync();
        var data2 = all.Select(s => new SubjectResponse(s.Id, s.Name, s.ClassId, s.Class?.Name ?? "", s.CreatedAt)).ToList();
        return Ok(ApiResponse<List<SubjectResponse>>.Ok(data2));
    }

    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest req)
    {
        var cls = await _classes.GetByIdAsync(req.ClassId) ?? throw new NotFoundException("Class not found");
        var subject = await _subjects.CreateAsync(new Subject { Name = req.Name, ClassId = req.ClassId });
        return StatusCode(201, ApiResponse<SubjectResponse>.Created(new SubjectResponse(subject.Id, subject.Name, subject.ClassId, cls.Name, subject.CreatedAt)));
    }

    [HttpPut("subjects/{id}")]
    public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] CreateSubjectRequest req)
    {
        var sub = await _subjects.GetByIdAsync(id) ?? throw new NotFoundException("Subject not found");
        sub.Name = req.Name; await _subjects.UpdateAsync(sub);
        return Ok(ApiResponse.OkNoData("Subject updated"));
    }

    [HttpDelete("subjects/{id}")]
    public async Task<IActionResult> DeleteSubject(Guid id) { await _subjects.DeleteAsync(id); return Ok(ApiResponse.OkNoData("Subject deleted")); }

    [HttpPost("teacher-subjects")]
    public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherRequest req)
    {
        var teacher = await _users.GetByIdAsync(req.TeacherId) ?? throw new NotFoundException("Teacher not found");
        if (teacher.Role != UserRole.Teacher) throw new BusinessRuleException("User is not a teacher");
        var exists = await _teacherSubjects.ExistsAsync(req.TeacherId, req.SubjectId, req.ClassId);
        if (exists) throw new DuplicateException("Teacher already assigned");
        await _teacherSubjects.CreateAsync(new TeacherSubject { TeacherId = req.TeacherId, SubjectId = req.SubjectId, ClassId = req.ClassId });
        return StatusCode(201, ApiResponse.OkNoData("Teacher assigned"));
    }

    [HttpDelete("teacher-subjects/{id}")]
    public async Task<IActionResult> RemoveTeacherSubject(Guid id) { await _teacherSubjects.DeleteAsync(id); return Ok(ApiResponse.OkNoData("Teacher subject removed")); }

    [HttpPost("student-classes")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollStudentRequest req)
    {
        var student = await _users.GetByIdAsync(req.StudentId) ?? throw new NotFoundException("Student not found");
        if (student.Role != UserRole.Student) throw new BusinessRuleException("User is not a student");
        var exists = await _studentClasses.ExistsAsync(req.StudentId, req.ClassId);
        if (exists) throw new DuplicateException("Student already enrolled");
        await _studentClasses.CreateAsync(new StudentClass { StudentId = req.StudentId, ClassId = req.ClassId });
        return StatusCode(201, ApiResponse.OkNoData("Student enrolled"));
    }

    [HttpDelete("student-classes/{id}")]
    public async Task<IActionResult> RemoveStudentClass(Guid id) { await _studentClasses.DeleteAsync(id); return Ok(ApiResponse.OkNoData("Enrollment removed")); }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAllAssignments()
    {
        var list = await _assignmentService.GetAllAsync();
        return Ok(ApiResponse<List<AssignmentResponse>>.Ok(list));
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> GetAllSubmissions()
    {
        var list = await _submissionService.GetAllAsync();
        return Ok(ApiResponse<List<SubmissionResponse>>.Ok(list));
    }
}
