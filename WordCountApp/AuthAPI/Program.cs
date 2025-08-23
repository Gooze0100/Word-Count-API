using AuthAPI.Startup;
using Shared;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddConfiguration();
builder.AddSettings();

builder.Services.AddOpenApi();

builder.AddCors();

builder.AddDependencies();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseOpenApi();

app.UseCorsConfig();

app.UseEndpointsConfig();

app.Run();