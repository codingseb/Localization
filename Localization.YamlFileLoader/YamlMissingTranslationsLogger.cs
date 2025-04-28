using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;

namespace CodingSeb.Localization
{
    /// <summary>
    /// -- Describe here to what is this class used for. (What is it's purpose) --
    /// </summary>
    public static class YamlMissingTranslationsLogger
    {
        public static void EnableLog(Loc loc)
        {
            loc.LogOutMissingTranslations = true;
            loc.MissingTranslationFound += Loc_MissingTranslationFound;
        }

        public static void DisableLog(Loc loc)
        {
            loc.MissingTranslationFound -= Loc_MissingTranslationFound;
        }

        public static string MissingTranslationsFileName { get; set; } = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "LocalizationMissingTranslations.yaml");

        private static void Loc_MissingTranslationFound(object sender, LocalizationMissingTranslationEventArgs e)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(e.MissingTranslations);

            File.WriteAllText(MissingTranslationsFileName, yaml);
        }
    }
}
