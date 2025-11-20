using Microsoft.AspNetCore.RateLimiting;
using SearchEngine.Application;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.Persistence;
using SearchEngine.Infrastructure;
using Hangfire;
using SearchEngine.API.Jobs;
using Core.CrossCuttingConcerns.Jobs;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

// Hangfire Configuration 
builder.Services.AddHangfireServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    //10 saniye de maks 25 istek alabilir ve queue limiti 5 istek olabilir.
    options.AddFixedWindowLimiter("searchLimiter", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.PermitLimit = 25;
        limiterOptions.QueueLimit = 5;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    //burda veri scraiping işlemi ağır olabileceği için queue limiti 1 olarak tuttum backgroundjoblarının stacklenmesinin önüne geçer
    options.AddFixedWindowLimiter("syncLimiter", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 5;
        limiterOptions.QueueLimit = 1;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureCustomExceptionMiddleware();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard();

// Schedule Recurring Job
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<ContentSyncJob>(
    "sync-contents-job",
    job => job.ExecuteAsync(),
    Cron.Minutely);

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
    .ExcludeFromDescription();

app.Run();
