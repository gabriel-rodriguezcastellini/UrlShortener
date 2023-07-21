using Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using WebAPI.CustomExceptionMiddleware;
using WebAPI.ExceptionFilters;
using WebAPI.Extensions;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Context>(optionsAction => optionsAction.UseSqlServer(builder.Configuration.GetConnectionString("db")!, sqlServerOptionsAction =>
{
    sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
}));

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("seq")!));

Serilog.Debugging.SelfLog.Enable(Console.Error);
builder.Services.AddResponseCaching();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpResponseExceptionFilter>();
    options.CacheProfiles.Add("Default30", new CacheProfile { Duration = 30 });
}).ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(context.ModelState) { ContentTypes = { Application.Json, Application.Xml } };
        options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";
    }).AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "URL Shortener API",
        Description = "An URL shortener service",
        Contact = new OpenApiContact
        {
            Name = "Gabriel Rodríguez Castellini",
            Url = new Uri("https://www.linkedin.com/in/gabrielrodriguezcastellini/"),
            Email = "gabriel.rodriguezcastellini@outlook.com"
        }
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

//Configure other services up here
builder.Services.AddTransient<Random>();
builder.Services.AddTransient<RandomUrlGenerator>();
builder.Services.AddTransient<ShortUrlRepository>();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("This service is healthy"), new string[] { "self" })
    .AddSqlServer(builder.Configuration.GetConnectionString("db"), name: "APIDb-check", tags: new string[] { "apidb" });

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseResponseCaching();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(configure =>
{
    configure.MapDefaultControllerRoute();
    configure.MapControllers();
    configure.MapHealthChecks("/hc", new HealthCheckOptions() { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
    configure.MapHealthChecks("/liveness", new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
});

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.InjectStylesheet("/swagger-ui/custom.css");
});

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<Context>().Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}

app.ConfigureExceptionHandler();
app.ConfigureCustomExceptionMiddleware();
app.UseStatusCodePagesWithReExecute("/StatusCode", "?statusCode={0}");

app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(10)
        };

    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };
    await next();
});

app.MapControllers();
app.Run();
