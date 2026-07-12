using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services
    .AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training", options => { });

builder.Services.AddAuthorization();

var app = builder.Build();

// Request pipeline
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

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