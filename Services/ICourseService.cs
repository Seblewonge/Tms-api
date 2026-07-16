using TmsApi.Entities;
using Tms.Api.Dtos;
namespace TmsApi.Services;

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