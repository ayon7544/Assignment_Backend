namespace AssignmentSystem.API.DTOs.Responses;

public record SubmissionResponse(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    Guid StudentId,
    string StudentName,
    string? AnswerText,
    string? FileUrl,
    DateTime SubmittedAt,
    int? Marks,
    string? Feedback,
    string Status,
    bool IsLate
);

public record SubmissionDetailResponse(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    int MaxMarks,
    Guid StudentId,
    string StudentName,
    string? AnswerText,
    string? FileUrl,
    DateTime SubmittedAt,
    int? Marks,
    string? Feedback,
    string Status,
    bool IsLate
);
