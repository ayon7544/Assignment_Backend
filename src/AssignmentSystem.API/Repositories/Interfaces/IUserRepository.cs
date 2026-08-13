using AssignmentSystem.API.Models;

namespace AssignmentSystem.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<(List<User> Users, int Total)> GetAllAsync(string? role, int page, int pageSize);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
}