using TmsApi.Application.DTOs.Course;
using TmsApi.Application.DTOs.Paged;
namespace TmsApi.Application.Interfaces;

public interface ICourseService
{
    // Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    Task<CourseResponseDto?> GetByIdAsync(
         int id,
         CancellationToken ct);
    Task<CourseResponseDto> CreateAsync(
       CreateCourseRequest request,
       CancellationToken ct);

    Task<bool> CodeExistsAsync(
        string code,
        CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest
    request, CancellationToken ct);

}