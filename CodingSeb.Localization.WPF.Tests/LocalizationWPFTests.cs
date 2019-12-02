using CodingSeb.Localization.Loaders;
using NUnit.Framework;
using Shouldly;
using System.Threading;
using System.Windows.Controls;

namespace CodingSeb.Localization.WPF.Tests
{
    [NonParallelizable, Apartment(ApartmentState.STA)]
    public class LocalizationWPFTests
    {
        private LocalizationLoader loader;

        [OneTimeSetUp]
        public void LoadTranslations()
        {
            loader = new LocalizationLoader(Loc.Instance);

            loader.AddTranslation("SayHello", "en", "Hello");
            loader.AddTranslation("SayHello", "fr", "Bonjour");
        }

        [OneTimeTearDown]
        public void ClearDicts()
        {
            loader.ClearAllTranslations();
        }

        [Test]
        public void TrMarkupCurrentLanguageChangedOnTextBlock()
        {
            Loc.Instance.CurrentLanguage = "en";

            Tr tr = new Tr("SayHello");

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("Hello");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Bonjour");
        }

        [Test]
        public void TrMarkupPrefixOnTextBlock()
        {
            Loc.Instance.CurrentLanguage = "en";

            Tr tr = new Tr("SayHello")
            {
                Prefix = "-"
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("-Hello");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("-Bonjour");
        }

        [Test]
        public void TrMarkupSufixOnTextBlock()
        {
            Loc.Instance.CurrentLanguage = "en";

            Tr tr = new Tr("SayHello")
            {
                Suffix = "!"
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("Hello!");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Bonjour!");
        }
    }
}
