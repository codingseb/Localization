using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    /// <summary>
    /// Converter to Translate a specific TextId in the Binding LanguageId.
    /// If Translation don't exist return DefaultText.
    /// Not usable in TwoWay Binding mode.
    /// </summary>
    public class TrLanguageIdConverter : TrConverterBase, IValueConverter
    {
        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        [ConstructorArgument("textId")]
        public virtual string TextId { get; set; }

        /// <summary>
        /// The text to return if no text correspond to textId in the current language
        /// </summary>
        public string DefaultText { get; set; }

        /// <summary>
        /// To provide a prefix to add at the begining of the translated text.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// To provide a suffix to add at the end of the translated text.
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Prefix + Loc.Tr(TextId, DefaultText?.Replace("[apos]", "'"), value as string) + Suffix;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                SetXamlObjects(serviceProvider);

                if (string.IsNullOrEmpty(TextId))
                {
                    if (xamlTargetObject != null && xamlDependencyProperty != null)
                    {
                        string context = xamlTargetObject.GetContextByName();
                        string obj = xamlTargetObject.FormatForTextId();
                        string property = xamlDependencyProperty.ToString();

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
            return this;
        }
    }
}
