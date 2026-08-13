using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;

namespace AssignmentSystem.API.Services.Interfaces;

public interface ISubmissionService
{
    Task<SubmissionResponse> SubmitAsync(Guid assignmentId, CreateSubmissionRequest req, Guid studentId);
    Task<SubmissionResponse> UpdateAsync(Guid id, UpdateSubmissionRequest req, Guid studentId);
    Task<SubmissionDetailResponse> GradeAsync(Guid id, GradeSubmissionRequest req, Guid teacherId);
    Task<List<SubmissionResponse>> GetByAssignmentAsync(Guid assignmentId, Guid teacherId);
    Task<List<SubmissionResponse>> GetByStudentAsync(Guid studentId);
    Task<List<SubmissionResponse>> GetAllAsync();
}