using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SearchEngine.Application.Common.Behaviors;
using SearchEngine.Application.Services.ContentProviders;
using System.Reflection;

namespace SearchEngine.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationServiceRegistration), typeof(Core.CrossCuttingConcerns.Logging.DbLog.Profiles.DbLogProfile));
        services.AddMediatR(Assembly.GetExecutingAssembly());
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Core.Application.Pipelines.DbLogging.DbLoggingBehavior<,>));

        services.AddTransient<Core.CrossCuttingConcerns.Logging.DbLog.Logging>();
        
        // memory leak riski olmasın request bittiğinde direk dispose edilsin diye scoped kullandım. ayrıca her bir request için için ayrı instance oluşturacak bundan dolayı da başka requestler geldiğinde karmaşıklıktan kurtulabilir ??
        services.AddScoped<IContentProviderFactory, ContentProviderFactory>();

        return services;
    }
}