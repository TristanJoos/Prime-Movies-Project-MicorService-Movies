using Howestprime.Movies.Main.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModules(builder.Configuration);

WebApplication app = await builder.Build().UseModules();

await app.RunAsync();
