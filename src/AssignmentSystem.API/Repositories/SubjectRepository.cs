using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AssignmentSystem.API.Repositories;
public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _db;
    public SubjectRepository(ApplicationDbContext db) => _db = db;
    public async Task<Subject?> GetByIdAsync(Guid id) => await _db.Subjects.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == id);
    public async Task<List<Subject>> GetByClassIdAsync(Guid classId) => await _db.Subjects.Where(s => s.ClassId == classId).OrderBy(s => s.Name).ToListAsync();
    public async Task<Subject> CreateAsync(Subject subject) { _db.Subjects.Add(subject); await _db.SaveChangesAsync(); return subject; }
    public async Task<Subject> UpdateAsync(Subject subject) { _db.Subjects.Update(subject); await _db.SaveChangesAsync(); return subject; }
    public async Task DeleteAsync(Guid id) { var s = await _db.Subjects.FindAsync(id); if (s != null) { _db.Subjects.Remove(s); await _db.SaveChangesAsync(); } }
}
