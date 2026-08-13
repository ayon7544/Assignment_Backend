namespace AssignmentSystem.API.DTOs.Responses;

public record UserResponse(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);