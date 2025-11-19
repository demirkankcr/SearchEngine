using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SearchEngine.Application.Common.Behaviors;
using System.Reflection;

namespace SearchEngine.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Core.Application.Pipelines.DbLogging.DbLoggingBehavior<,>));
        
        services.AddTransient<Core.CrossCuttingConcerns.Logging.DbLog.Logging>();

        return services;
    }
}

