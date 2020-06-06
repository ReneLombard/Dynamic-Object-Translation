using Newtonsoft.Json;
using ReneLombard.Translation.Sample.Models;
using System;
using System.Threading.Tasks;

namespace ReneLombard.Translation.Sample
{
    public class ConsoleService
    {
        private TranslationService<BlogPost> _translationService;

        public ConsoleService(TranslationService<BlogPost> translationService)
        {
            this._translationService = translationService;
        }
        public async Task RunAsync()
        {
            Console.WriteLine("Test");

            var faker = new AutoBogus.AutoFaker<BlogPost>();
            var result = faker.Generate();
            Console.WriteLine("================================================");
            Console.WriteLine("Original Value");
            Console.WriteLine("================================================");

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            Console.WriteLine("================================================");
            Console.WriteLine("Translated Value");
            Console.WriteLine("================================================");

            Console.WriteLine(JsonConvert.SerializeObject(await this._translationService.TranslateAsync(result, "nl"), Formatting.Indented));
            Console.ReadKey();
            return;
        }
    }
}
