using CodingSeb.Localization.FodyAddin.Fody;
using Fody;
using System;
using Xunit;

namespace CodingSeb.Localization.FodyAddin.Tests
{
    public class WeaverTests
    {
        static TestResult testResult;

        static WeaverTests()
        {
            var weavingTask = new ModuleWeaver();
            testResult = weavingTask.ExecuteTestRun("CodingSeb.Localization.AssemblyToProcess.dll", runPeVerify: false);
        }

        [Fact]
        public void ValidateThatPropertyWithLocalizeAttributeIsUpdatewhenLanguageChanged()
        {
            var type = testResult.Assembly.GetType("CodingSeb.Localization.AssemblyToProcess.LocalizedWithFodyClass");
            var instance = (dynamic)Activator.CreateInstance(type);

            Assert.Equal("Hello World", instance.World());
        }
    }
}
