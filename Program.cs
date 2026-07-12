using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using TmsApi.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
// Services
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseExceptionHandler();
}
//app.UseExceptionHandler();
// Request pipeline
app.UseStatusCodePages();
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
app.MapGet("/api/error", () =>
{
throw new TmsDatabaseException("Simulated database failure for ProblemDetails testing");
});

app.Run();