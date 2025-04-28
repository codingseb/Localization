using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace CodingSeb.Localization
{
    /// <summary>
    /// A logguer for missing translations in Json format
    /// </summary>
    public static class JsonMissingTranslationsLogger
    {
        /// <summary>
        /// Subscribe to MissingTranslationFound event to log it on the specified loc instanc
        /// </summary>
        /// <param name="loc">the loc instance to observe</param>
        public static void EnableLogFor(Loc loc)
        {
            loc.LogOutMissingTranslations = true;
            loc.MissingTranslationFound += Loc_MissingTranslationFound;
        }

        /// <summary>
        /// Unsubscribe from MissingTranslationFound event to stop logs on the specified loc instanc
        /// </summary>
        /// <param name="loc">the loc instance to unsubscribe from</param>
        public static void DisableLogFor(Loc loc)
        {
            loc.MissingTranslationFound -= Loc_MissingTranslationFound;
        }

        /// <summary>
        /// The FileName of je json file where to log missing tranlsations
        /// </summary>
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
