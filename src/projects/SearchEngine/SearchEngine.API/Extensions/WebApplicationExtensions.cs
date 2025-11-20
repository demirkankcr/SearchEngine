using Hangfire;
using SearchEngine.API.Filters;
using SearchEngine.API.Jobs;
using StackExchange.Redis;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SearchEngine.API.Extensions;

public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"[Database] Applying {pendingMigrations.Count} pending migration(s)...");
                dbContext.Database.Migrate();
                Console.WriteLine("[Database] Migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("[Database] Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Database] Error applying migrations: {ex.Message}");
            // Development ortamında hata fırlat, production'da logla ama devam et
            if (app.Environment.IsDevelopment())
            {
                throw;
            }
        }
    }

    public static void CheckRedisConnection(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        try
        {
            using var scope = app.Services.CreateScope();
            var redis = scope.ServiceProvider.GetService<IConnectionMultiplexer>();

            if (redis != null)
            {
                if (redis.IsConnected)
                {
                    Console.WriteLine($"[Redis] Connection Successful: {redis.Configuration}");
                    Console.WriteLine($"   - Client Name: {redis.ClientName}");
                    Console.WriteLine($"   - Status: {redis.GetStatus()}");
                }
                else
                {
                    Console.WriteLine($"[Redis] Connection Failed: Multiplexer is registered but not connected.");
                }
            }
            else
            {
                // Redis kapalıysa MemoryCache kullanılıyor demektir.
                Console.WriteLine("[Cache] Redis is disabled or not configured. Using MemoryCache.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Redis] Error checking connection: {ex.Message}");
        }
    }

    public static void ConfigureHangfireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        });
    }

    public static void ConfigureHangfireJobs(this WebApplication app)
    {
        // Schedule Recurring Job
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<ContentSyncJob>(
            "sync-contents-job",
            job => job.ExecuteAsync(),
            Cron.Minutely);
    }
}
