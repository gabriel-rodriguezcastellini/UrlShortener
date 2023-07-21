using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebFrontEnd;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("Self", () => HealthCheckResult.Healthy("This service is healthy"), new string[] { "self" })
    .AddUrlGroup(new Uri(builder.Configuration.GetValue<string>("ApiHealthCheck")), "api-check", tags: new string[] { "api" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(configure =>
{
    configure.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
    configure.MapHealthChecks("/hc", new HealthCheckOptions() { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
});

app.MapRazorPages();
app.Run();
