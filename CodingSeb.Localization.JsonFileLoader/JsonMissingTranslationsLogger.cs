using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace CodingSeb.Localization
{
    public static class JsonMissingTranslationsLogger
    {
        public static void EnableLogFor(Loc loc)
        {
            loc.LogOutMissingTranslations = true;
            loc.MissingTranslationFound += Loc_MissingTranslationFound;
        }

        public static void DisableLogFor(Loc loc)
        {
            loc.MissingTranslationFound -= Loc_MissingTranslationFound;
        }

        public static string MissingTranslationsFileName { get; set; } = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "LocalizationMissingTranslations.json");

        private static void Loc_MissingTranslationFound(object sender, LocalizationMissingTranslationEventArgs e)
        {
            File.WriteAllText(MissingTranslationsFileName,
                JsonConvert.SerializeObject(e.MissingTranslations, Formatting.Indented));
        }
    }
}
