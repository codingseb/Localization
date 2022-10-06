using System;
using System.Collections.Generic;
using System.Text;
using CodingSeb.Localization.Formatters;
using NUnit.Framework;
using Shouldly;

namespace CodingSeb.Localization.Tests
{
    public class FormatterTest
    {
        public InjectionFormatter InjectionFormatter { get; set; }
        public PluralizationFormatter PluralizationFormatter { get; set; }
        public TernaryFormatter TernaryFormatter { get; set; }

        public class FakeModelClass
        {
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string StringProperty { get; set; }

            public FakeModelClass FakeModelProperty { get; set; }
        }

        public static IEnumerable<TestCaseData> CreateInjectionFormatterTestCase()
        {
            var fakeObject1 = new FakeModelClass()
            {
                IntProperty = 1,
                DoubleProperty = 2.0,
                StringProperty = "QWERTZERTY",
                FakeModelProperty = new FakeModelClass()
                {
                    IntProperty = 1234,
                    DoubleProperty = Math.E,
                    StringProperty = "La fleur en bouquet fane et jamais ne renait."
                }
            };

            yield return new TestCaseData("m = {m}", 1, "m = 1");
            yield return new TestCaseData("m = {m}", 2, "m = 2");
            yield return new TestCaseData("m = {0}", 2, "m = {0}");
            yield return new TestCaseData("m = {m.IntProperty}", fakeObject1, "m = 1");
            yield return new TestCaseData("m = {m.DoubleProperty}", fakeObject1, "m = 2");
            yield return new TestCaseData("m = {m.StringProperty}", fakeObject1, "m = QWERTZERTY");
            yield return new TestCaseData("m = {m.FakeModelProperty.IntProperty}", fakeObject1, "m = 1234");

            yield return new TestCaseData("m = {m.StringProperty}, m2 = {m.DoubleProperty}", fakeObject1,
                "m = QWERTZERTY, m2 = 2");

            yield return new TestCaseData("Tool = {tool}, Event = {event}", new Dictionary<string,object>()
                {
                    {"tool" , 1},
                    {"event", "CreateTool"}
                },
                "Tool = 1, Event = CreateTool");

            yield return new TestCaseData("Tool = {tool.IntProperty}, Event = {event}", new Dictionary<string, object>()
                {
                    {"tool" , fakeObject1},
                    {"event", "CreateTool"}
                },
                "Tool = 1, Event = CreateTool");

            yield return new TestCaseData("Tool = {tool.FakeModelProperty.IntProperty}, Event = {event}", new Dictionary<string, object>()
                {
                    {"tool" , fakeObject1},
                    {"event", "CreateTool"}
                },
                "Tool = 1234, Event = CreateTool");
        }

        [OneTimeSetUp]
        public void CreateFormatters()
        {
            InjectionFormatter = new InjectionFormatter();
            PluralizationFormatter = new PluralizationFormatter();
            TernaryFormatter = new TernaryFormatter();
        }

        [Test]
        [TestCaseSource(nameof(CreateInjectionFormatterTestCase))]
        public void InjectFormatterTest(string format, object model, string result)
        {
            InjectionFormatter.Format(format, model).ShouldBe(result);
        }

        [Test]
        [TestCase("ZO | More", 123, "More")]
        [TestCase("ZO | More", 1, "ZO")]
        [TestCase("ZO | More", 0, "ZO")]
        [TestCase("ZO | More", 2, "More")]
        [TestCase("QWERTZ", 1, "QWERTZ")]
        [TestCase("QWERTZ", 0, "QWERTZ")]
        [TestCase("QWERTZ", 2, "QWERTZ")]
        [TestCase("No reason to live | One reason to live | {m} reasons to live", 0, "No reason to live")]
        [TestCase("No reason to live | One reason to live | {m} reasons to live", 1, "One reason to live")]
        [TestCase("No reason to live | One reason to live | {m} reasons to live", 2, "{m} reasons to live")]
        [TestCase("No reason to live | One reason to live | {m} reasons to live", 45, "{m} reasons to live")]
        public void PluralizationFormatterTests(string format, object model, string result)
        {
            PluralizationFormatter.Format(format, model).ShouldBe(result);
        }

        [Test]
        [TestCase("Vrai | False", true, "Vrai")]
        [TestCase("Vrai | False", false, "False")]
        [TestCase("Valide | Invalide", false, "Invalide")]
        [TestCase("Valide | Invalide", true, "Valide")]
        [TestCase("Fertig | Unfertig", false, "Unfertig")]
        [TestCase("Fertig | Unfertig", true, "Fertig")]
        public void TenaryFormatterTests(string format, object model, string result)
        {
            TernaryFormatter.Format(format, model).ShouldBe(result);
        }
    }
}
