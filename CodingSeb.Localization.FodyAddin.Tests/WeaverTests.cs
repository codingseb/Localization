using CodingSeb.Localization.FodyAddin.Fody;
using Fody;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace CodingSeb.Localization.FodyAddin.Tests
{
    public class WeaverTests
    {
        private static readonly TestResult testResult;

        static WeaverTests()
        {
            var weavingTask = new ModuleWeaver();
            testResult = weavingTask.ExecuteTestRun("CodingSeb.Localization.AssemblyToProcess.dll", runPeVerify: false);
        }

        [Fact]
        public void ValidateThatPropertyWithLocalizeAttributeIsUpdateWhenLanguageChanged()
        {
            var type = testResult.Assembly.GetType("CodingSeb.Localization.AssemblyToProcess.LocalizedWithFodyClass");

            FieldInfo propertyNamesField = type.GetField("__localizedPropertyNames__", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo languageChangedMethod = type.GetMethod("__CurrentLanguageChanged__", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(propertyNamesField);
            Assert.NotNull(languageChangedMethod);

            var instance = (dynamic)Activator.CreateInstance(type);

            List<string> listOfPropertyNames = propertyNamesField.GetValue(instance) as List<string>;

            Assert.NotNull(listOfPropertyNames);
            Assert.Contains("TestProperty", listOfPropertyNames);

            INotifyPropertyChanged notifyPropertyChanged = instance as INotifyPropertyChanged;

            string propertyName = string.Empty;

            void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                propertyName = e.PropertyName;
            }

            notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;

            languageChangedMethod.Invoke(instance, new object[] { Loc.Instance, new CurrentLanguageChangedEventArgs("en", "fr") });
            Assert.Equal("TestProperty", propertyName);

            propertyName = string.Empty;

            notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged;

        }
    }
}
