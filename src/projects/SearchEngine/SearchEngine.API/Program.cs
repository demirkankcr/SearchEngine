using SearchEngine.Application;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.Persistence;
using SearchEngine.Infrastructure;
using Core.CrossCuttingConcerns.Jobs;
using SearchEngine.API.Extensions;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// --- Service Registrations ---

// Core Layers
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);

// API Essentials
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom Extensions (Clean Code)
builder.Services.AddCustomRateLimiting();

var app = builder.Build();

// --- Startup Tasks ---
// Apply database migrations automatically
app.ApplyMigrations();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Startup Checks
    app.CheckRedisConnection();
}

// Global Exception Handling
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
