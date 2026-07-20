namespace TmsApi.Application.DTOs.Enrollment;
public record EnrollmentResponseDto(
int Id,
int CourseId,
int StudentId,
DateTime EnrolledAt);