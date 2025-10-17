using Application.Abstractions;
using Application.Dtos;
using Core.Abstractions;
using Core.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService(IUserRepository userRepository, IMapper mapper): IUserService
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return mapper.Map<List<UserDto>>(users);
    }

    public async Task<List<UserDto>> GetAllByDateRangeAsync(DateTime from, DateTime to)
    {
        var users = await userRepository.GetAllByDateRangeAsync(from, to);
        return mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto?> GetAsync(Guid id)
    {
        var user = await userRepository.GetAsync(id);
        return mapper.Map<UserDto?>(user);
    }

    public async Task<UserDto?> AddAsync(UserDto dto)
    {
        var existedUser = await userRepository.GetByUsernameAsync(dto.Username);

        if (existedUser != null)
            return null;
        
        var hashedPassword = new PasswordHasher<UserDto>().HashPassword(null, dto.Password);
        
        var user = mapper.Map<User>(dto);
        user.Password = hashedPassword;
        
        var createdUser = await userRepository.AddAsync(user);
        return mapper.Map<UserDto?>(createdUser);
    }

    public async Task<UserDto?> UpdateAsync(UserDto dto)
    {
        var existedUser = await userRepository.GetByUsernameAsync(dto.Username);

        if (existedUser != null)
            return null;

        var user = mapper.Map<User>(dto);

        var updatedUser = await userRepository.UpdateAsync(user);
        return mapper.Map<UserDto?>(updatedUser);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await userRepository.DeleteAsync(id);
    }
}
