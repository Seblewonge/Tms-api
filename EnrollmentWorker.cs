// module 4 exercise 2
using TmsApi.Services;
public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        // ✅ Create a short-lived scope
        using var scope = _scopeFactory.CreateScope();

        // ✅ Resolve the scoped service inside this scope
        var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // ✅ Use the service
        // var all = svc.GetAllAsync().Result;

        // Example: log or process enrollments
        // Console.WriteLine($"Processed {all.Count} enrollments at {DateTime.UtcNow}");
    }
}
