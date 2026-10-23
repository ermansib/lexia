using Lexia.Books.Application.Abstractions;
using Lexia.Books.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lexia.Books.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBooksInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BooksDb")
            ?? throw new InvalidOperationException("Connection string 'BooksDb' is missing.");

        services.AddDbContext<BooksDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations", BooksDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BooksDbContext>());
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddHealthChecks().AddDbContextCheck<BooksDbContext>("books-db");
        return services;
    }
}
