using Core.Entities;

namespace Core.Abstractions;

public interface IUserRepository : IRepository<Guid, User>
{
    Task<ICollection<User>> GetAllByDateRangeAsync(DateTime from, DateTime to);
    Task<User?> GetByUsernameAsync(string username);
}