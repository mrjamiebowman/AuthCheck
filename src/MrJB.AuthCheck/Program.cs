using MrJB.AuthCheck;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;
using MrJB.AuthCheck.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

// logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Code,
        outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}" +
            "{Message:lj}{NewLine}" +
            "{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.AddServiceDefaults();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<OAuthCheckConfiguration>(builder.Configuration.GetSection(OAuthCheckConfiguration.Position));

builder.Services.AddHttpClient<IAuthCheckService, AuthCheckService>();

// worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
