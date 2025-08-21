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

app.UseOpenApi();

app.UseHttpsRedirection();

app.UseCorsConfig();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseEndpointsConfig();

app.Run();