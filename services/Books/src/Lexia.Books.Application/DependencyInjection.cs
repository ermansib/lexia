using Lexia.Books.Application.Books;
using Lexia.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Lexia.Books.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBooksApplication(this IServiceCollection services)
    {
        services.AddScoped<BookEditor>();
        return services.AddApplicationHandlers(typeof(DependencyInjection).Assembly);
    }
}
