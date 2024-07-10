using CodingSeb.Localization.Loaders;
using CodingSeb.Localization.Translators;
using NUnit.Framework;
using Shouldly;
using System.IO;
using System.Reflection;

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

            loader = new LocalizationLoader();

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

            YamlMissingTranslationsLogger.EnableLog();

            try
            {
                loc.CurrentLanguage = "it";

                loc.Translate("LanguageName", "Test").ShouldBe("Test");

                Loc.MissingTranslations.ShouldContainKey("LanguageName");
                Loc.MissingTranslations["LanguageName"].ShouldContainKey("it");
                Loc.MissingTranslations["LanguageName"]["it"].ShouldBe("default text : Test");

                loc.CurrentLanguage = "en";

                loc.Translate("NotExistingTextId", "Test2").ShouldBe("Test2");

                Loc.MissingTranslations.ShouldContainKey("NotExistingTextId");
                Loc.MissingTranslations["NotExistingTextId"].ShouldContainKey("en");
                Loc.MissingTranslations["NotExistingTextId"]["en"].ShouldBe("default text : Test2");

                File.ReadAllText(missingFilesFileName).ShouldBe(File.ReadAllText(missingForComparaisonFileName));
            }
            finally
            {
                YamlMissingTranslationsLogger.DisableLog();

                try
                {
                    File.Delete(missingFilesFileName);
                }
                catch { }
            }
        }
    }
}