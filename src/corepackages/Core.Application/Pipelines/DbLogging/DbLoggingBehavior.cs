using AutoMapper;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.CrossCuttingConcerns.Logging.DbLog;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.CrossCuttingConcerns.Logging.DbLog.PostgreSQL;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace Core.Application.Pipelines.DbLogging;

public class DbLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly Core.CrossCuttingConcerns.Logging.DbLog.Logging _logging;
    private readonly IMapper _mapper;

    public DbLoggingBehavior(IHttpContextAccessor httpContextAccessor, Core.CrossCuttingConcerns.Logging.DbLog.Logging logging, IConfiguration configuration, IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _logging = logging;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Request
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return await next();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            NullValueHandling = NullValueHandling.Ignore,
        };
        settings.ContractResolver = new DefaultContractResolver
        {
            IgnoreSerializableAttribute = true
        };
        settings.Error = (sender, args) =>
        {
            args.ErrorContext.Handled = true;
        };

        string requestBody = await ReadRequestBody(httpContext.Request);
        // Opsiyonel request body loglama
        // requestBody += JsonConvert.SerializeObject(request, settings);

        Log logEntry = CreateLogEntry(httpContext, requestBody);

        // Response
        TResponse response;
        try
        {
            response = await next();
            stopwatch.Stop();

            logEntry.ResponseBody = JsonConvert.SerializeObject(response, settings);
            logEntry.StatusCode = (response as ObjectResult)?.StatusCode ?? httpContext.Response.StatusCode;
            logEntry.LogDate = DateTime.UtcNow;
            logEntry.ResponseTime = stopwatch.ElapsedMilliseconds;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logEntry.Exception = ex.ToString();
            logEntry.ExceptionMessage = ex.Message;
            logEntry.InnerException = ex.InnerException?.ToString();
            logEntry.InnerExceptionMessage = ex.InnerException?.Message;
            logEntry.StatusCode = 500;
            logEntry.LogDate = DateTime.UtcNow;
            logEntry.ResponseTime = stopwatch.ElapsedMilliseconds;

            await AddLogToDatabase(logEntry);
            throw;
        }

        // Başarılı işlem logu
        await AddLogToDatabase(logEntry);
        return response;
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        using (var reader = new StreamReader(request.Body, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
    }

    private Log CreateLogEntry(HttpContext context, string requestBody)
    {
        string headers = string.Join(";", context.Request.Headers.Select(h => $"{h.Key}:{h.Value}"));
        string responseHeaders = string.Join(";", context.Response.Headers.Select(h => $"{h.Key}:{h.Value}"));

        var logEntry = new Log
        {
            LogDate = DateTime.UtcNow,
            EventId = Guid.NewGuid().ToString(),
            UserId = context.User?.Identity?.Name,
            LogDomain = context.Request.Host.Value,
            Host = context.Request.Host.Value,
            Path = context.Request.Path,
            Scheme = context.Request.Scheme,
            QueryString = context.Request.QueryString.ToString(),
            RemoteIp = context.Connection.RemoteIpAddress?.ToString(),
            Headers = headers,
            RequestBody = requestBody,
            RequestMethod = context.Request.Method,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            ResponseHeaders = responseHeaders,
        };

        return logEntry;
    }

    private async Task AddLogToDatabase(Log logEntry)
    {
        LogDto data = _mapper.Map<LogDto>(logEntry);
        // PostgreSqlLogService her çağrıda yeni context oluşturduğu için thread-safe
        await _logging.CreateLog(new PostgreSqlLogService(_configuration, _mapper), data);
    }
}

