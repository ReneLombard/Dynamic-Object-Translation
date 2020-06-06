using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReneLombard.Translation.DataStore;
using ReneLombard.Translation.Options;
using ReneLombard.Translation.Sample.Models;
using System.Net.Http;

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
            services.AddTransient<AzureTranslationClient>();
            services.AddTransient<ReneLombard.Translation.TranslationService<BlogPost>>();
            services.Configure<AzureConfig>(Configuration.GetSection("Azure"));
            services.AddSingleton<InMemoryCollection>();
            services.AddSingleton<HttpClient>();
            //services.AddSingleton<IMyService, MyService>();
        }
    }
}
