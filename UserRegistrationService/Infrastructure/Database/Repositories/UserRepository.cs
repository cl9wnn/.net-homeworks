using Core.Abstractions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class UserRepository(AppDbContext dbContext): IUserRepository
{
    public async Task<ICollection<User>> GetAllAsync()
    {
        return await dbContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ICollection<User>> GetAllByDateRangeAsync(DateTime from, DateTime to)
    {
        return await dbContext.Users
            .Where(u => u.CreatedDate >= from && u.CreatedDate <= to ||
                        u.UpdatedDate >= from && u.UpdatedDate <= to)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetAsync(Guid id)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> AddAsync(User entity)
    {
        if (await dbContext.Users.AnyAsync(u => u.Username == entity.Username))
        {
            return null;
        }
        
        await dbContext.Users.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        
        return entity;
    }

    public async Task<User?> UpdateAsync(User entity)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == entity.Id);

        if (user == null)
           return null; 
        
        user.Username = entity.Username;
        user.UpdatedDate = entity.UpdatedDate;
        
        await dbContext.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return false;
        
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
       return await dbContext.Users.AnyAsync(u => u.Id == id);
    }
}