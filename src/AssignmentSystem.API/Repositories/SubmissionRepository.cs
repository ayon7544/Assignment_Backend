using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _db;
    public SubmissionRepository(ApplicationDbContext db) => _db = db;

    public async Task<Submission?> GetByIdAsync(Guid id) =>
        await _db.Submissions
            .Include(s => s.Assignment).ThenInclude(a => a.Class)
            .Include(s => s.Assignment).ThenInclude(a => a.Subject)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId) =>
        await _db.Submissions
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

    public async Task<List<Submission>> GetByAssignmentAsync(Guid assignmentId) =>
        await _db.Submissions.Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAt).ToListAsync();

    public async Task<List<Submission>> GetByStudentAsync(Guid studentId) =>
        await _db.Submissions
            .Include(s => s.Assignment).ThenInclude(a => a.Class)
            .Include(s => s.Assignment).ThenInclude(a => a.Subject)
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.SubmittedAt).ToListAsync();

    public async Task<List<Submission>> GetAllAsync() =>
        await _db.Submissions
            .Include(s => s.Assignment).Include(s => s.Student)
            .OrderByDescending(s => s.SubmittedAt).ToListAsync();

    public async Task<Submission> CreateAsync(Submission submission)
    {
        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync();
        return submission;
    }

    public async Task<Submission> UpdateAsync(Submission submission)
    {
        _db.Submissions.Update(submission);
        await _db.SaveChangesAsync();
        return submission;
    }
}