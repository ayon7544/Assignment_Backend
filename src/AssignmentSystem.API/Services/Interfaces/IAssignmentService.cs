using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;

namespace AssignmentSystem.API.Services.Interfaces;

public interface IAssignmentService
{
    Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request, Guid teacherId);
    Task<AssignmentResponse> UpdateAsync(Guid id, UpdateAssignmentRequest request, Guid teacherId);
    Task DeleteAsync(Guid id, Guid teacherId);
    Task<AssignmentResponse> PublishAsync(Guid id, Guid teacherId);
    Task<List<AssignmentResponse>> GetByTeacherAsync(Guid teacherId);
    Task<List<AssignmentResponse>> GetPublishedForStudentAsync(Guid studentId);
    Task<AssignmentResponse> GetByIdForStudentAsync(Guid id, Guid studentId);
    Task<List<AssignmentResponse>> GetAllAsync();
}