using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CodingSeb.Localization.Loaders;
using NUnit.Framework;
using Shouldly;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using CodingSeb.Localization.WPF.Tests.Models;

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

            loader.AddTranslation("SayHelloFirstName", "en", "Hello {0}");
            loader.AddTranslation("SayHelloFirstName", "fr", "Bonjour {0}");

            loader.AddTranslation("SayHi", "en", "Hi");
            loader.AddTranslation("SayHi", "fr", "Salut");

            loader.AddTranslation("Handy", "en", "No arm, no chocolate | Half chocolate | {n} chocolates");
            loader.AddTranslation("Handy", "fr", "Pas de bras, pas de chocolat | Demi-chocolat | {n} chocolats");

            loader.AddTranslation("Format", "en", "Hello {person.Prefix} {person.FirstName} {person.LastName}, owner of {dog.Name}.");
            loader.AddTranslation("Format", "fr", "Bonjour {person.Prefix} {person.FirstName} {person.LastName}, proprietaire de {dog.Name}.");

            loader.AddTranslation("Alarm", "fr", "Aucune alarme {0} | {m} alarme {0} | {m} alarmes {0}");
            loader.AddTranslation("Message", "fr", "Aucun message {0} | {m} message {0} | {m} messages {0}");
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

        [Test]
        public void TrMarkupAdvancedFormat()
        {
            Loc.Instance.CurrentLanguage = "en";

            Tr tr = new Tr("Format")
            {
                Model = new Dictionary<string, object>()
                {
                    {
                        "person", new Person()
                        {
                            Prefix = "Dr.",
                            FirstName = "Gregory",
                            LastName = "House"
                        }
                    },
                    {
                        "dog", new Animal()
                        {
                            Name = "Bojack"
                        }
                    }
                }
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("Hello Dr. Gregory House, owner of Bojack.");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Bonjour Dr. Gregory House, proprietaire de Bojack.");
        }

        [Test]
        public void TrMarkupModelBinding()
        {
            Loc.Instance.CurrentLanguage = "en";

            var model = new Person();

            Tr tr = new Tr("Handy")
            {
                ModelBinding = new Binding(nameof(Person.NumberOfHands)){ Source = model },
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            Loc.Instance.CurrentLanguage = "en";

            model.NumberOfHands = 0;

            textBlock.Text.ShouldBe("No arm, no chocolate");

            model.NumberOfHands = 1;

            textBlock.Text.ShouldBe("Half chocolate");

            model.NumberOfHands = 2;

            textBlock.Text.ShouldBe("2 chocolates");

            model.NumberOfHands = 3;

            textBlock.Text.ShouldBe("3 chocolates");

            Loc.Instance.CurrentLanguage = "fr";

            // "Pas de bras, pas de chocolat | Demi-chocolat | {n} chocolats"

            model.NumberOfHands = 0;

            textBlock.Text.ShouldBe("Pas de bras, pas de chocolat");

            model.NumberOfHands = 1;

            textBlock.Text.ShouldBe("Demi-chocolat");

            model.NumberOfHands = 2;

            textBlock.Text.ShouldBe("2 chocolats");

            model.NumberOfHands = 3;

            textBlock.Text.ShouldBe("3 chocolats");
        }

        [Test]
        public void TrMarkupTextIdBinding()
        {
            Loc.Instance.CurrentLanguage = "en";

            var model = new Person();
            model.Label = "SayHello";

            Tr tr = new Tr()
            {
                TextIdBinding = new Binding(nameof(Person.Label)) { Source = model },
                Suffix = "!"
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("Hello!");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Bonjour!");

            Loc.Instance.CurrentLanguage = "en";

            model.Label = "SayHi";

            textBlock.Text.ShouldBe("Hi!");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Salut!");
        }

        [Test]
        public void TrMarkupStringFormatBinding()
        {
            Loc.Instance.CurrentLanguage = "en";

            var model = new Person
            {
                FirstName = "Jack"
            };

            Tr tr = new Tr("SayHelloFirstName")
            {
                StringFormatArgBinding = new Binding(nameof(Person.FirstName)) { Source = model },
                Suffix = "!"
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("Hello Jack!");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Bonjour Jack!");

            Loc.Instance.CurrentLanguage = "en";

            model.FirstName = "John";

            textBlock.Text.ShouldBe("Hello John!");

            Loc.Instance.CurrentLanguage = "fr";

            textBlock.Text.ShouldBe("Bonjour John!");

            Loc.Instance.CurrentLanguage = "en";
        }

        [Test]
        public void TrMarkupFullBindingTest()
        {
            Loc.Instance.CurrentLanguage = "fr";

            var model = new Notification
            {
                TextIdLabel = "Alarm",
                NumberOfNotification = 0,
                State = "active"
            };

            Tr tr = new Tr
            {
                StringFormatArgBinding = new Binding(nameof(Notification.State)) { Source = model },
                ModelBinding = new Binding(nameof(Notification.NumberOfNotification)) { Source = model },
                TextIdBinding = new Binding(nameof(Notification.TextIdLabel)) { Source = model }
            };

            TextBlock textBlock = new TextBlock();

            tr.ProvideValue(new TestsServiceProvider(textBlock, TextBlock.TextProperty));

            textBlock.Text.ShouldBe("Aucune alarme active");
            Console.WriteLine(textBlock.Text);

            model.NumberOfNotification = 1;

            textBlock.Text.ShouldBe("1 alarme active");
            Console.WriteLine(textBlock.Text);

            model.NumberOfNotification = 2;

            textBlock.Text.ShouldBe("2 alarmes active");
            Console.WriteLine(textBlock.Text);

            model.State = "résolus";

            textBlock.Text.ShouldBe("2 alarmes résolus");
            Console.WriteLine(textBlock.Text);

            model.TextIdLabel = "Message";

            textBlock.Text.ShouldBe("2 messages résolus");
            Console.WriteLine(textBlock.Text);

            model.NumberOfNotification = 0;

            textBlock.Text.ShouldBe("Aucun message résolus");
            Console.WriteLine(textBlock.Text);
        }
    }
}
