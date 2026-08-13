namespace AssignmentSystem.API.DTOs.Requests;

public record AssignTeacherRequest(Guid TeacherId, Guid SubjectId, Guid ClassId);
public record EnrollStudentRequest(Guid StudentId, Guid ClassId);