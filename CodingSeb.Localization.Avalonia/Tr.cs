using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace CodingSeb.Localization.Avalonia
{
    public class Tr : MarkupExtension
    {
        private static readonly WeakDictionary<AvaloniaObject, List<IBinding>> bindingAutoCleanRefs = new();

        #region #region Constructors and args Management

        /// <summary>
        /// Translate the current Property in the current language
        /// The Default TextId is "CurrentNamespace.CurrentClass.CurrentProperty"
        /// </summary>
        public Tr()
        { }

        /// <summary>
        /// Translate the current Property in the current language
        /// The Default TextId is "CurrentNamespace.CurrentClass.CurrentProperty"
        /// </summary>
        /// <param name="textId">To force the use of a specific identifier</param>
        public Tr(object textId)
        {
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
        [AssignBinding]
        public IBinding TextIdBinding { get; set; }

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
        /// The language id in which to get the translation. If not Specify -> CurrentLanguage
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
        public IBinding StringFormatArgBinding { get; set; }

        /// <summary>
        /// A collection of bindings to inject in the translated text with a string.Format
        /// </summary>
        public Collection<IBinding> StringFormatArgsBindings { get; } = new Collection<IBinding>();

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
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget service
                || service.TargetObject is not AvaloniaObject targetObject
                || service.TargetProperty is not AvaloniaProperty targetProperty)
            {
                return this;
            }

            try
            {
                if (string.IsNullOrEmpty(TextId) && TextIdBinding == null)
                {
                    if (serviceProvider is IRootObjectProvider rootObjectProvider)
                    {
                        TextId = $"{rootObjectProvider.RootObject.GetType().Name}";

                        if (rootObjectProvider.RootObject is Control rootControl && !string.IsNullOrEmpty(rootControl.Name))
                            TextId += $"[{rootControl.Name}]";

                        TextId += ".";
                    }

                    TextId = (TextId ?? "") + $"{targetObject.GetType().Name}";

                    if (targetObject is Control targetControl && !string.IsNullOrEmpty(targetControl.Name))
                        TextId += $"[{targetControl.Name}]";

                    TextId += $".{targetProperty.Name}";
                }
            }
            catch (InvalidCastException)
            {
                // For Xaml Design Time
                TextId = Guid.NewGuid().ToString();
            }

            if (IsDynamic)
            {
                TrData trData = new()
                {
                    TextId = TextId,
                    TextIdStringFormat = TextIdStringFormat,
                    DefaultText = DefaultText,
                    LanguageId = LanguageId,
                    Prefix = Prefix,
                    Suffix = Suffix
                };

                Binding binding = new(nameof(TrData.TranslatedText))
                {
                    Source = trData,
                };

                if (StringFormatArgBinding == null && StringFormatArgsBindings.Count == 0 && TextIdBinding == null)
                {
                    if (Converter != null)
                    {
                        binding.Converter = Converter;
                        binding.ConverterParameter = ConverterParameter;
                    }

                    if (InMultiTr)
                    {
                        return binding;
                    }
                    else
                    {
                        KeepRefOfBindingUntilTargetObjectDie(targetObject, binding);

                        targetObject.Bind(targetProperty, binding, targetObject);

                        return trData.TranslatedText;
                    }
                }
                else
                {
                    var internalConverter = new ForTrMarkupInternalStringFormatMultiValuesConverter()
                    {
                        Data = trData,
                        TextIdBindingBase = TextIdBinding,
                        TrConverter = Converter,
                        TrConverterParameter = ConverterParameter,
                        TrConverterCulture = ConverterCulture,
                        StringFormatBindings = StringFormatArgsBindings ?? new Collection<IBinding>()
                    };

                    MultiBinding multiBinding = new()
                    {
                        Converter = internalConverter
                    };

                    if (TextIdBinding != null)
                    {
                        if (TextIdBinding is MultiBinding textIdMultiBinding)
                        {
                            textIdMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
                        }
                        else
                        {
                            multiBinding.Bindings.Add(TextIdBinding);
                        }
                    }

                    multiBinding.Bindings.Add(binding);

                    if (StringFormatArgBinding != null)
                    {
                        internalConverter.StringFormatBindings.Insert(0, StringFormatArgBinding);
                        ManageStringFormatArgs(multiBinding, StringFormatArgBinding);
                    }
                    if (StringFormatArgsBindings.Count > 0)
                    {
                        StringFormatArgsBindings.ToList().ForEach(binding => ManageStringFormatArgs(multiBinding, binding));
                    }

                    if (InMultiTr)
                    {
                        return multiBinding;
                    }
                    else
                    {
                        KeepRefOfBindingUntilTargetObjectDie(targetObject, multiBinding);

                        targetObject.Bind(targetProperty, multiBinding, targetObject);

                        return trData.TranslatedText;
                    }
                }
            }
            else
            {
                object result = Prefix + Loc.Tr(TextId, DefaultText, LanguageId) + Suffix;

                if (Converter != null)
                {
                    result = Converter.Convert(result, targetProperty?.PropertyType, ConverterParameter, ConverterCulture);
                }

                return result;
            }
        }

        private static void KeepRefOfBindingUntilTargetObjectDie(AvaloniaObject targetObject, IBinding binding)
        {
            bindingAutoCleanRefs.ManualShrink();

            if (!bindingAutoCleanRefs.ContainsKey(targetObject))
            {
                bindingAutoCleanRefs.Add(targetObject, new List<IBinding>());
            }

            bindingAutoCleanRefs[targetObject].Add(binding);
        }

        private static void ManageStringFormatArgs(MultiBinding multiBinding, IBinding stringFormatBinding)
        {
            if (stringFormatBinding == null)
                return;

            if (stringFormatBinding is Binding)
            {
                multiBinding.Bindings.Add(stringFormatBinding);
            }
            else if (stringFormatBinding is MultiBinding stringFormatMultiBinding)
            {
                stringFormatMultiBinding.Bindings.ToList().ForEach(multiBinding.Bindings.Add);
            }
        }

        protected class ForTrMarkupInternalStringFormatMultiValuesConverter : IMultiValueConverter
        {
            internal TrData Data { get; set; }
            internal IBinding TextIdBindingBase { get; set; }
            internal IValueConverter TrConverter { get; set; }
            internal object TrConverterParameter { get; set; }
            internal CultureInfo TrConverterCulture { get; set; }
            internal Collection<IBinding> StringFormatBindings { get; set; }

            public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    int offset = 1;

                    if (TextIdBindingBase is MultiBinding textIdMultiBinding)
                    {
                        Data.TextId = textIdMultiBinding.Converter.Convert(values.Take(textIdMultiBinding.Bindings.Count).ToArray(), null, textIdMultiBinding.ConverterParameter, TrConverterCulture).ToString();
                        offset = textIdMultiBinding.Bindings.Count;
                    }
                    else if (TextIdBindingBase is Binding)
                    {
                        if (values.Count > 0)
                            Data.TextId = values[0]?.ToString() ?? string.Empty;
                        else
                            Data.TextId = string.Empty;
                        offset++;
                    }

                    List<object> stringFormatArgs = new();

                    for (int i = 0; i < StringFormatBindings.Count; i++)
                    {
                        if (StringFormatBindings[i] is MultiBinding stringFormatMultiBinding)
                        {
                            int bindingsCount = stringFormatMultiBinding.Bindings.Count;
                            stringFormatArgs.Add(stringFormatMultiBinding.Converter.Convert(values.Skip(offset).Take(bindingsCount).ToArray(), null, stringFormatMultiBinding.ConverterParameter, TrConverterCulture));
                            offset += bindingsCount;
                        }
                        else
                        {
                            if (values.Count > offset)
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

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
    }
}
