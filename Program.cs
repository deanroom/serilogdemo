// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await using (var scope = host.Services.CreateAsyncScope())
        {
            var services = scope.ServiceProvider;
            var myService = services.GetRequiredService<MyCustomService>();
            var dataProcessingService = services.GetRequiredService<DataProcessingService>();

            await myService.CreateAsync();
            await myService.DoWorkAsync();

            // Use the new DataProcessingService
            await dataProcessingService.EnqueueDataAsync("Hello");
            await dataProcessingService.EnqueueDataAsync("World");
            
            string processedItem = await dataProcessingService.ProcessNextItemAsync();
            Console.WriteLine($"Processed item: {processedItem}");

            int queueSize = await dataProcessingService.GetQueueSizeAsync();
            Console.WriteLine($"Queue size: {queueSize}");

            await dataProcessingService.ClearQueueAsync();
        }

        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<MyCustomService>();
                services.AddSingleton<DataProcessingService>(); // Register the new service
            });
}
