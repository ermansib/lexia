using Lexia.Studio.Application.Abstractions;
using Lexia.Studio.Infrastructure.Pdf;
using Microsoft.Extensions.DependencyInjection;

namespace Lexia.Studio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStudioInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IInteriorRenderer, PdfInteriorRenderer>();
        return services;
    }
}
