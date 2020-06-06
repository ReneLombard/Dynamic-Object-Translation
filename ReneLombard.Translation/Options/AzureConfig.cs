namespace ReneLombard.Translation.Options
{
    public class AzureConfig
    {
        public string TranslationApiVersion { get; set; } = "3.0";
        public string TranslationSubscriptionKey { get; set; }
        public string TranslationBaseUrl { get; set; } = "https://azure.microsoft.com/en-us/services/cognitive-services/translator/#features";
    }
}
