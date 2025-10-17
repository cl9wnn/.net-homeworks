using Application.Dtos;
using Core.Entities;
using Mapster;

namespace Application.Mappings;

public class UserMappingConfig: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        
        config.NewConfig<UserDto, User>()
            .Map(dest => dest.Id, src => src.Id == Guid.Empty ? Guid.NewGuid() : src.Id)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate == default ? DateTime.UtcNow : src.CreatedDate)
            .Map(dest => dest.UpdatedDate, _ => DateTime.UtcNow);
    }
}