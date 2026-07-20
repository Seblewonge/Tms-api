using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Infrastructure.Persistence;
namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(TmsDbContext context) : ControllerBase
{
    // 1. Active students with GPA >= 3.0
    [HttpGet("active-students-count")]
    public async Task<IActionResult> GetActiveStudentsCount()
    {
        var count = await context.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m)
            .CountAsync();

        return Ok(count);
    }

    // 2. Courses with the most enrollments
    [HttpGet("course-enrollments")]
    public async Task<IActionResult> GetCourseEnrollments()
    {
        var list = await context.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ToListAsync();

        return Ok(list);
    }

    // 3. Average GPA per course
    [HttpGet("average-gpa-per-course")]
    public async Task<IActionResult> GetAverageGpaPerCourse()
    {
        var list = await context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                AverageGPA = g.Average(e => e.Student.GPA)
            })
            .ToListAsync();

        return Ok(list);
    }

    // 4A. Students with zero enrollments (NOT EXISTS)
    [HttpGet("students-without-enrollments")]
    public async Task<IActionResult> GetStudentsWithoutEnrollments()
    {
        var list = await context.Students
            .Where(s => !s.Enrollments.Any())
            .Select(s => s.Name)
            .ToListAsync();

        return Ok(list);
    }

    // 4B. Students with zero enrollments (LEFT JOIN - EF Core 10)
    [HttpGet("students-without-enrollments-leftjoin")]
    public async Task<IActionResult> GetStudentsWithoutEnrollmentsLeftJoin()
    {
        var list = await context.Students
            .LeftJoin(
                context.Enrollments,
                s => s.Id,
                e => e.StudentId,
                (s, e) => new { s, e })
            .Where(x => x.e == null)
            .Select(x => x.s.Name)
            .ToListAsync();

        return Ok(list);
    }
// GET: api/reports/students?page=1
[HttpGet("students")]
public async Task<IActionResult> GetStudents(
    int page = 1,
    CancellationToken cancellationToken = default)
{
    const int pageSize = 20;

    var students = await context.Students
        .OrderBy(s => s.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return Ok(students);
}
// GET: api/reports/top-courses
[HttpGet("top-courses")]
public async Task<IActionResult> GetTopCourses(
    CancellationToken cancellationToken = default)
{
    var result = await context.Enrollments
        .GroupBy(e => e.Course.Title)
        .Select(g => new
        {
            CourseTitle = g.Key,
            EnrollmentCount = g.Count()
        })
        .OrderByDescending(x => x.EnrollmentCount)
        .Take(5)
        .ToListAsync(cancellationToken);

    return Ok(result);
}
[HttpGet("nplusone")]
public async Task<IActionResult> NPlusOne(CancellationToken cancellationToken)
{
    var students = await context.Students
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var result = new List<object>();

    foreach (var s in students)
    {
        var count = await context.Enrollments
            .AsNoTracking()
            .CountAsync(e => e.StudentId == s.Id, cancellationToken);

        result.Add(new
        {
            s.Name,
            EnrollmentCount = count
        });
    }

    return Ok(result);
}
[HttpGet("shaped-query")]
public async Task<IActionResult> ShapedQuery(CancellationToken cancellationToken)
{
    var report = await context.Students
        .AsNoTracking()
        .Select(s => new
        {
            s.Name,
            EnrollmentCount = s.Enrollments.Count
        })
        .ToListAsync(cancellationToken);

    return Ok(report);
}
}