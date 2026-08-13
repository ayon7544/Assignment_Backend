using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AssignmentSystem.API.Repositories;
public class TeacherSubjectRepository : ITeacherSubjectRepository
{
    private readonly ApplicationDbContext _db;
    public TeacherSubjectRepository(ApplicationDbContext db) => _db = db;
    public async Task<TeacherSubject?> GetByIdAsync(Guid id) => await _db.TeacherSubjects.FindAsync(id);
    public async Task<List<TeacherSubject>> GetByTeacherIdAsync(Guid teacherId) => await _db.TeacherSubjects.Include(t=>t.Subject).Include(t=>t.Class).Where(t=>t.TeacherId==teacherId).ToListAsync();
    public async Task<List<TeacherSubject>> GetByClassIdAsync(Guid classId) => await _db.TeacherSubjects.Include(t=>t.Teacher).Include(t=>t.Subject).Where(t=>t.ClassId==classId).ToListAsync();
    public async Task<TeacherSubject> CreateAsync(TeacherSubject ts) { _db.TeacherSubjects.Add(ts); await _db.SaveChangesAsync(); return ts; }
    public async Task DeleteAsync(Guid id) { var t = await _db.TeacherSubjects.FindAsync(id); if(t!=null){_db.TeacherSubjects.Remove(t); await _db.SaveChangesAsync();} }
    public async Task<bool> ExistsAsync(Guid teacherId, Guid subjectId, Guid classId) => await _db.TeacherSubjects.AnyAsync(t=>t.TeacherId==teacherId && t.SubjectId==subjectId && t.ClassId==classId);
}
