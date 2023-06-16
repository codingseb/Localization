using System;
using System.Collections.Generic;
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
            loader = new LocalizationLoader();
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
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.OpenMenuItem.Header", "Notknown", null, "en", ExpectedResult = "Open")]
        [TestCase("JsonTranslations.MainMenu.FileMenuItem.OpenMenuItem.Header", "Notknown", null, "fr", ExpectedResult = "Ouvrir")]
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

            Loc.LogOutMissingTranslations = true;

            void Instance_MissingTranslationFound(object sender, LocalizationMissingTranslationEventArgs e)
            {
                eventFired++;
            }

            Loc.MissingTranslationFound += Instance_MissingTranslationFound;

            JsonMissingTranslationsLogger.MissingTranslationsFileName = missingFilesFileName;

            JsonMissingTranslationsLogger.EnableLog();

            try
            {
                Loc.Instance.CurrentLanguage = "it";

                Loc.Tr("LanguageName", "Test").ShouldBe("Test");

                Loc.MissingTranslations.ShouldContainKey("LanguageName");
                Loc.MissingTranslations["LanguageName"].ShouldContainKey("it");
                Loc.MissingTranslations["LanguageName"]["it"].ShouldBe("default text : Test");

                Loc.Instance.CurrentLanguage = "en";

                Loc.Tr("NotExistingTextId", "Test2").ShouldBe("Test2");

                Loc.MissingTranslations.ShouldContainKey("NotExistingTextId");
                Loc.MissingTranslations["NotExistingTextId"].ShouldContainKey("en");
                Loc.MissingTranslations["NotExistingTextId"]["en"].ShouldBe("default text : Test2");

                eventFired.ShouldBe(2);

                File.ReadAllText(missingFilesFileName).ShouldBe(GetEmbeddedResource("MissingTranslationsfileForComparaison.json"));
            }
            finally
            {
                Loc.MissingTranslationFound -= Instance_MissingTranslationFound;
                JsonMissingTranslationsLogger.DisableLog();
            }
        }

        [Test]
        public void DefaultLanguage()
        {
            Loc.Instance.CurrentLanguage = "es";
            Loc.Instance.FallbackLanguage = "en";
            Loc.Instance.UseFallbackLanguage = true;

            Loc.Tr("SayHello", "HW").ShouldBe("Hello");

            Loc.Instance.FallbackLanguage = null;
        }

        public class FakeModelClass
        {
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string StringProperty { get; set; }

            public FakeModelClass FakeModelProperty { get; set; }
        }

        public static IEnumerable<TestCaseData> GetAdvancedFormatTestSource()
        {

            var fakeModel1 = new FakeModelClass()
            {
                IntProperty = 12,
                DoubleProperty = 3.5,
                StringProperty = "Test",
                FakeModelProperty = new FakeModelClass()
                {
                    IntProperty = 34,
                    DoubleProperty = 3.4,
                    StringProperty = "Fake"
                }
            };

            var fakeModel2 = new FakeModelClass()
            {
                IntProperty = 12,
                DoubleProperty = 3.5,
                StringProperty = "Test",
                FakeModelProperty = null
            };

            yield return new TestCaseData("TenaryTest", true, "Accepted", "en");
            yield return new TestCaseData("TenaryTest", false, "Denied", "en");
            yield return new TestCaseData("TenaryTest", true, "Accepté", "fr");
            yield return new TestCaseData("TenaryTest", false, "Refusé", "fr");
            yield return new TestCaseData("TenaryTest", true, "Accepted", "es");
            yield return new TestCaseData("TenaryTest", false, "Denied", "es");

            yield return new TestCaseData("TenaryTest", null, "Accepted | Denied", "es");

            yield return new TestCaseData("PluralizeTest1", 0, "No customer found", "en");
            yield return new TestCaseData("PluralizeTest1", 1, "One customer found", "en");
            yield return new TestCaseData("PluralizeTest1", 2, "2 customers found", "en");
            yield return new TestCaseData("PluralizeTest1", 142, "142 customers found", "en");

            yield return new TestCaseData("PluralizeTest1", 0, "Aucun client trouvé", "fr");
            yield return new TestCaseData("PluralizeTest1", 1, "Un client trouvé", "fr");
            yield return new TestCaseData("PluralizeTest1", 2, "2 clients trouvés", "fr");
            yield return new TestCaseData("PluralizeTest1", 142, "142 clients trouvés", "fr");

            yield return new TestCaseData("PluralizeTest2", 0, "Alarm", "en");
            yield return new TestCaseData("PluralizeTest2", 1, "Alarm", "en");
            yield return new TestCaseData("PluralizeTest2", 2, "Alarms", "en");
            yield return new TestCaseData("PluralizeTest2", 142, "Alarms", "en");
                                                        
            yield return new TestCaseData("PluralizeTest2", 0, "Alarme", "fr");
            yield return new TestCaseData("PluralizeTest2", 1, "Alarme", "fr");
            yield return new TestCaseData("PluralizeTest2", 2, "Alarmes", "fr");
            yield return new TestCaseData("PluralizeTest2", 142, "Alarmes", "fr");

            yield return new TestCaseData("PluralizeTest2", 0, "Alarm", "es");
            yield return new TestCaseData("PluralizeTest2", 1, "Alarm", "es");
            yield return new TestCaseData("PluralizeTest2", 2, "Alarms", "es");
            yield return new TestCaseData("PluralizeTest2", 142, "Alarms", "es");

            yield return new TestCaseData("InjectionTest1", fakeModel1, "Age 12, Longueur : 3.5, Nom: Test\nAge 34, Longueur : 3.4, Nom: Fake", "fr");
            yield return new TestCaseData("InjectionTest1", fakeModel1, "Age 12, Length : 3.5, Name: Test\nAge 34, Length : 3.4, Name: Fake", "en");

            yield return new TestCaseData("InjectionTest1", null, "Age {m.IntProperty}, Length : {m.DoubleProperty}, Name: {m.StringProperty}\nAge {m.FakeModelProperty.IntProperty}, Length : {m.FakeModelProperty.DoubleProperty}, Name: {m.FakeModelProperty.StringProperty}", "en");
            yield return new TestCaseData("UnknownLabel", fakeModel1, "UnknownLabel", "en");

            yield return new TestCaseData("InjectionTest1", fakeModel2, "Age 12, Length : 3.5, Name: Test\nAge {m.FakeModelProperty.IntProperty}, Length : {m.FakeModelProperty.DoubleProperty}, Name: {m.FakeModelProperty.StringProperty}", "en");

        }

        [Test]
        [TestCaseSource(nameof(GetAdvancedFormatTestSource))]
        public void AdvancedFormatTest(string label, object model, string result, string language)
        {
            Loc.Instance.FallbackLanguage = "en";
            Loc.Instance.UseFallbackLanguage = true;
            var message = Loc.Tr(label, model, languageId: language);
            Console.WriteLine(message);
            message.ShouldBe(result);
            Loc.Instance.UseFallbackLanguage = false;
        }

        [Test]
        public void LocMultiInstance()
        {
            Loc loc1 = new Loc();
            Loc loc2 = new Loc();
            loc1.CurrentLanguage = "fr";
            loc2.CurrentLanguage = "en";

            loc1.CurrentLanguage.ShouldBe("fr");
            loc2.CurrentLanguage.ShouldBe("en");

            loc1.Translate("SayHello").ShouldBe("Bonjour");
            loc2.Translate("SayHello").ShouldBe("Hello");
        }
    }
}