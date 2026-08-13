using AssignmentSystem.API.Models;
namespace AssignmentSystem.API.Repositories.Interfaces;
public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id);
    Task<List<Class>> GetAllAsync();
    Task<Class> CreateAsync(Class cls);
    Task<Class> UpdateAsync(Class cls);
}
