using Application.Dtos;
using Core.Entities;

namespace Application.Mappers;

public static class UserMappingExtensions
{
    public static User ToUser(this UserDto dto, string? hashPassword = null)
    {
        return new User
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            Password = hashPassword ?? dto.Password,
            CreatedDate = dto.CreatedDate == default ? DateTime.UtcNow : dto.CreatedDate,
            UpdatedDate = DateTime.UtcNow
        };
    }

    public static UserDto ToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate
        };
    }

    public static List<UserDto> ToUserDtoList(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToUserDto()).ToList();
    }
}