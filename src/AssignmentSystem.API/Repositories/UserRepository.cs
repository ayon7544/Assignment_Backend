using AssignmentSystem.API.Data;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;
    public UserRepository(ApplicationDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id) => await _db.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

public async Task<(List<User> Users, int Total)> GetAllAsync(string? role, int page, int pageSize)
{
    var query = _db.Users.AsQueryable();
    if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var roleEnum))
        query = query.Where(u => u.Role == roleEnum);
    var total = await query.CountAsync();
    var users = await query.OrderBy(u => u.FullName)
        .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    return (users, total);
}

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }
}