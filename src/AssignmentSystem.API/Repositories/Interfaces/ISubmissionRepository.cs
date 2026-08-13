using AssignmentSystem.API.Models;

namespace AssignmentSystem.API.Repositories.Interfaces;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id);
    Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId);
    Task<List<Submission>> GetByAssignmentAsync(Guid assignmentId);
    Task<List<Submission>> GetByStudentAsync(Guid studentId);
    Task<List<Submission>> GetAllAsync();
    Task<Submission> CreateAsync(Submission submission);
    Task<Submission> UpdateAsync(Submission submission);
}