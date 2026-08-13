namespace AssignmentSystem.API.DTOs.Requests;

public record CreateAssignmentRequest(
    string Title,
    string Description,
    Guid ClassId,
    Guid SubjectId,
    int MaxMarks,
    DateTime Deadline,
    bool AllowLate = false
);

public record UpdateAssignmentRequest(
    string Title,
    string Description,
    int MaxMarks,
    DateTime Deadline,
    bool AllowLate
);

public record GradeSubmissionRequest(int Marks, string? Feedback, string Status);