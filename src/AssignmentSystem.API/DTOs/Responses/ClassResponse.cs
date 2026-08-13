namespace AssignmentSystem.API.DTOs.Responses;

public record ClassResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    int SubjectCount,
    int StudentCount,
    DateTime CreatedAt
);