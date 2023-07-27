namespace Domain.Configurations;

using DTOs;
using Entities;
using Mapster;

public static class MappingConfig
{
    public static void RegisterDomainMappings()
    {
        TypeAdapterConfig<ItemDto, Story>
            .NewConfig()
            .Map(dest => dest.PostedBy, src => src.By)
            .Map(dest => dest.CommentCount, src => src.Descendants)
            .Map(dest => dest.Uri, src => src.Url)
            .Map(dest=>dest.Time, src => DateTimeOffset.FromUnixTimeSeconds(src.Time));
    }
}