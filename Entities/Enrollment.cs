using System;
namespace TmsApi.Entities;
public class Enrollment
{
public int Id { get; set; }
public int StudentId { get; set; }
public int CourseId { get; set; }
public decimal? Grade { get; set; } // Nullable, as student m
// Nullable, as student may be currently enrolled
public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
// Navigation properties back to entities
public bool IsArchived { get; set; }
public Student Student { get; set; } = null!;
public Course Course { get; set; } = null!;
}