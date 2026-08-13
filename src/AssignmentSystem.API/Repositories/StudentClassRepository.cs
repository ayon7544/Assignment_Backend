using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AssignmentSystem.API.Repositories;
public class StudentClassRepository : IStudentClassRepository
{
    private readonly ApplicationDbContext _db;
    public StudentClassRepository(ApplicationDbContext db) => _db = db;
    public async Task<StudentClass?> GetByIdAsync(Guid id) => await _db.StudentClasses.FindAsync(id);
    public async Task<List<StudentClass>> GetByStudentIdAsync(Guid studentId) => await _db.StudentClasses.Include(s=>s.Class).Where(s=>s.StudentId==studentId).ToListAsync();
    public async Task<List<StudentClass>> GetByClassIdAsync(Guid classId) => await _db.StudentClasses.Include(s=>s.Student).Where(s=>s.ClassId==classId).ToListAsync();
    public async Task<StudentClass> CreateAsync(StudentClass sc) { _db.StudentClasses.Add(sc); await _db.SaveChangesAsync(); return sc; }
    public async Task DeleteAsync(Guid id) { var s = await _db.StudentClasses.FindAsync(id); if(s!=null){_db.StudentClasses.Remove(s); await _db.SaveChangesAsync();} }
    public async Task<bool> ExistsAsync(Guid studentId, Guid classId) => await _db.StudentClasses.AnyAsync(s=>s.StudentId==studentId && s.ClassId==classId);
}
