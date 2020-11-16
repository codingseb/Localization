using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    /// <summary>
    /// Allow to Concatenate Multiple Tr Results with a string.Format or a separator.
    /// </summary>
    [ContentProperty(nameof(Collection))]
    [ContentWrapper(typeof(Tr))]
    public class MultiTr : MarkupExtension
    {
        public string StringFormat { get; set; }

        public string Separator { get; set; } = " ";

        public Collection<Tr> Collection { get; } = new Collection<Tr>();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
                return this;

            if (!(service.TargetObject is DependencyObject targetObject)
                || !(service.TargetProperty is DependencyProperty targetProperty))
            {
                return this;
            }

            IEnumerable<object> providedValues = Collection.Select(tr => tr.ProvideValue(serviceProvider, true) as BindingBase ?? (object)tr);

            if (providedValues.All(p => p is BindingBase))
            {
                MultiTrData multiTrData = new MultiTrData()
                {
                    StringFormat = StringFormat ?? string.Join(Separator, Enumerable.Range(0, Collection.Count).Select(i => "{" + i.ToString() + "}"))
                };

                MultiBinding multiBinding = new MultiBinding()
                {
                    Converter = new ForMultiTrMarkupInternalStringFormatMultiValuesConverter(),
                    ConverterParameter = multiTrData
                };

                Collection.ToList().ForEach(tr =>
                {
                    BindingBase bindingBase = tr.ProvideValue(serviceProvider, true) as BindingBase;

                    if (bindingBase is MultiBinding trMultiBinding)
                    {
                        trMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                    }
                    else
                    {
                        multiBinding.Bindings.Add(bindingBase);
                    }

                    multiTrData.Bindings.Add(bindingBase);
                });

                BindingOperations.SetBinding(targetObject, targetProperty, multiBinding);

                return multiBinding.ProvideValue(serviceProvider);
            }
            else
            {
                return this;
            }
        }

        protected class ForMultiTrMarkupInternalStringFormatMultiValuesConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                MultiTrData multiTrData = parameter as MultiTrData;
                List<object> stringFormatValues = new List<object>();

                int offset = 0;

                multiTrData.Bindings.ForEach(bindingBase =>
                {
                    if (bindingBase is MultiBinding multiBinding)
                    {
                        stringFormatValues.Add(multiBinding.Converter.Convert(values.Skip(offset).Take(multiBinding.Bindings.Count).ToArray(), null, multiBinding.ConverterParameter, multiBinding.ConverterCulture));
                        offset += multiBinding.Bindings.Count;
                    }
                    else
                    {
                        stringFormatValues.Add(values[offset]);
                        offset++;
                    }
                });

                return string.Format(multiTrData.StringFormat, stringFormatValues.ToArray());
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }

        protected class MultiTrData
        {
            public string StringFormat { get; set; }

            public List<BindingBase> Bindings { get; set; } = new List<BindingBase>();
        }
    }
}
