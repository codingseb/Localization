using System;
using System.Windows;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF.Tests
{
    public sealed class TestsServiceProvider : IServiceProvider, IProvideValueTarget
    {
        private readonly DependencyObject targetObject;
        private readonly DependencyProperty targetProperty;

        public TestsServiceProvider(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            this.targetObject = targetObject;
            this.targetProperty = targetProperty;
        }

        object IProvideValueTarget.TargetObject { get { return targetObject; } }
        object IProvideValueTarget.TargetProperty { get { return targetProperty; } }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IProvideValueTarget))
                return this;
            return null;
        }
    }
}
