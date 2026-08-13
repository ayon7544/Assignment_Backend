namespace AssignmentSystem.API.DTOs.Responses;

public record SubjectResponse(
    Guid Id,
    string Name,
    Guid ClassId,
    string ClassName,
    DateTime CreatedAt
);