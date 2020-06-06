using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReneLombard.Translation.Extensions;
using ReneLombard.Translation.Options;
using ReneLombard.Translation.Sample.Models;

namespace ReneLombard.Translation.Sample
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(x => x.AddConsole());

            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<ConsoleService>();
            services.Configure<AzureConfig>(Configuration.GetSection("Azure"));
            services.AddTranslation<BlogPost>();
        }
    }
}
