using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;
using AssignmentSystem.API.Middleware;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using AssignmentSystem.API.Services.Interfaces;

namespace AssignmentSystem.API.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissions;
    private readonly IAssignmentRepository _assignments;
    private readonly IStudentClassRepository _studentClasses;

    public SubmissionService(
        ISubmissionRepository submissions,
        IAssignmentRepository assignments,
        IStudentClassRepository studentClasses)
    {
        _submissions = submissions;
        _assignments = assignments;
        _studentClasses = studentClasses;
    }

    public async Task<SubmissionResponse> SubmitAsync(Guid assignmentId, CreateSubmissionRequest req, Guid studentId)
    {
        var assignment = await _assignments.GetByIdAsync(assignmentId)
            ?? throw new NotFoundException("Assignment not found");

        if (assignment.Status != AssignmentStatus.Published)
            throw new NotFoundException("Assignment not found");

        var isEnrolled = await _studentClasses.ExistsAsync(studentId, assignment.ClassId);
        if (!isEnrolled)
            throw new ForbiddenException("You are not enrolled in the class for this assignment");

        var existing = await _submissions.GetByAssignmentAndStudentAsync(assignmentId, studentId);
        if (existing != null)
            throw new DuplicateException("You have already submitted for this assignment");

        var now = DateTime.UtcNow;
        var isLate = now > assignment.Deadline;

        if (isLate && !assignment.AllowLate)
            throw new DeadlineException("The deadline for this assignment has passed");

        var submission = new Submission
        {
            AssignmentId = assignmentId, StudentId = studentId,
            AnswerText = req.AnswerText, FileUrl = req.FileUrl,
            IsLate = isLate,
            Status = isLate ? SubmissionStatus.Late : SubmissionStatus.Submitted
        };

        var created = await _submissions.CreateAsync(submission);
        return MapToResponse(created, assignment.Title);
    }

    public async Task<SubmissionResponse> UpdateAsync(Guid id, UpdateSubmissionRequest req, Guid studentId)
    {
        var submission = await _submissions.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission not found");

        if (submission.StudentId != studentId)
            throw new ForbiddenException("You can only update your own submissions");

        if (submission.Assignment?.Deadline != null && DateTime.UtcNow > submission.Assignment.Deadline)
            throw new DeadlineException("Cannot update submission after the deadline");

        if (req.AnswerText != null) submission.AnswerText = req.AnswerText;
        if (req.FileUrl != null) submission.FileUrl = req.FileUrl;
        submission.UpdatedAt = DateTime.UtcNow;

        var updated = await _submissions.UpdateAsync(submission);
        return MapToResponse(updated, submission.Assignment?.Title ?? "");
    }

    public async Task<SubmissionDetailResponse> GradeAsync(Guid id, GradeSubmissionRequest req, Guid teacherId)
    {
        var submission = await _submissions.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission not found");

        if (submission.Assignment?.TeacherId != teacherId)
            throw new ForbiddenException("You can only grade submissions for your own assignments");

        if (req.Marks > submission.Assignment!.MaxMarks)
            throw new BusinessRuleException($@"Marks cannot exceed maximum marks ({submission.Assignment.MaxMarks})");

        if (req.Marks < 0)
            throw new BusinessRuleException("Marks cannot be negative");

        submission.Marks = req.Marks;
        submission.Feedback = req.Feedback;
        submission.Status = req.Status == "Rejected" ? SubmissionStatus.Rejected : SubmissionStatus.Graded;
        submission.UpdatedAt = DateTime.UtcNow;

        var updated = await _submissions.UpdateAsync(submission);
        return new SubmissionDetailResponse(
            updated.Id, updated.AssignmentId, submission.Assignment.Title,
            submission.Assignment.MaxMarks, updated.StudentId,
            updated.Student?.FullName ?? "", updated.AnswerText, updated.FileUrl,
            updated.SubmittedAt, updated.Marks, updated.Feedback,
            updated.Status.ToString(), updated.IsLate
        );
    }

    public async Task<List<SubmissionResponse>> GetByAssignmentAsync(Guid assignmentId, Guid teacherId)
    {
        var assignment = await _assignments.GetByIdAsync(assignmentId)
            ?? throw new NotFoundException("Assignment not found");

        if (assignment.TeacherId != teacherId)
            throw new ForbiddenException("You can only view submissions for your own assignments");

        var list = await _submissions.GetByAssignmentAsync(assignmentId);
        return list.Select(s => MapToResponse(s, assignment.Title)).ToList();
    }

    public async Task<List<SubmissionResponse>> GetByStudentAsync(Guid studentId)
    {
        var list = await _submissions.GetByStudentAsync(studentId);
        return list.Select(s => MapToResponse(s, s.Assignment?.Title ?? "")).ToList();
    }

    public async Task<List<SubmissionResponse>> GetAllAsync()
    {
        var list = await _submissions.GetAllAsync();
        return list.Select(s => MapToResponse(s, s.Assignment?.Title ?? "")).ToList();
    }

    private static SubmissionResponse MapToResponse(Submission s, string title) => new(
        s.Id, s.AssignmentId, title, s.StudentId,
        s.Student?.FullName ?? "", s.AnswerText, s.FileUrl,
        s.SubmittedAt, s.Marks, s.Feedback,
        s.Status.ToString(), s.IsLate
    );
}
