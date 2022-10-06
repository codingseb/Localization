using System;
using System.Windows;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF.Tests
{
    public sealed class TestsServiceProvider : IServiceProvider, IProvideValueTarget
    {
        public TestsServiceProvider(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            TargetObject = targetObject;
            TargetProperty = targetProperty;
        }

        public object TargetObject { get; }
        public object TargetProperty { get; }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IProvideValueTarget))
                return this;
            return null;
        }
    }
}
