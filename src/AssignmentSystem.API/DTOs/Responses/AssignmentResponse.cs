namespace AssignmentSystem.API.DTOs.Responses;

public record AssignmentResponse(
    Guid Id,
    string Title,
    string Description,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName,
    Guid TeacherId,
    string TeacherName,
    int MaxMarks,
    DateTime Deadline,
    string Status,
    bool AllowLate,
    int SubmissionCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
