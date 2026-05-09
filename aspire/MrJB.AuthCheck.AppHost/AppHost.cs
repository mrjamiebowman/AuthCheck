var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MrJB_AuthCheck>("mrjb-authcheck");

builder.Build().Run();
