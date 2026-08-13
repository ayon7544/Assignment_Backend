using AssignmentSystem.API.Models;
namespace AssignmentSystem.API.Repositories.Interfaces;
public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(Guid id);
    Task<List<Subject>> GetByClassIdAsync(Guid classId);
    Task<Subject> CreateAsync(Subject subject);
    Task<Subject> UpdateAsync(Subject subject);
    Task DeleteAsync(Guid id);
}
