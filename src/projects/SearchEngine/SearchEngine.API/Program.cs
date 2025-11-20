using SearchEngine.Application;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.Persistence;
using SearchEngine.Infrastructure;
using Core.CrossCuttingConcerns.Jobs;
using SearchEngine.API.Extensions;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomRateLimiting();

var app = builder.Build();

// --- Startup Tasks ---
// Apply database migrations automatically
app.ApplyMigrations();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.CheckRedisConnection();
}

app.ConfigureCustomExceptionMiddleware();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthorization();

// Hangfire Dashboard & Jobs
app.ConfigureHangfireDashboard();
app.ConfigureHangfireJobs();

app.MapControllers();

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
    .ExcludeFromDescription();

app.Run();
