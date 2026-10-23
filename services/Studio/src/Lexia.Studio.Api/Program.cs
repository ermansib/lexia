using System.Text.Json.Serialization;
using Lexia.BuildingBlocks.Api;
using Lexia.Studio.Api.Endpoints;
using Lexia.Studio.Application;
using Lexia.Studio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddStudioApplication();
builder.Services.AddStudioInfrastructure();

var app = builder.Build();

app.UseServiceDefaults();
app.MapStudioEndpoints();

app.Run();

/// <summary>Entry point, exposed so integration tests can host the service.</summary>
public partial class Program;
