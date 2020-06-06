using System;

namespace ReneLombard.Translation.Models
{
    [Serializable]
    public class SupportedLanguages
    {
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Dir { get; set; }
    }
}
