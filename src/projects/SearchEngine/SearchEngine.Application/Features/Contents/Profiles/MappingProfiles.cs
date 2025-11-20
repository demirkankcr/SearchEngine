using AutoMapper;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using SearchEngine.Application.Features.Contents.Queries.GetSearchContents;

namespace SearchEngine.Application.Features.Contents.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Entity -> DTO Mapping
        CreateMap<Content, SearchContentDto>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src is VideoContent ? ((VideoContent)src).Duration : null))
            .ForMember(dest => dest.ReadingTime, opt => opt.MapFrom(src => src is TextContent ? ((TextContent)src).ReadingTime : 0));
    }
}
