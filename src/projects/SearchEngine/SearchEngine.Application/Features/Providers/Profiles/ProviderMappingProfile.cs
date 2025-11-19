using AutoMapper;
using Core.Domain.Entities;
using SearchEngine.Application.Features.Providers.Queries.GetProviders;

namespace SearchEngine.Application.Features.Providers.Profiles;

public class ProviderMappingProfile : Profile
{
    public ProviderMappingProfile()
    {
        CreateMap<Content, GetProvidersResponse>()
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType.ToString()))
            .Include<VideoContent, GetProvidersResponse>()
            .Include<TextContent, GetProvidersResponse>();

        CreateMap<VideoContent, GetProvidersResponse>()
            .ForMember(dest => dest.Views, opt => opt.MapFrom(src => src.Views))
            .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));

        CreateMap<TextContent, GetProvidersResponse>()
            .ForMember(dest => dest.ReadingTime, opt => opt.MapFrom(src => src.ReadingTime))
            .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src.Reactions))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
    }
}