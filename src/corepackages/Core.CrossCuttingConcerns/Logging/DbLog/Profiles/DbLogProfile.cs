using AutoMapper;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.Domain.Entities;

namespace Core.CrossCuttingConcerns.Logging.DbLog.Profiles;

public class DbLogProfile : Profile
{
    public DbLogProfile()
    {
        CreateMap<Log, LogDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.LogDomain, opt => opt.MapFrom(src => src.LogDomain))
            .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.Host))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.Scheme, opt => opt.MapFrom(src => src.Scheme))
            .ForMember(dest => dest.QueryString, opt => opt.MapFrom(src => src.QueryString))
            .ForMember(dest => dest.RemoteIp, opt => opt.MapFrom(src => src.RemoteIp))
            .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Headers))
            .ForMember(dest => dest.RequestBody, opt => opt.MapFrom(src => src.RequestBody))
            .ForMember(dest => dest.RequestMethod, opt => opt.MapFrom(src => src.RequestMethod))
            .ForMember(dest => dest.UserAgent, opt => opt.MapFrom(src => src.UserAgent))
            .ForMember(dest => dest.ResponseHeaders, opt => opt.MapFrom(src => src.ResponseHeaders))
            .ForMember(dest => dest.ResponseBody, opt => opt.MapFrom(src => src.ResponseBody))
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
            .ForMember(dest => dest.ResponseTime, opt => opt.MapFrom(src => src.ResponseTime))
            .ForMember(dest => dest.Exception, opt => opt.MapFrom(src => src.Exception))
            .ForMember(dest => dest.ExceptionMessage, opt => opt.MapFrom(src => src.ExceptionMessage))
            .ForMember(dest => dest.InnerException, opt => opt.MapFrom(src => src.InnerException))
            .ForMember(dest => dest.InnerExceptionMessage, opt => opt.MapFrom(src => src.InnerExceptionMessage))
            .ForMember(dest => dest.LogDate, opt => opt.MapFrom(src => src.LogDate));

        CreateMap<LogDto, Log>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.LogDomain, opt => opt.MapFrom(src => src.LogDomain))
            .ForMember(dest => dest.Host, opt => opt.MapFrom(src => src.Host))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.Scheme, opt => opt.MapFrom(src => src.Scheme))
            .ForMember(dest => dest.QueryString, opt => opt.MapFrom(src => src.QueryString))
            .ForMember(dest => dest.RemoteIp, opt => opt.MapFrom(src => src.RemoteIp))
            .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Headers))
            .ForMember(dest => dest.RequestBody, opt => opt.MapFrom(src => src.RequestBody))
            .ForMember(dest => dest.RequestMethod, opt => opt.MapFrom(src => src.RequestMethod))
            .ForMember(dest => dest.UserAgent, opt => opt.MapFrom(src => src.UserAgent))
            .ForMember(dest => dest.ResponseHeaders, opt => opt.MapFrom(src => src.ResponseHeaders))
            .ForMember(dest => dest.ResponseBody, opt => opt.MapFrom(src => src.ResponseBody))
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
            .ForMember(dest => dest.ResponseTime, opt => opt.MapFrom(src => src.ResponseTime))
            .ForMember(dest => dest.Exception, opt => opt.MapFrom(src => src.Exception))
            .ForMember(dest => dest.ExceptionMessage, opt => opt.MapFrom(src => src.ExceptionMessage))
            .ForMember(dest => dest.InnerException, opt => opt.MapFrom(src => src.InnerException))
            .ForMember(dest => dest.InnerExceptionMessage, opt => opt.MapFrom(src => src.InnerExceptionMessage))
            .ForMember(dest => dest.LogDate, opt => opt.MapFrom(src => src.LogDate))
            .ForMember(dest => dest.GetLog, opt => opt.Ignore())
            .ForMember(dest => dest.GetErrorLog, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore());
    }
}

