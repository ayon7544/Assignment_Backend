using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _db;
    public AssignmentRepository(ApplicationDbContext db) => _db = db;

    public async Task<Assignment?> GetByIdAsync(Guid id) =>
        await _db.Assignments
            .Include(a => a.Class).Include(a => a.Subject)
            .Include(a => a.Teacher).Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<Assignment>> GetByTeacherAsync(Guid teacherId) =>
        await _db.Assignments
            .Include(a => a.Class).Include(a => a.Subject).Include(a => a.Submissions)
            .Where(a => a.TeacherId == teacherId)
            .OrderByDescending(a => a.CreatedAt).ToListAsync();

    public async Task<List<Assignment>> GetPublishedByClassAsync(Guid classId) =>
        await _db.Assignments
            .Include(a => a.Class).Include(a => a.Subject).Include(a => a.Teacher)
            .Where(a => a.ClassId == classId && a.Status == AssignmentStatus.Published)
            .OrderByDescending(a => a.Deadline).ToListAsync();

    public async Task<List<Assignment>> GetAllAsync() =>
        await _db.Assignments
            .Include(a => a.Class).Include(a => a.Subject)
            .Include(a => a.Teacher).Include(a => a.Submissions)
            .OrderByDescending(a => a.CreatedAt).ToListAsync();

    public async Task<Assignment> CreateAsync(Assignment assignment)
    {
        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync();
        return assignment;
    }

    public async Task<Assignment> UpdateAsync(Assignment assignment)
    {
        _db.Assignments.Update(assignment);
        await _db.SaveChangesAsync();
        return assignment;
    }

    public async Task DeleteAsync(Assignment assignment)
    {
        _db.Assignments.Remove(assignment);
        await _db.SaveChangesAsync();
    }
}