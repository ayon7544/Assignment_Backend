namespace AssignmentSystem.API.DTOs.Requests;

public record CreateClassRequest(string Name, string? Description);
public record UpdateClassRequest(string Name, string? Description);