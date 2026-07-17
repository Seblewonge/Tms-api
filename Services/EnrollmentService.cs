using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Dtos.Enrollment;
using TmsApi.Entities;

using TmsApi.Services;


public class EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger) : IEnrollmentService
{
public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int
id, CancellationToken ct) =>
context.Enrollments
.AsNoTracking()
.Where(e => e.Id == id && e.CourseId == courseId)
.Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.
StudentId, e.EnrolledAt))
.FirstOrDefaultAsync(ct);
public async Task<EnrollmentResponseDto> CreateAsync(int courseId,
EnrollStudentRequest request, CancellationToken ct)
    {
          var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow
        };
context.Enrollments.Add(enrollment);

    await context.SaveChangesAsync(ct);

    logger.LogInformation(
        "Created enrollment {EnrollmentId} for Course {CourseId} and Student {StudentId}",
        enrollment.Id,
        enrollment.CourseId,
        enrollment.StudentId);

    return (await GetByIdAsync(courseId, enrollment.Id, ct))!;
    }
public async Task<List<EnrollmentResponseDto>> GetByCourseAsync(
    int courseId,
    CancellationToken ct)
{
    return await context.Enrollments
        .AsNoTracking()
        .Where(e => e.CourseId == courseId)
        .Select(e => new EnrollmentResponseDto
        (
             e.Id,
             e.StudentId,
            e.CourseId,
            e.EnrolledAt
        ))
        .ToListAsync(ct);
}
}