using Application.Dtos;

namespace Application.Abstractions;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<List<UserDto>> GetAllByDateRangeAsync(DateTime from, DateTime to);
    Task<UserDto?> GetAsync(Guid id);
    Task<UserDto?> AddAsync(UserDto dto);
    Task<UserDto?> UpdateAsync(UserDto dto);
    Task<bool> DeleteAsync(Guid id);
}