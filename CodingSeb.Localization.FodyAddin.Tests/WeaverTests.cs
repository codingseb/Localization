using CodingSeb.Localization.FodyAddin.Fody;
using Fody;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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

            var instance = (dynamic)Activator.CreateInstance(type, true);

            List<string> listOfPropertyNames = propertyNamesField.GetValue(instance) as List<string>;

            Assert.NotNull(listOfPropertyNames);
            Assert.Contains("TestProperty", listOfPropertyNames);

            INotifyPropertyChanged notifyPropertyChanged = instance as INotifyPropertyChanged;

            List<string> propertyNames = new List<string>();

            void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                propertyNames.Add(e.PropertyName);
            }

            notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;

            languageChangedMethod.Invoke(instance, new object[] { Loc.Instance, new CurrentLanguageChangedEventArgs("en", "fr") });

            Assert.Contains("TestProperty", propertyNames);
            Assert.Contains("TextIdInAttribute", propertyNames);

            propertyNames.Clear();

            Assert.Empty(propertyNames);

            Loc.Instance.CurrentLanguage = "es";

            Assert.Contains("TestProperty", propertyNames);
            Assert.Contains("TextIdInAttribute", propertyNames);

            notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged;
        }

        [Fact]
        public void ValidateCustomInstanceLocPropertyAttribute()
        {
            var type = testResult.Assembly.GetType("CodingSeb.Localization.AssemblyToProcess.LocalizedWithFodyAndCustomLocPropertyClass");

            FieldInfo propertyNamesField = type.GetField("__localizedPropertyNames__", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo languageChangedMethod = type.GetMethod("__CurrentLanguageChanged__", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(propertyNamesField);
            Assert.NotNull(languageChangedMethod);

            var instance = (dynamic)Activator.CreateInstance(type, true);

            List<string> listOfPropertyNames = propertyNamesField.GetValue(instance) as List<string>;

            Assert.NotNull(listOfPropertyNames);
            Assert.Contains("TestProperty", listOfPropertyNames);

            INotifyPropertyChanged notifyPropertyChanged = instance as INotifyPropertyChanged;

            List<string> propertyNames = new List<string>();

            void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                propertyNames.Add(e.PropertyName);
            }

            notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;

            Loc customLoc = type.GetProperty("CustomLoc").GetValue(instance) as Loc;

            Assert.NotNull(customLoc);

            languageChangedMethod.Invoke(instance, new object[] { customLoc, new CurrentLanguageChangedEventArgs("en", "fr") });

            Assert.Contains("TestProperty", propertyNames);
            Assert.Contains("TextIdInAttribute", propertyNames);

            propertyNames.Clear();

            Assert.Empty(propertyNames);

            customLoc.CurrentLanguage = "es";

            Assert.Contains("TestProperty", propertyNames);
            Assert.Contains("TextIdInAttribute", propertyNames);

            notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged;
        }

        [Fact]
        public void ValidateCustomInstanceLocFieldAttribute()
        {
            var type = testResult.Assembly.GetType("CodingSeb.Localization.AssemblyToProcess.LocalizedWithFodyAndCustomLocFieldClass");

            FieldInfo propertyNamesField = type.GetField("__localizedPropertyNames__", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo languageChangedMethod = type.GetMethod("__CurrentLanguageChanged__", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(propertyNamesField);
            Assert.NotNull(languageChangedMethod);

            var instance = (dynamic)Activator.CreateInstance(type, true);

            List<string> listOfPropertyNames = propertyNamesField.GetValue(instance) as List<string>;

            Assert.NotNull(listOfPropertyNames);
            Assert.Contains("TestProperty", listOfPropertyNames);

            INotifyPropertyChanged notifyPropertyChanged = instance as INotifyPropertyChanged;

            List<string> propertyNames = new List<string>();

            void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                propertyNames.Add(e.PropertyName);
            }

            notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;

            Loc customLoc = type.GetField("customLoc", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance) as Loc;

            Assert.NotNull(customLoc);

            languageChangedMethod.Invoke(instance, new object[] { customLoc, new CurrentLanguageChangedEventArgs("en", "fr") });

            Assert.Contains("TestProperty", propertyNames);
            Assert.Contains("TextIdInAttribute", propertyNames);

            propertyNames.Clear();

            Assert.Empty(propertyNames);

            customLoc.CurrentLanguage = "es";

            Assert.Contains("TestProperty", propertyNames);
            Assert.Contains("TextIdInAttribute", propertyNames);

            notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged;
        }
    }
}
