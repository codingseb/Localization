using Avalonia.Utilities;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodingSeb.Localization.Avalonia
{
    /// <summary>
    /// This class is used as viewModel to bind  to DependencyProperties
    /// Is use by Tr MarkupExtension to dynamically update the translation when current language changed
    /// </summary>
    internal class TrData : INotifyPropertyChanged
    {
        public TrData()
        {
            WeakEventHandlerManager.Subscribe<Loc,CurrentLanguageChangedEventArgs,TrData>(Loc.Instance, nameof(Loc.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        ~TrData()
        {
            WeakEventHandlerManager.Unsubscribe<CurrentLanguageChangedEventArgs, TrData>(Loc.Instance, nameof(Loc.CurrentLanguageChanged), CurrentLanguageChanged);
        }

        /// <summary>
        /// To force the use of a specific identifier
        /// </summary>
        [DoNotNotify]
        public string TextId { get; set; }

        /// <summary>
        /// To Format the Given TextId (useful when binding TextId).
        /// Default value : "{0}"
        /// </summary>
        public string TextIdStringFormat { get; set; } = "{0}";

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
        /// An optional object use as data  that is represented by this translation
        /// (Example used for Enum values translation)
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// When the current Language changed update the binding (Call OnPropertyChanged)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentLanguageChanged(object sender, CurrentLanguageChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TranslatedText));
        }

        public string TranslatedText => Prefix + Loc.Tr(string.Format(TextIdStringFormat, TextId), DefaultText, LanguageId) + Suffix;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
