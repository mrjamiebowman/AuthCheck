using CoreMS.AuthCheck;
using CoreMS.AuthCheck.Domain.Configuration;
using CoreMS.AuthCheck.Domain.Interfaces;
using CoreMS.AuthCheck.ServiceDefaults;
using CoreMS.AuthCheck.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

/******************************************/
/*                logging                 */
/******************************************/

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

/******************************************/
/*            configuration               */
/******************************************/

// environment
var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

// configuration
builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.mrjamiebowman.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// startup logs
Log.Logger.Information("Starting Auth Check, Environment: {environment}", environment);

// app
builder.Services.Configure<AuthCheckConfiguration>(builder.Configuration.GetSection(AuthCheckConfiguration.Position));

/******************************************/
/*                serilog                 */
/******************************************/

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    var otlpEndpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .WriteTo.Console(
            theme: AnsiConsoleTheme.Code,
            outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}" +
                "{Message:lj}{NewLine}" +
                "{Exception}"
    );

    // serilog dumps the default logger and replaces it...
    // without this serilog will block logs being shipped to otel/aspire.
    // i.e., removet his an structured logs goes away...
    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
    {
        loggerConfiguration.WriteTo.OpenTelemetry(options =>
        {
            options.Endpoint = otlpEndpoint;
        });
    }
});

builder.AddServiceDefaults();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
