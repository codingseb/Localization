﻿using System;
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
        #region Constructors and args Management

        /// <summary>
        /// Default constructor
        /// </summary>
        public MultiTr()
        { }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2)
        {
            ManageArg(new List<object> { arg1, arg2 });
        }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        /// <param name="arg3">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2, object arg3)
        {
            ManageArg(new List<object> { arg1, arg2, arg3 });
        }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        /// <param name="arg3">a textId or a Tr</param>
        /// <param name="arg4">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2, object arg3, object arg4)
        {
            ManageArg(new List<object> { arg1, arg2, arg3, arg4 });
        }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        /// <param name="arg3">a textId or a Tr</param>
        /// <param name="arg4">a textId or a Tr</param>
        /// <param name="arg5">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            ManageArg(new List<object> { arg1, arg2, arg3, arg4, arg5 });
        }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        /// <param name="arg3">a textId or a Tr</param>
        /// <param name="arg4">a textId or a Tr</param>
        /// <param name="arg5">a textId or a Tr</param>
        /// <param name="arg6">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            ManageArg(new List<object> { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        /// <param name="arg3">a textId or a Tr</param>
        /// <param name="arg4">a textId or a Tr</param>
        /// <param name="arg5">a textId or a Tr</param>
        /// <param name="arg6">a textId or a Tr</param>
        /// <param name="arg7">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            ManageArg(new List<object> { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        /// <summary>
        /// Constructor with textIds or textIdbindings given directly
        /// </summary>
        /// <param name="arg1">a textId or a Tr</param>
        /// <param name="arg2">a textId or a Tr</param>
        /// <param name="arg3">a textId or a Tr</param>
        /// <param name="arg4">a textId or a Tr</param>
        /// <param name="arg5">a textId or a Tr</param>
        /// <param name="arg6">a textId or a Tr</param>
        /// <param name="arg7">a textId or a Tr</param>
        /// <param name="arg8">a textId or a Tr</param>
        public MultiTr(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            ManageArg(new List<object> { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        private void ManageArg(List<object> args)
        {
            args.ForEach(arg =>
            {
                if (arg is Tr tr)
                    Collection.Add(tr);
                else
                    Collection.Add(new Tr(arg));
            });
        }

        #endregion

        /// <summary>
        /// To specify the way translations are concatenate
        /// </summary>
        public string StringFormat { get; set; }

        /// <summary>
        /// A separator text to concat between each translations
        /// Used if StringFormat is not set.
        /// </summary>
        public string Separator { get; set; } = " ";

        /// <summary>
        /// A collection of sub translations to concatenate
        /// </summary>
        public Collection<Tr> Collection { get; } = new Collection<Tr>();

        /// <summary>
        /// Converter to apply on the result text
        /// </summary>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// The parameter to pass to the converter
        /// </summary>
        public object ConverterParameter { get; set; }

        /// <summary>
        /// The culture to pass to the converter
        /// </summary>
        public CultureInfo ConverterCulture { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget service)
                return this;

            IEnumerable<object> providedValues = Collection.Select(tr => tr.ProvideValue(serviceProvider, true) as BindingBase ?? (object)tr);

            if (providedValues.All(p => p is BindingBase))
            {
                var internalConverter = new ForMultiTrMarkupInternalStringFormatMultiValuesConverter()
                {
                    StringFormat = StringFormat ?? string.Join(Separator, Enumerable.Range(0, Collection.Count).Select(i => "{" + i.ToString() + "}")),
                    MultiTrConverter = Converter,
                    MultiTrConverterParameter = ConverterParameter,
                    MultiTrConverterCulture = ConverterCulture,
                };

                MultiBinding multiBinding = new()
                {
                    Converter = internalConverter,
                    ConverterCulture = ConverterCulture,
                    ConverterParameter = ConverterParameter,
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

                    internalConverter.StringFormatBindings.Add(bindingBase);
                });

                if (service.TargetObject is not DependencyObject dependencyObject || service.TargetProperty is not DependencyProperty dependencyProperty)
                {
                    return multiBinding;
                }
                else
                {
                    BindingOperations.SetBinding(dependencyObject, dependencyProperty, multiBinding);

                    return multiBinding.ProvideValue(serviceProvider);
                }
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// internal converter to manage multibinding plumbery for MultiTr and String Format stuff
        /// </summary>
        protected class ForMultiTrMarkupInternalStringFormatMultiValuesConverter : IMultiValueConverter
        {
            internal string StringFormat { get; set; }
            internal List<BindingBase> StringFormatBindings { get; } = new List<BindingBase>();
            internal IValueConverter MultiTrConverter { get; set; }
            internal object MultiTrConverterParameter { get; set; }
            internal CultureInfo MultiTrConverterCulture { get; set; }

            /// <inheritdoc/>
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                List<object> stringFormatValues = new();

                int offset = 0;

                StringFormatBindings.ForEach(bindingBase =>
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

                var result = string.Format(StringFormat, stringFormatValues.ToArray());

                return MultiTrConverter == null ? result : MultiTrConverter.Convert(result, null, MultiTrConverterParameter, MultiTrConverterCulture);
            }

            /// <inheritdoc/>
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
    }
}
