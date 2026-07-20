using Microsoft.Extensions.Logging;

//using TmsApi.Application.Interfaces;
//namespace TmsApi.Infrastructure.Services;/
namespace TmsApi.Infrastructure.Services;

public interface IStudentService
{
    Task<StudentRecord> CreateAsync(string id, string name);
    Task<StudentRecord?> GetByIdAsync(string id);
    Task<IReadOnlyList<StudentRecord>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
}
public class StudentService : IStudentService
{
    private readonly Dictionary<string, StudentRecord> _store = new();
    private readonly ILogger<StudentService> _logger;

    public StudentService(ILogger<StudentService> logger)
    {
        _logger = logger;
    }

    public Task<StudentRecord> CreateAsync(string id, string name)
    {
        if (_store.ContainsKey(id))
        {
            _logger.LogWarning("Duplicate student {StudentId}", id);
            return Task.FromResult(_store[id]);
        }

        var record = new StudentRecord(id, name, DateTime.UtcNow);
        _store[id] = record;

        _logger.LogInformation("Added student {StudentId}", id);
        return Task.FromResult(record);
    }

    public Task<StudentRecord?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var record);
        if (record is null)
            _logger.LogWarning("Student {StudentId} not found", id);

        return Task.FromResult(record);
    }

    public Task<IReadOnlyList<StudentRecord>> GetAllAsync()
    {
        IReadOnlyList<StudentRecord> all = _store.Values.ToList();
        return Task.FromResult(all);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.Remove(id);
        if (removed)
            _logger.LogInformation("Deleted student {StudentId}", id);
        else
            _logger.LogWarning("Delete failed student {StudentId} not found", id);

        return Task.FromResult(removed);
    }
    public Task<IReadOnlyList<StudentRecord>> GetPagedStudentsAsync(
            int pageNumber,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
    {
        var pagedStudents = _store.Values
            .OrderBy(student => student.Name)                // stable sort by name
            .Skip((pageNumber - 1) * pageSize)               // calculate offset
            .Take(pageSize)                                  // take only pageSize items
            .ToList();

        return Task.FromResult<IReadOnlyList<StudentRecord>>(pagedStudents);
    }

}


public record StudentRecord(string Id, string Name, DateTime CreatedAt);
