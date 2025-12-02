using Core.Entities;

namespace Core.Abstractions;

public interface IUserRegistrationRepository
{
    Task Add(UserRegistration userRegistration);
    Task<ICollection<UserRegistration>> GetAllByTimeInterval(DateTime startTime, DateTime endTime);
}