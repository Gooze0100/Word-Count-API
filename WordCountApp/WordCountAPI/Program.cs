using WordCountAPI.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiServices();
builder.Services.AddAllHealthChecks();

builder.AddConfiguration();
builder.AddSettings();

builder.AddAuth();

builder.AddCors();
builder.AddDependencies();

builder.AddOther();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseOpenApi();

app.UseRouting();
app.UseCorsConfig();

app.UseAuth();

app.UseEndpointsConfig();

app.Run();

public partial class Program { }