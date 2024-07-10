﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodingSeb.Localization.WPF
{
    public class TrStringFormatConverter : TrConverterBase, IValueConverter
    {
        public TrStringFormatConverter()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        public TrStringFormatConverter(string textId)
        {
            TextId = textId;
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.AddHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        ~TrStringFormatConverter()
        {
            WeakEventManager<Loc, CurrentLanguageChangedEventArgs>.RemoveHandler(Loc.Instance, nameof(Loc.Instance.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        /// <summary>
        /// The text to return if no text correspond to textId in the current language
        /// </summary>
        public string DefaultText { get; set; }

        /// <summary>
        /// The language id in which to get the translation. To Specify if not CurrentLanguage
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// To provide a prefix to add at the begining of the translated text.
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// To provide a suffix to add at the end of the translated text.
        /// </summary>
        public string Suffix { get; set; } = string.Empty;

        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        [ConstructorArgument("textId")]
        public string TextId { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Prefix + string.Format(string.IsNullOrEmpty(TextId) ? "" : Loc.Tr(TextId, DefaultText?.Replace("[apos]", "'"), LanguageId), value) + Suffix;
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

        private void CurrentLanguageChanged(object sender, CurrentLanguageChangedEventArgs e)
        {
            if (xamlTargetObject != null && xamlDependencyProperty != null)
            {
                if (IsInAMultiBinding)
                    BindingOperations.GetMultiBindingExpression(xamlTargetObject, xamlDependencyProperty)?.UpdateTarget();
                else
                    BindingOperations.GetBindingExpression(xamlTargetObject, xamlDependencyProperty)?.UpdateTarget();
            }
        }
    }
}
