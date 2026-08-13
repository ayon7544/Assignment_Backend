using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AssignmentSystem.API.Repositories;
public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _db;
    public ClassRepository(ApplicationDbContext db) => _db = db;
    public async Task<Class?> GetByIdAsync(Guid id) => await _db.Classes.FindAsync(id);
    public async Task<List<Class>> GetAllAsync() => await _db.Classes.Include(c=>c.Subjects).Include(c=>c.StudentClasses).ToListAsync();
    public async Task<Class> CreateAsync(Class cls) { _db.Classes.Add(cls); await _db.SaveChangesAsync(); return cls; }
    public async Task<Class> UpdateAsync(Class cls) { _db.Classes.Update(cls); await _db.SaveChangesAsync(); return cls; }
}
