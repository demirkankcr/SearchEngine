using SearchEngine.Application;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.Persistence;
using SearchEngine.Infrastructure;
using Hangfire;
using SearchEngine.API.Jobs;
using Core.CrossCuttingConcerns.Jobs;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureCustomExceptionMiddleware();

app.UseHttpsRedirection();

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
