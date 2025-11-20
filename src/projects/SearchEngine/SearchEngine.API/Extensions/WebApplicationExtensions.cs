using Hangfire;
using SearchEngine.API.Filters;
using SearchEngine.API.Jobs;
using StackExchange.Redis;

namespace SearchEngine.API.Extensions;

public static class WebApplicationExtensions
{
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
                    Console.WriteLine($"✅ [Redis] Connection Successful: {redis.Configuration}");
                    Console.WriteLine($"   - Client Name: {redis.ClientName}");
                    Console.WriteLine($"   - Status: {redis.GetStatus()}");
                }
                else
                {
                    Console.WriteLine($"❌ [Redis] Connection Failed: Multiplexer is registered but not connected.");
                }
            }
            else
            {
                // Redis kapalıysa MemoryCache kullanılıyor demektir.
                Console.WriteLine("ℹ️ [Cache] Redis is disabled or not configured. Using MemoryCache.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [Redis] Error checking connection: {ex.Message}");
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
