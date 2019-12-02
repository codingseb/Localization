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
            string exampleFileFileName = Path.Combine(languagesFilesDirectory, "Example1.loc.json");
            LocalizationLoader.Instance.FileLanguageLoaders.Add(new JsonFileLoader());

            LocalizationLoader.Instance.AddFile(exampleFileFileName);
        }
    }
}
