using CodingSeb.Localization.Loaders;
using NUnit.Framework;
using Shouldly;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CodingSeb.Localization.Tests
{
    [NonParallelizable]
    public class LocalizationTests
    {
        private LocalizationLoader loader;
        private readonly string missingFilesFileName = Path.Combine(Path.GetTempPath(), "MissingTranslations.json");
        private readonly string localizationFileName = Path.Combine(Path.GetTempPath(), "LocFileTest.loc.json");
        private readonly string structuredTransFileName = Path.Combine(Path.GetTempPath(), "StructuredTrans.loc.json");

        #region Init and cleanup

        public string GetEmbeddedResource(string res)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{GetType().Namespace}.Resources.{res}")))
            {
                return reader.ReadToEnd();
            }
        }

        [OneTimeSetUp]
        public void LoadTranslations()
        {
            loader = new LocalizationLoader(Loc.Instance);
            JsonFileLoader jsonFileLoader = new JsonFileLoader();
            loader.FileLanguageLoaders.Add(jsonFileLoader);

            loader.AddTranslation("SayHello", "en", "Hello");
            loader.AddTranslation("SayHello", "fr", "Bonjour");

            File.WriteAllText(localizationFileName, GetEmbeddedResource("LocFileTest.loc.json"), Encoding.UTF8);
            File.WriteAllText(structuredTransFileName, GetEmbeddedResource("StructuredTrans.loc.json"), Encoding.UTF8);

            Thread.Sleep(1000);

            loader.AddFile(localizationFileName);
            loader.AddFile(structuredTransFileName);
            jsonFileLoader.LabelPathRootPrefix = "/";
            jsonFileLoader.LabelPathSeparator = "/";
            jsonFileLoader.LabelPathSuffix = ":";
            loader.AddFile(structuredTransFileName);

            Loc.Instance.Translators.Add(new InMemoryTranslator());
        }

        [OneTimeTearDown]
        public void ClearDicts()
        {
            loader.ClearAllTranslations();

            try
            {
                File.Delete(missingFilesFileName);
            }
            catch { }

            try
            {
                File.Delete(localizationFileName);
            }
            catch { }
        }

        #endregion

        [TestCase("TestNoTextId", "Test", null, null, ExpectedResult = "Test")]
        [TestCase("SayHello", "SH", null, null, ExpectedResult = "Hello")]
        [TestCase("SayHello", "SH", "fr", null, ExpectedResult = "Bonjour")]
        [TestCase("SayHello", "SH", "fr", "en", ExpectedResult = "Hello")]
        [TestCase("SayHello", "SH", "fr", "es", ExpectedResult = "SH")]
        [TestCase("LanguageName", "Notknown", "en", null, ExpectedResult = "English")]
        [TestCase("LanguageName", "Notknown", "en", "es", ExpectedResult = "Español")]
        [TestCase("LanguageName", "Notknown", "en", "it", ExpectedResult = "Notknown")]
        [TestCase("LanguageName", "Notknown", "it", null, ExpectedResult = "Notknown")]
        [TestCase("LanguageName", "Notknown", "fr", null, ExpectedResult = "Français")]
        [TestCase("HelloInCurrentLanguage", "Notknown", "en", null, ExpectedResult = "Hello in the current language")]
        [TestCase("HelloInCurrentLanguage", "Notknown", "es", null, ExpectedResult = "Hola en la lengua actual")]
        [TestCase("HelloInCurrentLanguage", "Notknown", "fr", null, ExpectedResult = "Bonjour dans la langue actuelle")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.Header", "Notknown", "en", null, ExpectedResult = "File")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.Header", "Notknown", "fr", null, ExpectedResult = "Fichier")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.NewMenuItem.Header", "Notknown", "en", null, ExpectedResult = "New")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.NewMenuItem.Header", "Notknown", "fr", null, ExpectedResult = "Nouveau")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.OpenMenuItem.Header", "Notknown", null , "en", ExpectedResult = "Open")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.OpenMenuItem.Header", "Notknown", null , "fr", ExpectedResult = "Ouvrir")]
        [TestCase("JsonTranslations.MainMenu.EditMenuItem.Header", "Notknown", null, "en", ExpectedResult = "Edit")]
        [TestCase("JsonTranslations.MainMenu.EditMenuItem.Header", "Notknown", null, "fr", ExpectedResult = "Edition")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/Header:", "Notknown", "en", null, ExpectedResult = "Edit")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/Header:", "Notknown", "fr", null, ExpectedResult = "Edition")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/CopyMenuItem/Header:", "Notknown", "en", null, ExpectedResult = "Copy")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/CopyMenuItem/Header:", "Notknown", "fr", null, ExpectedResult = "Copier")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/CutMenuItem/Header:", "Notknown", "en", null, ExpectedResult = "Cut")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/CutMenuItem/Header:", "Notknown", "fr", null, ExpectedResult = "Couper")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/PasteMenuItem/Header:", "Notknown", "en", null, ExpectedResult = "Paste")]
        [TestCase("/JsonTranslations/MainMenu/EditMenuItem/PasteMenuItem/Header:", "Notknown", "fr", null, ExpectedResult = "Coller")]
        [TestCase("->Hello", "Notknown", "en", null, ExpectedResult = "en : Hello")]
        public string StaticBasicTranslations(string textId, string defaultText, string currentLanguage, string forceCurrentLanguage)
        {
            if (currentLanguage != null)
                Loc.Instance.CurrentLanguage = currentLanguage;

            if (forceCurrentLanguage != null)
            {
                return Loc.Tr(textId, defaultText, forceCurrentLanguage);
            }
            else
            {
                return Loc.Tr(textId, defaultText);
            }
        }

        [Test]
        public void MissingTranslationsInJson()
        {
            int eventFired = 0;

            Loc.Instance.LogOutMissingTranslations = true;

            void Instance_MissingTranslationFound(object sender, LocalizationMissingTranslationEventArgs e)
            {
                eventFired++;
            }

            Loc.Instance.MissingTranslationFound += Instance_MissingTranslationFound;

            JsonMissingTranslationsLogger.MissingTranslationsFileName = missingFilesFileName;

            JsonMissingTranslationsLogger.EnableLogFor(Loc.Instance);

            try
            {
                Loc.Instance.CurrentLanguage = "it";

                Loc.Tr("LanguageName", "Test").ShouldBe("Test");

                Loc.Instance.MissingTranslations.ShouldContainKey("LanguageName");
                Loc.Instance.MissingTranslations["LanguageName"].ShouldContainKey("it");
                Loc.Instance.MissingTranslations["LanguageName"]["it"].ShouldBe("default text : Test");

                Loc.Instance.CurrentLanguage = "en";

                Loc.Tr("NotExistingTextId", "Test2").ShouldBe("Test2");

                Loc.Instance.MissingTranslations.ShouldContainKey("NotExistingTextId");
                Loc.Instance.MissingTranslations["NotExistingTextId"].ShouldContainKey("en");
                Loc.Instance.MissingTranslations["NotExistingTextId"]["en"].ShouldBe("default text : Test2");

                eventFired.ShouldBe(2);

                File.ReadAllText(missingFilesFileName).ShouldBe(GetEmbeddedResource("MissingTranslationsfileForComparaison.json"));
            }
            finally
            {
                Loc.Instance.MissingTranslationFound -= Instance_MissingTranslationFound;
                JsonMissingTranslationsLogger.DisableLogFor(Loc.Instance);
            }
        }
    }
}