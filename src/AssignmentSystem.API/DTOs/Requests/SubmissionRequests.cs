namespace AssignmentSystem.API.DTOs.Requests;

public record CreateSubmissionRequest(string? AnswerText, string? FileUrl);
public record UpdateSubmissionRequest(string? AnswerText, string? FileUrl);
