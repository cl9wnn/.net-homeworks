using Application.Abstractions;
using Application.Dtos;
using Application.Mappers;
using Core.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class UserService(IUserRepository userRepository): IUserService
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.ToUserDtoList();
    }

    public async Task<List<UserDto>> GetAllByDateRangeAsync(DateTime from, DateTime to)
    {
        var users = await userRepository.GetAllByDateRangeAsync(from, to);
        return users.ToUserDtoList();
    }

    public async Task<UserDto?> GetAsync(Guid id)
    {
        var user = await userRepository.GetAsync(id);
        return user?.ToUserDto();
    }

    public async Task<UserDto?> AddAsync(UserDto dto)
    {
        var existedUser = await userRepository.GetByUsernameAsync(dto.Username);

        if (existedUser != null)
            return null;
        
        var hashedPassword = new PasswordHasher<UserDto>().HashPassword(null, dto.Password);
        var user = dto.ToUser(hashedPassword);
        
        var createdUser = await userRepository.AddAsync(user);
        return createdUser?.ToUserDto();
    }

    public async Task<UserDto?> UpdateAsync(UserDto dto)
    {
        var existedUser = await userRepository.GetByUsernameAsync(dto.Username);

        if (existedUser != null)
            return null;

        var user = dto.ToUser();

        var updatedUser = await userRepository.UpdateAsync(user);
        return updatedUser?.ToUserDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await userRepository.DeleteAsync(id);
    }
}
