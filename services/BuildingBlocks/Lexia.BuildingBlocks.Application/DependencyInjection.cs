using System.Reflection;
using FluentValidation;
using Lexia.BuildingBlocks.Application.Behaviors;
using Lexia.BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Lexia.BuildingBlocks.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers every handler and validator in <paramref name="assembly"/>, then wraps handlers
    /// with validation and logging. The last decorator registered runs first.
    /// </summary>
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(c => c.AssignableToAny(
                    typeof(ICommandHandler<>), typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)),
                publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationDecorator<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator<,>));
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryValidationDecorator<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingDecorator<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator<,>));

        return services;
    }
}
