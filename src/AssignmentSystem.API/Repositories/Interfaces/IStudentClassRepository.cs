using AssignmentSystem.API.Models;
namespace AssignmentSystem.API.Repositories.Interfaces;
public interface IStudentClassRepository
{
    Task<StudentClass?> GetByIdAsync(Guid id);
    Task<List<StudentClass>> GetByStudentIdAsync(Guid studentId);
    Task<List<StudentClass>> GetByClassIdAsync(Guid classId);
    Task<StudentClass> CreateAsync(StudentClass sc);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid studentId, Guid classId);
}
