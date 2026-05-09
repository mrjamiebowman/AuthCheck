using MrJB.AuthCheck;
using MrJB.AuthCheck.Domain.Configuration;
using MrJB.AuthCheck.Domain.Interfaces;
using MrJB.AuthCheck.Services;

var builder = WebApplication.CreateBuilder(args);

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
