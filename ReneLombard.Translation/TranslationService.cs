using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReneLombard.Translation.DataStore;
using ReneLombard.Translation.Helper;
using ReneLombard.Translation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReneLombard.Translation
{
    public class TranslationService<T>
    {
        private AzureTranslationClient _client;
        private readonly InMemoryCollection _inMemoryCollection;
        public TranslationService(AzureTranslationClient client, InMemoryCollection inMemoryCollection)
        {
            this._inMemoryCollection = inMemoryCollection ?? throw new ArgumentNullException(nameof(inMemoryCollection));
            this._client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<bool> IsSupportedLanguageAsync(string languageCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentNullException(nameof(languageCode));
            var result = await GetSupportedLanguagesAsync(cancellationToken);
            return result.SupportedLanguages.Keys.Contains(languageCode);
        }

        public async Task<SupportedTranslationLanguages> GetSupportedLanguagesAsync(CancellationToken cancellationToken = default)
        {
            if (_inMemoryCollection.SupportedTranslationLanguages == null
                || _inMemoryCollection.SupportedTranslationLanguages.SupportedLanguages == null
                || _inMemoryCollection.SupportedTranslationLanguages.SupportedLanguages.Count == 0)
            {
                _inMemoryCollection.SupportedTranslationLanguages
                    = await _client.GetSupportedLanguagesAsync(cancellationToken);
            }
            return _inMemoryCollection.SupportedTranslationLanguages;
        }

        public async Task<T> TranslateAsync(T value, string language, CancellationToken cancellationToken = default)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException(nameof(language));
            }
            string json = JsonConvert.SerializeObject(value, new Formatting() { });

            var reader = new JsonTextReader(new StringReader(json));
            reader.DateParseHandling = DateParseHandling.None;
            var o = JObject.Load(reader);

            var result = JSONFlattener.Flatten(o);
            var listOfStrings = new List<string>();
            foreach (var item in result)
            {
                if ((item.Key.type == "String") && (!Regex.IsMatch(item.Value, "(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)")))
                {
                    listOfStrings.Add(item.Value);
                }
            }
            var translatedList = await _client
                .TranslateAsync(listOfStrings, language, cancellationToken)
                .ConfigureAwait(false);

            var count = 0;

            var newList = new Dictionary<(string path, string type), string>();
            foreach (var item in result)
            {
                if (item.Key.type == "String" && (!Regex.IsMatch(item.Value, "(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)")))
                {
                    newList.Add(item.Key, translatedList[count++]);
                }
                else
                {
                    newList.Add(item.Key, item.Value);
                }
            }
            var unflattern = JSONFlattener.Unflatten(newList);
            return unflattern.ToObject<T>();
        }
    }
}
