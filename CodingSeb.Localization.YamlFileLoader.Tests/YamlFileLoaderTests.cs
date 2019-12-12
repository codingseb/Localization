using CodingSeb.Localization.Loaders;
using CodingSeb.Localization;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using Shouldly;

namespace CodingSeb.Localization.Tests
{
    [NonParallelizable]
    public class YamlFileLoaderTests
    {
        private static readonly string directory = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "lang");

        private readonly string missingForComparaisonFileName = Path.Combine(directory, "MissingTranslationsFileForComparaison.yaml");
        private readonly string missingFilesFileName = Path.Combine(Path.GetTempPath(), "MissingTranslations.yaml");
        private LocalizationLoader loader;
        private Loc loc;

        [OneTimeSetUp]
        public void LoadYaml()
        {
            loc = new Loc();

            loader = new LocalizationLoader(loc);

            loader.FileLanguageLoaders.Add(new YamlFileLoader());

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

        [Test]
        public void MissingTranslationsInYaml()
        {
            YamlMissingTranslationsLogger.MissingTranslationsFileName = missingFilesFileName;

            YamlMissingTranslationsLogger.EnableLogFor(loc);

            try
            {
                loc.CurrentLanguage = "it";

                loc.Translate("LanguageName", "Test").ShouldBe("Test");

                loc.MissingTranslations.ShouldContainKey("LanguageName");
                loc.MissingTranslations["LanguageName"].ShouldContainKey("it");
                loc.MissingTranslations["LanguageName"]["it"].ShouldBe("default text : Test");

                loc.CurrentLanguage = "en";

                loc.Translate("NotExistingTextId", "Test2").ShouldBe("Test2");

                loc.MissingTranslations.ShouldContainKey("NotExistingTextId");
                loc.MissingTranslations["NotExistingTextId"].ShouldContainKey("en");
                loc.MissingTranslations["NotExistingTextId"]["en"].ShouldBe("default text : Test2");

                File.ReadAllText(missingFilesFileName).ShouldBe(File.ReadAllText(missingForComparaisonFileName));
            }
            finally
            {
                YamlMissingTranslationsLogger.DisableLogFor(loc);
            }
        }
    }
}