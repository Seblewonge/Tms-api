using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Scalar.AspNetCore;
using TmsApi.Middleware;
using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Services;
using Tms.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
// Services
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IEnrollmentService,EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();


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
// Register TmsDbContext scoped for incoming HTTP requests
builder.Services.AddDbContext<TmsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")).LogTo(Console.WriteLine, LogLevel.Information) // Log SQL to output window
.EnableSensitiveDataLogging());
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
throw new Exception("Simulated database failure for ProblemDetails testing");
});
// Seed test data at startup
using (var scope = app.Services.CreateScope())
{
var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
context.Database.Migrate(); // Applies any pending migrations; keeps migration history intact
if (!context.Students.Any())
{
var students = new List<Student>
{
new() { RegistrationNumber = "TMS-2026-0001", Name = "AliceSmith", GPA = 3.8m, Age = 23, IsActive = true },
new() { RegistrationNumber = "TMS-2026-0002", Name = "Bob Jones", GPA = 2.9m,  Age = 24, IsActive = true },
new() { RegistrationNumber = "TMS-2026-0003", Name = "Charlie Brown", GPA = 3.4m, Age = 26, IsActive = false },
new() { RegistrationNumber = "TMS-2026-0004", Name = "DianaPrince", GPA = 3.9m,  Age = 25,IsActive = true },
new() { RegistrationNumber = "TMS-2026-0005", Name = "EvanWright", GPA = 2.5m,  Age = 23, IsActive = true }
};
context.Students.AddRange(students);
var courses = new List<Course>
{
new() { Code = "CS-101", Title = "Introduction to ComputerScience", MaxCapacity = 30 },
new() { Code = "CS-201", Title = "Data Structures and Algorithms", MaxCapacity = 25 },
new() { Code = "MAT-101", Title = "Calculus I", MaxCapacity =40 }
};
context.Courses.AddRange(courses);
context.SaveChanges();
var enrollments = new List<Enrollment>
{
new() { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 4.0m },
new() { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 3.6m },
new() { StudentId = students[1].Id, CourseId = courses[0].Id, Grade = 2.8m },
new() { StudentId = students[3].Id, CourseId = courses[1].Id, Grade = 3.9m }
};
context.Enrollments.AddRange(enrollments);
context.SaveChanges();
}
}

app.Run();