using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using TmsApi.Middleware;
using TmsApi.Data;
using TmsApi.Services;
using TmsApi.Data.Persistence;
using Tms.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Controllers and services
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddControllers(options =>
{
options.Filters.Add<AuditLogFilter>();
});
// Authentication
builder.Services
    .AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training", options => { });

builder.Services.AddAuthorization();

// Payment options
builder.Services
    .AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Database
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging());

builder.Services.AddOpenApi("v1", options =>
{
options.ShouldInclude = description =>
description.GroupName == "v1";
});
builder.Services.AddOpenApi("v2", options =>
{
options.ShouldInclude = description =>
description.GroupName == "v2";
});
builder.Services.AddApiVersioning(options =>
{
options.DefaultApiVersion = new ApiVersion(1, 0);
options.AssumeDefaultVersionWhenUnspecified = true;
options.ReportApiVersions = true;
options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
options.GroupNameFormat = "'v'VVV";
options.SubstituteApiVersionInUrl = true;
});
// update your scalar config
// app.MapScalarApiReference(options =>
// {
// options.WithTitle("TMS API Reference")
// .WithTheme(ScalarTheme.DeepSpace)
// .WithDefaultHttpClient(ScalarTarget.CSharp,
// ScalarClient.HttpClient);
// // Tell Scalar to pull both documents into its sidebar dropdown
// options
// .AddDocument("v1", "API Version 1.0")
// .AddDocument("v2", "API Version 2.0");
// });
 var app = builder.Build();


// Development tools
if (app.Environment.IsDevelopment())
{
     app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("TMS API Reference")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(
                ScalarTarget.CSharp,
                ScalarClient.HttpClient);

        // Tell Scalar to show both API versions
        options
            .AddDocument("v1", "API Version 1.0")
            .AddDocument("v2", "API Version 2.0");
    });
}
else
{
    app.UseExceptionHandler();
}


// Middleware pipeline
app.UseStatusCodePages();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseMiddleware<V1DeprecationMiddleware>();

// Controllers
app.MapControllers();


// Protected endpoint
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        message = "Assessment results retrieved successfully."
    });
})
.RequireAuthorization();


// Error testing endpoint
app.MapGet("/api/error", () =>
{
    throw new Exception("Simulated database failure for ProblemDetails testing");
});


// Apply migrations and run deterministic seeder
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    await context.Database.MigrateAsync();
}


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    await DataSeeder.SeedAsync(context);
}


app.Run();