// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Starting up");
            var host = CreateHostBuilder(args).Build();

            await using (var scope = host.Services.CreateAsyncScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var myService = services.GetRequiredService<MyCustomService>();
                var dataProcessingService = services.GetRequiredService<DataProcessingService>();

                logger.LogInformation("Services resolved");

                await myService.CreateAsync();
                await myService.DoWorkAsync();

                await dataProcessingService.EnqueueDataAsync("Hello");
                await dataProcessingService.EnqueueDataAsync("World");
                
                string processedItem = await dataProcessingService.ProcessNextItemAsync();
                logger.LogInformation("Processed item: {ProcessedItem}", processedItem);

                int queueSize = await dataProcessingService.GetQueueSizeAsync();
                logger.LogInformation("Queue size: {QueueSize}", queueSize);

                await dataProcessingService.ClearQueueAsync();

                logger.LogInformation("All operations completed");
            }

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<MyCustomService>();
                services.AddSingleton<DataProcessingService>();
            });
}
