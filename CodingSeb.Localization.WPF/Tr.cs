using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    /// <summary>
    /// Translate With Localization
    /// </summary>
    public class Tr : MarkupExtension
    {
        private string defaultText;

        /// <summary>
        /// Translate the current Property in the current language
        /// The Default TextId is "CurrentNamespace.CurrentClass.CurrentProperty"
        /// </summary>
        public Tr()
        {}

        /// <summary>
        /// Translate the current Property in the current language
        /// The Default TextId is "CurrentNamespace.CurrentClass.CurrentProperty"
        /// </summary>
        /// <param name="textId">To force the use of a specific identifier</param>
        public Tr(object textId)
        {
			if (textId is BindingBase textIdBinding)
                TextIdBinding = textIdBinding;
            else
                TextId = textId.ToString();
        }

        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        public virtual string TextId { get; set; }

        /// <summary>
		/// To Provide a TextId by binding
        /// </summary>
        public BindingBase TextIdBinding { get; set; }

        /// <summary>
        /// The text to return if no text correspond to textId in the current language
        /// </summary>
        public string DefaultText
        {
            get { return defaultText; }
            set
            {
                defaultText = value?.Replace("[apos]", "'");
            }
        }

        /// <summary>
        /// The language id in which to get the translation. To Specify if not CurrentLanguage
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// If set to true, The text will automatically be update when Current Language Change. (use Binding)
        /// If not the property must be updated manually (use single string value).
        /// By default is set to true.
        /// </summary>
        public bool IsDynamic { get; set; } = true;

        /// <summary>
        /// To provide a prefix to add at the begining of the translated text.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// To provide a suffix to add at the end of the translated text.
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// Converter to apply on the translated text
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

        /// <summary>
        /// A Simple binding to inject in the translated text with a string.Format
        /// </summary>
        public BindingBase StringFormatBinding { get; set; }

        /// <summary>
        /// A collection of bindings to inject in the translated text with a string.Format
        /// </summary>
        public Collection<BindingBase> StringFormatBindings { get; } = new Collection<BindingBase>();

        /// <summary>
        /// Translation In Xaml
        /// </summary>
        /// <param name="serviceProvider"></param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider, false);
        }

        /// <summary>
        /// Translation In Xaml
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="InMultiTr"></param>
        /// <returns></returns>
        public object ProvideValue(IServiceProvider serviceProvider, bool InMultiTr)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
                return this;

            DependencyProperty targetProperty = service.TargetProperty as DependencyProperty;
            DependencyObject targetObject = service.TargetObject as DependencyObject;

            if ((targetObject == null || targetProperty == null) && IsDynamic)
            {
                return this;
            }

            try
            {
                if (string.IsNullOrEmpty(TextId) && TextIdBinding == null)
                {
                    if (targetObject != null && targetProperty != null)
                    {
                        string context = targetObject.GetContextByName();
                        string obj = targetObject.FormatForTextId();
                        string property = targetProperty.ToString();

                        TextId = $"{context}.{obj}.{property}";
                    }
                    else if (!string.IsNullOrEmpty(DefaultText))
                    {
                        TextId = DefaultText;
                    }
                }
            }
            catch (InvalidCastException)
            {
                // For Xaml Design Time
                TextId = Guid.NewGuid().ToString();
            }

            if (IsDynamic)
            {
                TrData trData = new TrData()
                {
                    TextId = TextId,
                    DefaultText = DefaultText,
                    LanguageId = LanguageId,
                    Prefix = Prefix,
                    Suffix = Suffix
                };

                Binding binding = new Binding(nameof(TrData.TranslatedText))
                {
                    Source = trData
                };

                if (Converter != null)
                {
                    SetDependanciesInTrConverterBase(Converter, targetObject, targetProperty);
                    binding.Converter = Converter;
                    binding.ConverterParameter = ConverterParameter;
                    binding.ConverterCulture = ConverterCulture;
                }

                if (StringFormatBinding == null && StringFormatBindings == null && TextIdBinding == null)
                {
                    if (InMultiTr)
                    {
                        return binding;
                    }
                    else
                    {
                        BindingOperations.SetBinding(targetObject, targetProperty, binding);

                        return binding.ProvideValue(serviceProvider);
                    }
                }
                else
                {
                    MultiBinding multiBinding = new MultiBinding
                    {
                        Converter = new ForTrMarkupInternalStringFormatMultiValuesConverter()
                    };

                    if (TextIdBinding != null)
                    {
                        SetDependanciesInTrConverterBase(TextIdBinding, targetObject, targetProperty);

                        if (TextIdBinding is MultiBinding textIdMultiBinding)
                        {
                            textIdMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                            trData.MultiBinding = textIdMultiBinding;
                        }
                        else
                        {
                            multiBinding.Bindings.Add(TextIdBinding);
                        }

                        multiBinding.ConverterParameter = trData;
                    }

                    multiBinding.Bindings.Add(binding);

                    if (StringFormatBinding != null)
                    {
                        SetDependanciesInTrConverterBase(StringFormatBinding, targetObject, targetProperty);
                        multiBinding.Bindings.Add(StringFormatBinding);
                    }
                    if (StringFormatBindings != null)
                    {
                        StringFormatBindings.ToList().ForEach(binding =>
                        {
                            SetDependanciesInTrConverterBase(binding, targetObject, targetProperty);
                            multiBinding.Bindings.Add(binding);
                        });
                    }

                    if (InMultiTr)
                    {
                        return multiBinding;
                    }
                    else
                    {
                        BindingOperations.SetBinding(targetObject, targetProperty, multiBinding);

                        return multiBinding.ProvideValue(serviceProvider);
                    }
                }
            }
            else
            {
                object result = Prefix + Loc.Tr(TextId, DefaultText, LanguageId) + Suffix;

                if(Converter != null)
                {
                    result = Converter.Convert(result, targetProperty?.PropertyType, ConverterParameter, ConverterCulture);
                }

                return result;
            }
        }

        private void SetDependanciesInTrConverterBase(object converterContainer, DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            if(converterContainer is Binding binding)
            {
                converterContainer = binding.Converter;
            }
            else if(converterContainer is MultiBinding multiBinding)
            {
                converterContainer = multiBinding.Converter;
            }

            if(converterContainer is TrConverterBase trConverterBase)
            {
                trConverterBase.xamlTargetObject = dependencyObject;
                trConverterBase.xamlDependencyProperty = dependencyProperty;
                trConverterBase.IsInAMultiBinding = true;
            }
        }

        protected class ForTrMarkupInternalStringFormatMultiValuesConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (parameter is TrData trData)
                {
                    int offset = 1;

                    if (trData.MultiBinding != null)
                    {
                        trData.TextId = trData.MultiBinding.Converter.Convert(values.Take(trData.MultiBinding.Bindings.Count).ToArray(), null, trData.MultiBinding.ConverterParameter, trData.MultiBinding.ConverterCulture).ToString();
                        offset = trData.MultiBinding.Bindings.Count;
                    }
                    else
                    {
                        trData.TextId = values[0] as string ?? string.Empty;
                    }

                    return string.Format(trData.TranslatedText, values.Skip(1 + offset).ToArray());
                }
                else
                {
                    return string.Format((string)values[0], values.Skip(1).ToArray());
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
    }
}
