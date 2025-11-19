using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Core.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void RegisterAllEntities<BaseType>(this ModelBuilder modelBuilder, Assembly assembly)
    {
        var baseTypeDefinition = typeof(BaseType);
        var baseGenericTypeDefinition = baseTypeDefinition.IsGenericType 
            ? baseTypeDefinition.GetGenericTypeDefinition() 
            : null;

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && 
                t.BaseType != null && 
                t.BaseType.IsGenericType && 
                (baseGenericTypeDefinition != null 
                    ? t.BaseType.GetGenericTypeDefinition() == baseGenericTypeDefinition
                    : typeof(BaseType).IsAssignableFrom(t)));

        foreach (var type in types)
        {
            modelBuilder.Entity(type);
        }
    }

    public static void RegisterAllConfigurations(this ModelBuilder modelBuilder, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

        foreach (var type in types)
        {
            var entityType = type.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .GetGenericArguments()[0];

            var configuration = Activator.CreateInstance(type);
            var applyMethod = typeof(ModelBuilder)
                .GetMethods()
                .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration) && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);

            applyMethod.Invoke(modelBuilder, new[] { configuration });
        }
    }
}

