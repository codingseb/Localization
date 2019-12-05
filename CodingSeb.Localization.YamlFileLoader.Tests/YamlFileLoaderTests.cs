using CodingSeb.Localization.Loaders;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace CodingSeb.Localization.Tests
{
    public class YamlFileLoaderTests
    {
        private LocalizationLoader loader;
        private Loc loc;

        [OneTimeSetUp]
        public void LoadYaml()
        {
            loc = new Loc();

            loader = new LocalizationLoader(loc);

            loader.FileLanguageLoaders.Add(new YamlFileLoader());

            string directory = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "lang");

            loader.AddDirectory(directory);
        }

        [TestCase("LanguageName", "Notknown", "en", null, ExpectedResult = "English")]
        [TestCase("LanguageName", "Notknown", "en", "es", ExpectedResult = "Español")]
        [TestCase("LanguageName", "Notknown", "en", "it", ExpectedResult = "Notknown")]
        [TestCase("LanguageName", "Notknown", "it", null, ExpectedResult = "Notknown")]
        [TestCase("LanguageName", "Notknown", "fr", null, ExpectedResult = "Français")]
        [TestCase("HelloInCurrentLanguage", "Notknown", "en", null, ExpectedResult = "Hello in the current language")]
        [TestCase("HelloInCurrentLanguage", "Notknown", "es", null, ExpectedResult = "Hola en la lengua actual")]
        [TestCase("HelloInCurrentLanguage", "Notknown", "fr", null, ExpectedResult = "Bonjour dans la langue actuelle")]

        public string YamlBasicTranslations(string textId, string defaultText, string currentLanguage, string forceCurrentLanguage)
        {
            if (currentLanguage != null)
                loc.CurrentLanguage = currentLanguage;

            if (forceCurrentLanguage != null)
            {
                return loc.Translate(textId, defaultText, forceCurrentLanguage);
            }
            else
            {
                return loc.Translate(textId, defaultText);
            }
        }
    }
}