using Core.Abstractions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class UserRegistrationRepository(AppDbContext dbContext): IUserRegistrationRepository
{
    public async Task Add(UserRegistration userRegistration)
    {
        if (userRegistration == null)
        {
            throw new ArgumentNullException(nameof(userRegistration));
        }
        
        await dbContext.UserRegistrations.AddAsync(userRegistration);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ICollection<UserRegistration>> GetAllByTimeInterval(DateTime startTime, DateTime endTime)
    {
        return await dbContext.UserRegistrations
            .Where(u => u.RegisteredAt >= startTime && u.RegisteredAt <= endTime)
            .OrderBy(u => u.RegisteredAt)
            .AsNoTracking()
            .ToListAsync();
    }
}