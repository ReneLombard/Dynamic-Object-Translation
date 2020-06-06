using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReneLombard.Translation.DataStore;
using ReneLombard.Translation.Extensions;
using ReneLombard.Translation.Models;
using ReneLombard.Translation.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReneLombard.Translation
{
    public class AzureTranslationClient
    {
        private HttpClient _client;
        private ILogger<AzureTranslationClient> _logger;
        private readonly AzureConfig _config;
        private InMemoryCollection _inMemoryCollection;

        public AzureTranslationClient(HttpClient client, ILogger<AzureTranslationClient> logger, IOptions<AzureConfig> config, InMemoryCollection inMemoryCollection)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.BaseAddress = new Uri(_config.TranslationBaseUrl);
            _inMemoryCollection = inMemoryCollection ?? throw new ArgumentNullException(nameof(inMemoryCollection));
            _logger = logger;
        }

        public async Task<SupportedTranslationLanguages> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var languagesUrl = new Uri($"/languages?api-version={_config.TranslationApiVersion}", UriKind.Relative);
                var res = await _client.GetAsync(languagesUrl, cancellationToken);
                res.EnsureSuccessStatusCode();
                var jsonResults = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SupportedTranslationLanguages>(jsonResults);
                _inMemoryCollection.SupportedTranslationLanguages = result;
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to CognitiveService API {ex.ToString()}");
                throw;
            }
        }
        public async Task<string> Translate(string message, string language, CancellationToken cancellationToken = default)
        {
            try
            {
                System.Object[] body = new System.Object[] { new { Text = message } };
                var requestBody = JsonConvert.SerializeObject(body);

                var translateUrl = new Uri($"/translate?api-version={_config.TranslationApiVersion}&to={language}", UriKind.Relative);

                string jsonResults = null;
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = translateUrl;
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", _config.TranslationSubscriptionKey);

                    var res = await _client.SendAsync(request, cancellationToken);
                    jsonResults = await res.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TranslateResponse[]>(jsonResults);
                    return result.FirstOrDefault().Translations.FirstOrDefault().Text;
                }

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to CognitiveService API {ex.ToString()}");
                throw;
            }
        }

        public async Task<List<string>> TranslateAsync(List<string> messages, string language, CancellationToken cancellationToken = default)
        {
            try
            {
                var body = new List<object>();
                foreach (var message in messages)
                {
                    body.Add(new
                    {
                        Text = message
                    });
                }

                List<(int, Task<List<string>>)> tasks = new List<(int, Task<List<string>>)>();
                int count = 0;

                foreach (var item in body.Split(10))
                    tasks.Add((count++, Task.Run<List<string>>(() => DoTranlationAsync(language, item, cancellationToken), cancellationToken)));

                await Task.WhenAll(tasks.Select(x => x.Item2).ToArray());
                var list = new List<string>();
                for (int k = 0; k < tasks.Count; k++)
                {
                    var task = tasks.Where(x => x.Item1 == k).Select(m => m.Item2).FirstOrDefault();
                    list.AddRange(await task);
                }

                return list;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to CognitiveService API {ex.ToString()}");
                throw;
            }
        }

        private async Task<List<string>> DoTranlationAsync(string language, List<object> body, CancellationToken cancellationToken = default)
        {
            var requestBody = JsonConvert.SerializeObject(body);

            var translateUrl = new Uri($"/translate?api-version={_config.TranslationApiVersion}&to={language}", UriKind.Relative);

            string jsonResults = null;
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = translateUrl;
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _config.TranslationSubscriptionKey);

                var res = await _client.SendAsync(request, cancellationToken);
                jsonResults = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TranslateResponse[]>(jsonResults);
                var translations = new List<string>();
                foreach (var translationResponse in result)
                {
                    foreach (var translation in translationResponse.Translations)
                    {
                        translations.Add(translation.Text);
                    }
                }
                return translations;
            }
        }
    }
}
