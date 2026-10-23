using System.Text.Json.Serialization;
using Lexia.Books.Api.Endpoints;
using Lexia.Books.Application;
using Lexia.Books.Infrastructure;
using Lexia.Books.Infrastructure.Persistence;
using Lexia.BuildingBlocks.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddBooksApplication();
builder.Services.AddBooksInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<BooksDbContext>().Database.MigrateAsync();
}

app.UseServiceDefaults();
app.MapBookEndpoints();

await app.RunAsync();

/// <summary>Entry point, exposed so integration tests can host the service.</summary>
public partial class Program;
