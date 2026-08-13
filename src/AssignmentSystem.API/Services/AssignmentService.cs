using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;
using AssignmentSystem.API.Middleware;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using AssignmentSystem.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignments;
    private readonly ITeacherSubjectRepository _teacherSubjects;
    private readonly IStudentClassRepository _studentClasses;

    public AssignmentService(
        IAssignmentRepository assignments,
        ITeacherSubjectRepository teacherSubjects,
        IStudentClassRepository studentClasses)
    {
        _assignments = assignments;
        _teacherSubjects = teacherSubjects;
        _studentClasses = studentClasses;
    }

    public async Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest req, Guid teacherId)
    {
        var isAssigned = await _teacherSubjects.ExistsAsync(teacherId, req.SubjectId, req.ClassId);
        if (!isAssigned)
            throw new ForbiddenException("You are not assigned to this subject for this class");

        if (req.Deadline <= DateTime.UtcNow)
            throw new BusinessRuleException("Deadline must be in the future");

        var assignment = new Assignment
        {
            Title = req.Title, Description = req.Description,
            ClassId = req.ClassId, SubjectId = req.SubjectId,
            TeacherId = teacherId, MaxMarks = req.MaxMarks,
            Deadline = req.Deadline, AllowLate = req.AllowLate,
            Status = AssignmentStatus.Draft
        };

        var created = await _assignments.CreateAsync(assignment);
        var full = await _assignments.GetByIdAsync(created.Id);
        return MapToResponse(full!);
    }

    public async Task<AssignmentResponse> UpdateAsync(Guid id, UpdateAssignmentRequest req, Guid teacherId)
    {
        var assignment = await _assignments.GetByIdAsync(id)
            ?? throw new NotFoundException("Assignment not found");

        if (assignment.TeacherId != teacherId)
            throw new ForbiddenException("You can only update your own assignments");

        if (req.Deadline <= DateTime.UtcNow)
            throw new BusinessRuleException("Deadline must be in the future");

        assignment.Title = req.Title;
        assignment.Description = req.Description;
        assignment.MaxMarks = req.MaxMarks;
        assignment.Deadline = req.Deadline;
        assignment.AllowLate = req.AllowLate;

        await _assignments.UpdateAsync(assignment);
        return MapToResponse(assignment);
    }

    public async Task DeleteAsync(Guid id, Guid teacherId)
    {
        var assignment = await _assignments.GetByIdAsync(id)
            ?? throw new NotFoundException("Assignment not found");

        if (assignment.TeacherId != teacherId)
            throw new ForbiddenException("You can only delete your own assignments");

        if (assignment.Status == AssignmentStatus.Published)
            throw new BusinessRuleException("Published assignments cannot be deleted");

        await _assignments.DeleteAsync(assignment);
    }

    public async Task<AssignmentResponse> PublishAsync(Guid id, Guid teacherId)
    {
        var assignment = await _assignments.GetByIdAsync(id)
            ?? throw new NotFoundException("Assignment not found");

        if (assignment.TeacherId != teacherId)
            throw new ForbiddenException("You can only publish your own assignments");

        if (assignment.Status == AssignmentStatus.Published)
            throw new BusinessRuleException("Assignment is already published");

        assignment.Status = AssignmentStatus.Published;
        await _assignments.UpdateAsync(assignment);
        return MapToResponse(assignment);
    }

    public async Task<List<AssignmentResponse>> GetByTeacherAsync(Guid teacherId)
    {
        var list = await _assignments.GetByTeacherAsync(teacherId);
        return list.Select(MapToResponse).ToList();
    }

    public async Task<List<AssignmentResponse>> GetPublishedForStudentAsync(Guid studentId)
    {
        var enrollments = await _studentClasses.GetByStudentIdAsync(studentId);
        if (!enrollments.Any()) return new List<AssignmentResponse>();

        var result = new List<AssignmentResponse>();
        foreach (var enrollment in enrollments)
        {
            var list = await _assignments.GetPublishedByClassAsync(enrollment.ClassId);
            result.AddRange(list.Select(MapToResponse));
        }
        return result;
    }

    public async Task<AssignmentResponse> GetByIdForStudentAsync(Guid id, Guid studentId)
    {
        var assignment = await _assignments.GetByIdAsync(id)
            ?? throw new NotFoundException("Assignment not found");

        var inClass = await _studentClasses.ExistsAsync(studentId, assignment.ClassId);
        if (!inClass || assignment.Status != AssignmentStatus.Published)
            throw new ForbiddenException("You do not have access to this assignment");

        return MapToResponse(assignment);
    }

    public async Task<List<AssignmentResponse>> GetAllAsync()
    {
        var list = await _assignments.GetAllAsync();
        return list.Select(MapToResponse).ToList();
    }

    private static AssignmentResponse MapToResponse(Assignment a) => new(
        a.Id, a.Title, a.Description,
        a.ClassId, a.Class?.Name ?? "",
        a.SubjectId, a.Subject?.Name ?? "",
        a.TeacherId, a.Teacher?.FullName ?? "",
        a.MaxMarks, a.Deadline,
        a.Status.ToString(), a.AllowLate,
        a.Submissions?.Count ?? 0,
        a.CreatedAt, a.UpdatedAt
    );
}
