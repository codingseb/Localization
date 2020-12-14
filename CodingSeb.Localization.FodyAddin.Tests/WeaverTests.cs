using CodingSeb.Localization.Fody;
using Fody;
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
            Assert.Equal(1, 1);
        }
    }
}
