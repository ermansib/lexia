var builder = WebApplication.CreateBuilder(args);

// YARP (MIT) routes each /api/v1/<area> path to the service that owns it. Routes live in appsettings.json.
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHealthChecks();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("Content-Disposition")));

var app = builder.Build();

app.UseCors();
app.MapHealthChecks("/health");
app.MapReverseProxy();

await app.RunAsync();
