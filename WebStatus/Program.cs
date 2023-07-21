using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy("This service is healthy"), new string[] { "self" });

builder.Services.AddHealthChecksUI(setupSettings =>
{
    setupSettings.SetHeaderText("Health Checks Status");
}).AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(configure =>
{
    configure.MapDefaultControllerRoute();
    configure.MapHealthChecks("/liveness", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = p => p.Name.Contains("self") });

    configure.MapHealthChecksUI(setupOptions =>
    {        
        setupOptions.UIPath = "/hc-ui";
        setupOptions.AddCustomStylesheet("wwwroot/css/site.css");
    });
});

app.MapRazorPages();
app.Run();
