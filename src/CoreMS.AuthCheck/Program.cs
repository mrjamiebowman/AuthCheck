using CoreMS.AuthCheck;
using CoreMS.AuthCheck.Domain.Configuration;
using CoreMS.AuthCheck.Domain.Interfaces;
using CoreMS.AuthCheck.ServiceDefaults;
using CoreMS.AuthCheck.Services;

var builder = WebApplication.CreateBuilder(args);

/******************************************/
/*            configuration               */
/******************************************/

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.mrjamiebowman.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true)
    .Build();

// azure app config
builder.ConfigureAzureAppConfiguration();

/******************************************/
/*                   app                  */
/******************************************/

builder.Services.Configure<AuthCheckConfiguration>(builder.Configuration.GetSection(AuthCheckConfiguration.Position));

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// services
builder.Services.AddHttpClient<IAuthCheckService, AuthCheckService>();

// worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

/******************************************/
/*                   run                  */
/******************************************/

app.MapDefaultEndpoints();

// openapi
app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
