using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

public class DataProcessingService
{
    private readonly ILogger<DataProcessingService> _logger;
    private readonly ConcurrentQueue<string> _dataQueue = new ConcurrentQueue<string>();

    public DataProcessingService(ILogger<DataProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task EnqueueDataAsync(string data)
    {
        await Task.Delay(50); // Simulate some async work
        _dataQueue.Enqueue(data);
        _logger.LogInformation($"Data enqueued: {data}");
    }

    public async Task<string> ProcessNextItemAsync()
    {
        if (_dataQueue.TryDequeue(out string? item))
        {
            if (item is null)
            {
                _logger.LogWarning("Dequeued item is null");
                return string.Empty;
            }

            await Task.Delay(100); // Simulate processing time
            string processedItem = $"Processed: {item.ToUpper()}";
            _logger.LogInformation($"Item processed: {processedItem}");
            return processedItem;
        }
        
        _logger.LogWarning("No items to process");
        return string.Empty;
    }

    public async Task<int> GetQueueSizeAsync()
    {
        await Task.Delay(10); // Simulate a small delay
        int size = _dataQueue.Count;
        _logger.LogInformation($"Current queue size: {size}");
        return size;
    }

    public async Task ClearQueueAsync()
    {
        await Task.Delay(200); // Simulate clearing time
        _dataQueue.Clear();
        _logger.LogInformation("Queue cleared");
    }
}
