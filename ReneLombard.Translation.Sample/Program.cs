using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ReneLombard.Translation.Sample
{
    class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            // Startup.cs finally :)
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();


            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Logger is working!");

            // Get Service and call method
            var service = serviceProvider.GetService<ConsoleService>();
            service.RunAsync().Wait();
        }
    }
}
