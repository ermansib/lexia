using Lexia.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Lexia.Studio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStudioApplication(this IServiceCollection services) =>
        services.AddApplicationHandlers(typeof(DependencyInjection).Assembly);
}
