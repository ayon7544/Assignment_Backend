using AssignmentSystem.API.Models;
namespace AssignmentSystem.API.Repositories.Interfaces;
public interface ITeacherSubjectRepository
{
    Task<TeacherSubject?> GetByIdAsync(Guid id);
    Task<List<TeacherSubject>> GetByTeacherIdAsync(Guid teacherId);
    Task<List<TeacherSubject>> GetByClassIdAsync(Guid classId);
    Task<TeacherSubject> CreateAsync(TeacherSubject ts);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid teacherId, Guid subjectId, Guid classId);
}
