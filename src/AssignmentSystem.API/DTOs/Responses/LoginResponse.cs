namespace AssignmentSystem.API.DTOs.Responses;

public record LoginResponse(
    string Token,
    string RefreshToken,
    UserResponse User
);
