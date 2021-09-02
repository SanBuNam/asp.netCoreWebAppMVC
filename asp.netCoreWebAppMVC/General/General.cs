using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.General
{
    /*Dependency Injection*/
    // A dependency is an object that another object depends on.
    // Examine the following "MessageWriter" class with a Write method that other classes depend on.
    public class MessageWriter
    {
        public void Write(string message)
        {
            Console.WriteLine($"MessageWriter.Write(message: \"{message}\")");
        }
    }

    // A class can create an instance of the MessageWriter class to make use of its Write method.
    // Following example, the MessageWriter class is a dependency of the Worker class.
    public class Worker : BackGroundService
    {
        private readonly MessageWriter _messageWriter = new MessageWriter();

        // Task Class : Represents an asynchronous operation
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                _messageWriter.Write($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
    // Hard-coded dependencies, such as in this example, are problematic and should be avoided for the following reasons.
    // 1. To replace MessageWriter with a different implementation, the Worker class must be modified.
    // 2. If MessageWriter has dependencies, they must also be configured by the Worker class.
    // In a large project with multiple classes depending on MessageWriter, the configuration code becomes scattered across the app.
    // 3. This implementation is difficult to unit test. The app should use a mock or stub MessageWriter class, which isn't possible with this approach.

    /*
     Dependency injection addresses these problems through:
        1. The use of an interface or base class to abstract the dependency implementation.
        2. Registration of the dependency in a service container. .NET provides a built-in service container, IServiceProvider. 
        Services are typically registered at the app's start-up, and appended to an IServiceCollection. Once all services are added, you use BuildServiceProvider to create the service container.
        3. Injection of the service into the constructor of the class where it's used. The framework takes on the responsibility of creating an instance of the dependency and disposing of it when it's no longer needed.
     */

    // As an example, the IMessageWriter interface defines the Write method
    public interface IMessageWriter
    {
        void Write(string message);
    }

    public class MessageWriter2 : IMessageWriter
    {
        public void Write(string message)
        {
            Console.WriteLine($"MessageWriter.write(message; \"{message}\")");
        }
    }

    //class Program
    //{
    //    static Task Main(string[] args) =>
    //        CreateHostBuilder(args).Build().RunAsync();

    //    static IHostBuilder CreateHostBuilder(string[] args) =>
    //        Host.CreateDefaultBuilder(args)
    //            .ConfigureServices((_, services) =>
    //                services.AddHostedService<Worker>()
    //                        .AddScoped<IMessageWriter, MessageWriter>());
    //}

    //public class Worker : BackgroundService
    //{
    //    private readonly IMessageWriter _messageWriter;

    //    public Worker(IMessageWriter messageWriter) =>
    //        _messageWriter = messageWriter;

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            _messageWriter.Write($"Worker running at: {DateTimeOffset.Now}");
    //            await Task.Delay(1000, stoppingToken);
    //        }
    //    }
    //}

    // This is not enough...

}
