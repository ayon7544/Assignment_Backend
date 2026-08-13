namespace AssignmentSystem.API.DTOs.Requests;

public record CreateSubjectRequest(string Name, Guid ClassId);