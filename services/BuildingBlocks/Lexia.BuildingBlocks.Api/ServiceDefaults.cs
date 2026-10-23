using System.Globalization;
using Lexia.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Lexia.BuildingBlocks.Api;

/// <summary>Cross-cutting setup shared by every Lexia service: logging, auth, problem details, health checks, OpenAPI.</summary>
public static class ServiceDefaults
{
    private const string FrontendCorsPolicy = "frontend";

    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", context.HostingEnvironment.ApplicationName)
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture));

        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
        builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
        builder.Services.AddCors(o => o.AddPolicy(FrontendCorsPolicy, p => p
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition")));

        AddAuthentication(builder);
        builder.Services.AddAuthorization();
        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseSerilogRequestLogging();
        app.UseCors(FrontendCorsPolicy);
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapHealthChecks("/health");
        return app;
    }

    private static void AddAuthentication(WebApplicationBuilder builder)
    {
        var useDevelopmentUser = builder.Environment.IsDevelopment()
            && builder.Configuration.GetValue<bool>("Auth:UseDevelopmentUser");

        if (useDevelopmentUser)
        {
            builder.Services
                .AddAuthentication(DevelopmentAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                    DevelopmentAuthenticationHandler.SchemeName, _ => { });
            return;
        }

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Keycloak realm, for example http://localhost:8080/realms/lexia
                options.Authority = builder.Configuration["Auth:Authority"];
                options.Audience = builder.Configuration["Auth:Audience"];
                options.RequireHttpsMetadata = builder.Configuration.GetValue("Auth:RequireHttpsMetadata", defaultValue: true);
                options.MapInboundClaims = false;
            });
    }
}
