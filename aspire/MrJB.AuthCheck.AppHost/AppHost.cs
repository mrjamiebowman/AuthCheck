var builder = DistributedApplication.CreateBuilder(args);

// launch settings
var launchProfile = Environment.GetEnvironmentVariable("DOTNET_LAUNCH_PROFILE") ?? "https";

builder.AddProject<Projects.MrJB_AuthCheck>("mrjb-authcheck", launchProfile);

builder.Build().Run();
