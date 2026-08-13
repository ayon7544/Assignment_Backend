namespace AssignmentSystem.API.DTOs.Requests;

public record CreateUserRequest(string FullName, string Email, string Password, string Role);
public record UpdateUserRequest(string FullName, string Email);