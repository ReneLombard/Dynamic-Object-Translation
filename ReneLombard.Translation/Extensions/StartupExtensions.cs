using Microsoft.Extensions.DependencyInjection;
using ReneLombard.Translation.DataStore;
using System.Net.Http;

namespace ReneLombard.Translation.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddTranslation<T>(this IServiceCollection services)
        {
            services.AddSingleton<InMemoryCollection>();
            services.AddSingleton<HttpClient>();
            services.AddTransient<AzureTranslationClient>();
            services.AddTransient<ReneLombard.Translation.TranslationService<T>>();
            return services;
        }
    }
}
