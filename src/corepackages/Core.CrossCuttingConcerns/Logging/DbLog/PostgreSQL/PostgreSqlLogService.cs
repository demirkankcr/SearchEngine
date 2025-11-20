using AutoMapper;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.CrossCuttingConcerns.Logging.DbLog.PostgreSQL.Contexts;
using Core.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Core.CrossCuttingConcerns.Logging.DbLog.PostgreSQL;

public class PostgreSqlLogService : ILogService
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PostgreSqlLogService(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task CreateLog(LogDto logDto)
    {
        // Her log işleminde yeni context oluşturuyoruz (Scope yönetimi için)
        var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<LogDbContext>();
        using var context = new LogDbContext(optionsBuilder.Options, _configuration);

        var log = _mapper.Map<Log>(logDto);
        context.Logs.Add(log);
        await context.SaveChangesAsync();
    }
}

