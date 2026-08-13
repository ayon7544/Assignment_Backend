using AssignmentSystem.API.Models;

namespace AssignmentSystem.API.Repositories.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id);
    Task<List<Assignment>> GetByTeacherAsync(Guid teacherId);
    Task<List<Assignment>> GetPublishedByClassAsync(Guid classId);
    Task<List<Assignment>> GetAllAsync();
    Task<Assignment> CreateAsync(Assignment assignment);
    Task<Assignment> UpdateAsync(Assignment assignment);
    Task DeleteAsync(Assignment assignment);
}