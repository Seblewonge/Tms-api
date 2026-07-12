using Microsoft.AspNetCore.Authentication;
using TmsApi.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

builder.Services
    .AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training", options => { });

builder.Services.AddAuthorization();

builder.Services
    .AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

// Request pipeline

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

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

app.Run();