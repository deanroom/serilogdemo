using Microsoft.Extensions.Logging;

public class MyCustomService : IAsyncDisposable
{
    private readonly ILogger<MyCustomService> _logger;

    public MyCustomService(ILogger<MyCustomService> logger)
    {
        _logger = logger;
    }

    public async Task CreateAsync()
    {
        _logger.LogInformation("Creating MyCustomService");
        await Task.Delay(100); // Simulating some async work
    }

    public async Task DoWorkAsync()
    {
        _logger.LogInformation("Doing work in MyCustomService");
        await Task.Delay(500); // Simulating some async work
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing MyCustomService");
        await Task.Delay(100); // Simulating some async cleanup work
    }
}
