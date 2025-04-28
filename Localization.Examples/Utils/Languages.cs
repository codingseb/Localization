using CodingSeb.Localization.Loaders;
using System.IO;
using System.Reflection;

namespace CodingSeb.Localization.Examples
{
    public static class Languages
    {
        private static readonly string languagesFilesDirectory = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "lang");

        public static void Init()
        {
            Loc.Instance.LogOutMissingTranslations = true;

            LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());

            ReloadFiles();
        }

        public static void ReloadFiles()
        {
            string exampleFileFileName = Path.Combine(languagesFilesDirectory, "Example1.loc.json");
            LocalizationLoader.Instance.ClearAllTranslations();
            LocalizationLoader.Instance.AddFile(exampleFileFileName);
        }
    }
}
