using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using static System.Net.Mime.MediaTypeNames;

namespace CodingSeb.Localization.Avalonia
{
    public class Tr
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
            TextId = textId.ToString();
        }

        //public Tr(object textId, BindingBase arg1) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2, arg3 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5, BindingBase arg6) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5, arg6 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5, BindingBase arg6, BindingBase arg7) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        //}

        //public Tr(object textId, BindingBase arg1, BindingBase arg2, BindingBase arg3, BindingBase arg4, BindingBase arg5, BindingBase arg6, BindingBase arg7, BindingBase arg8) : this(textId)
        //{
        //    ManageArg(new List<BindingBase> { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        //}

        //private void ManageArg(List<BindingBase> args)
        //{
        //    args.ForEach(StringFormatArgsBindings.Add);
        //}

        #endregion

        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        [AssignBinding]
        [ConstructorArgument("textId")]
        public object TextId { get; set; }

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
        /// Translation In Xaml
        /// </summary>
        /// <param name="serviceProvider"></param>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget service 
                || service.TargetObject is not AvaloniaObject targetObject
                || service.TargetProperty is not AvaloniaProperty targetProperty)
            {
                return this;
            }

            if(TextId == null)
            {
                if (serviceProvider is IRootObjectProvider rootObjectProvider)
                {
                    TextId = $"{rootObjectProvider.RootObject.GetType().Name}";
                    
                    if(rootObjectProvider.RootObject is Control rootControl && !string.IsNullOrEmpty(rootControl.Name))
                        TextId += $"[{rootControl.Name}]";

                    TextId += ".";
                }

                TextId = (TextId ?? "") + $"{targetObject.GetType().Name}";

                if(targetObject is Control targetControl && !string.IsNullOrEmpty(targetControl.Name))
                        TextId += $"[{targetControl.Name}]";

                TextId += $".{targetProperty.Name}";
            }

            if(IsDynamic)
            {
                TrData trData = new TrData()
                {
                    TextId = TextId.ToString(),
                    TextIdStringFormat = TextIdStringFormat,
                    DefaultText = DefaultText,
                    LanguageId = LanguageId,
                    Prefix = Prefix,
                    Suffix = Suffix
                };

                Binding binding = new Binding(nameof(TrData.TranslatedText))
                {
                    Source = trData
                };
            }
            else
            {
                object result = Prefix + Loc.Tr(TextId.ToString(), DefaultText, LanguageId) + Suffix;

                if(Converter != null)
                {
                    result = Converter.Convert(result, targetProperty?.PropertyType, ConverterParameter, ConverterCulture);
                }

                return result;
            }

            return TextId?.ToString();
        }
    }
}
