using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ReneLombard.Translation.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class SupportedTranslationLanguages
    {
        [JsonProperty(PropertyName = "translation")]
        public Dictionary<string, SupportedLanguages> SupportedLanguages { get; set; }
    }
}
