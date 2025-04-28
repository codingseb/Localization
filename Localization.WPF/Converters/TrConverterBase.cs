using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    /// <summary>
    /// A base object for all markup extension linked to localization
    /// To manage memorization of xamlobject to refresh on language changes
    /// </summary>
    public abstract class TrConverterBase : MarkupExtension
    {
        internal DependencyObject xamlTargetObject;
        internal DependencyProperty xamlDependencyProperty;

        internal bool IsInAMultiBinding;

        /// <summary>
        /// Memorize xaml objects
        /// </summary>
        /// <param name="serviceProvider"></param>
        protected void SetXamlObjects(IServiceProvider serviceProvider)
        {
            try
            {
                var xamlContext = serviceProvider.GetType()
                    .GetRuntimeFields().ToList()
                    .Find(f => f.Name.Equals("_xamlContext"))
                    .GetValue(serviceProvider);

                xamlTargetObject ??= xamlContext?.GetType()
                    .GetProperty("GrandParentInstance")?
                    .GetValue(xamlContext) as DependencyObject;

                var xamlProperty = xamlContext?.GetType()
                    .GetProperty("GrandParentProperty")?
                    .GetValue(xamlContext);

                xamlDependencyProperty ??= xamlProperty?.GetType()
                    .GetProperty("DependencyProperty")?
                    .GetValue(xamlProperty) as DependencyProperty;
            }
            catch { }
        }
    }
}
