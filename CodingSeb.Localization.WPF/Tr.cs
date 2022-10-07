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
        #region #region Constructors and args Management

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

        public Tr(object textId, BindingBase arg1) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2, arg3 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5, BindingBase arg6) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5, BindingBase arg6, BindingBase arg7) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5, BindingBase arg6, BindingBase arg7, BindingBase arg8) : this(textId)
        {
            ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        private void ManageArg(List<BindingBase> args)
        {
            args.ForEach(StringFormatArgsBindings.Add);
        }

        #endregion

        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        public string TextId { get; set; }

        /// <summary>
		/// To Provide a TextId by binding
        /// </summary>
        public BindingBase TextIdBinding { get; set; }

        /// <summary>
        /// To Format the Given TextId (useful when binding TextId).
        /// Default value : "{0}"
        /// </summary>
        public string TextIdStringFormat { get; set; } = "{0}";

        private string defaultText;
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
        /// To Specify a DefaultText by binding
        /// </summary>
        public BindingBase DefaultTextBinding { get; set; }

        /// <summary>
        /// The language id in which to get the translation. To Specify if not CurrentLanguage
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// An model object to format string
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// A Binding for the model object.
        /// </summary>
        public BindingBase ModelBinding { get; set; }

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
        public BindingBase StringFormatArgBinding { get; set; }

        /// <summary>
        /// A collection of bindings to inject in the translated text with a string.Format
        /// </summary>
        public Collection<BindingBase> StringFormatArgsBindings { get; } = new Collection<BindingBase>();

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

            TrData trData = new TrData()
            {
                TextId = TextId,
                TextIdStringFormat = TextIdStringFormat,
                DefaultText = DefaultText,
                LanguageId = LanguageId,
                Prefix = Prefix,
                Suffix = Suffix,
                Model = Model,
            };

            if (IsDynamic)
            {

                Binding binding = new(nameof(TrData.TranslatedText))
                {
                    Source = trData
                };

                SetDependanciesInTrConverterBase(Converter, targetObject, targetProperty);

                if (StringFormatArgBinding == null && StringFormatArgsBindings.Count == 0 && TextIdBinding == null && ModelBinding == null && DefaultTextBinding == null)
                {
                    if (Converter != null)
                    {
                        binding.Converter = Converter;
                        binding.ConverterParameter = ConverterParameter;
                        binding.ConverterCulture = ConverterCulture;
                    }

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
                    var internalConverter = new ForTrMarkupInternalStringFormatMultiValuesConverter()
                    {
                        Data = trData,
                        TextIdBindingBase = TextIdBinding,
                        ModelBinding = ModelBinding,
                        DefaultTextBinding = DefaultTextBinding,
                        TrConverter = Converter,
                        TrConverterParameter = ConverterParameter,
                        TrConverterCulture = ConverterCulture,
                        StringFormatBindings = StringFormatArgsBindings ?? new Collection<BindingBase>()
                    };

                    MultiBinding multiBinding = new()
                    {
                        Converter = internalConverter
                    };

                    if (TextIdBinding != null)
                    {
                        SetDependanciesInTrConverterBase(TextIdBinding, targetObject, targetProperty);

                        if (TextIdBinding is MultiBinding textIdMultiBinding)
                        {
                            textIdMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                        }
                        else
                        {
                            multiBinding.Bindings.Add(TextIdBinding);
                        }
                    }

                    if (ModelBinding != null)
                    {
                        SetDependanciesInTrConverterBase(ModelBinding, targetObject, targetProperty);

                        if (ModelBinding is MultiBinding modelMultiBinding)
                        {
                            modelMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                        }
                        else
                        {
                            multiBinding.Bindings.Add(ModelBinding);
                        }
                    }

                    if (DefaultTextBinding != null)
                    {
                        SetDependanciesInTrConverterBase(DefaultTextBinding, targetObject, targetProperty);

                        if (DefaultTextBinding is MultiBinding defaultTextMultiBinding)
                        {
                            defaultTextMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                        }
                        else
                        {
                            multiBinding.Bindings.Add(DefaultTextBinding);
                        }
                    }

                    multiBinding.Bindings.Add(binding);

                    if (StringFormatArgBinding != null)
                    {
                        internalConverter.StringFormatBindings.Insert(0, StringFormatArgBinding);
                        ManageStringFormatArgs(multiBinding, StringFormatArgBinding, targetObject, targetProperty);
                    }
                    if (StringFormatArgsBindings.Count > 0)
                    {
                        StringFormatArgsBindings.ToList().ForEach(binding => ManageStringFormatArgs(multiBinding, binding, targetObject, targetProperty));
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
                object result = trData.TranslatedText;

                if(Converter != null)
                {
                    result = Converter.Convert(result, targetProperty?.PropertyType, ConverterParameter, ConverterCulture);
                }

                return result;
            }
        }

        private void ManageStringFormatArgs(MultiBinding multiBinding, BindingBase stringFormatBinding, DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            if (stringFormatBinding == null)
                return;

            SetDependanciesInTrConverterBase(stringFormatBinding, dependencyObject, dependencyProperty);

            if (stringFormatBinding is Binding)
            {
                multiBinding.Bindings.Add(stringFormatBinding);
            }
            else if(stringFormatBinding is MultiBinding stringFormatMultiBinding)
            {
                stringFormatMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
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
            internal TrData Data { get; set; }
            internal BindingBase TextIdBindingBase { get; set; }
            internal IValueConverter TrConverter { get; set; }
            internal object TrConverterParameter { get; set; }
            internal CultureInfo TrConverterCulture { get; set; }
            internal Collection<BindingBase> StringFormatBindings { get; set; }
            internal BindingBase ModelBinding { get; set; }
            internal BindingBase DefaultTextBinding { get; set; }

            /// <inheritdoc/>
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    int offset = 0;

                    if (TextIdBindingBase is MultiBinding textIdMultiBinding)
                    {
                        Data.TextId = textIdMultiBinding.Converter.Convert(values.Take(textIdMultiBinding.Bindings.Count).ToArray(), null, textIdMultiBinding.ConverterParameter, textIdMultiBinding.ConverterCulture).ToString();
                        offset += textIdMultiBinding.Bindings.Count;
                    }
                    else if (TextIdBindingBase is Binding)
                    {
                        if (values.Length > offset)
                            Data.TextId = values[offset]?.ToString() ?? string.Empty;
                        else
                            Data.TextId = string.Empty;
                        offset++;
                    }

                    if (ModelBinding is MultiBinding modelBinding)
                    {
                        Data.Model = modelBinding.Converter.Convert(values.Skip(offset).Take(modelBinding.Bindings.Count).ToArray(), null, modelBinding.ConverterParameter, modelBinding.ConverterCulture).ToString();
                        offset += modelBinding.Bindings.Count;
                    }
                    else if (ModelBinding is Binding)
                    {
                        if (values.Length > offset)
                            Data.Model = values[offset];
                        else
                            Data.Model = null;
                        offset++;
                    }

                    if (DefaultTextBinding is MultiBinding defaultTextMultiBinding)
                    {
                        Data.DefaultText = defaultTextMultiBinding.Converter.Convert(values.Skip(offset).Take(defaultTextMultiBinding.Bindings.Count).ToArray(), null, defaultTextMultiBinding.ConverterParameter, defaultTextMultiBinding.ConverterCulture).ToString();
                        offset += defaultTextMultiBinding.Bindings.Count;
                    }
                    else if (DefaultTextBinding is Binding)
                    {
                        if (values.Length > offset)
                            Data.DefaultText = values[offset]?.ToString() ?? string.Empty;
                        else
                            Data.DefaultText = null;
                        offset++;
                    }

                    offset++;

                    List<object> stringFormatArgs = new List<object>();

                    for (int i = 0; i < StringFormatBindings.Count; i++)
                    {
                        if (StringFormatBindings[i] is MultiBinding stringFormatMultiBinding)
                        {
                            int bindingsCount = stringFormatMultiBinding.Bindings.Count;
                            stringFormatArgs.Add(stringFormatMultiBinding.Converter.Convert(values.Skip(offset).Take(bindingsCount).ToArray(), null, stringFormatMultiBinding.ConverterParameter, stringFormatMultiBinding.ConverterCulture));
                            offset += bindingsCount;
                        }
                        else
                        {
                            if (values.Length > offset)
                                stringFormatArgs.Add(values[offset]);
                            offset++;
                        }
                    }

                    var translated = string.Format(Data.TranslatedText, stringFormatArgs.ToArray());

                    return TrConverter == null ? translated : TrConverter.Convert(translated, null, TrConverterParameter, TrConverterCulture);
                }
                catch
                {
                    return string.Empty;
                }
            }

            /// <inheritdoc/>
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
    }
}
