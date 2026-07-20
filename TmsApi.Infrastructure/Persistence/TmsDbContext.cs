using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Persistence;
public class TmsDbContext(DbContextOptions<TmsDbContext> options) : DbContext(options)
{
      public DbSet<Student> Students => Set<Student>();
      public DbSet<Course> Courses => Set<Course>();
      public DbSet<Enrollment> Enrollments => Set<Enrollment>();
  public DbSet<Assessment> Assessment => Set<Assessment>();
  public DbSet<Certificate> Certificate => Set<Certificate>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TmsDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
}
}

