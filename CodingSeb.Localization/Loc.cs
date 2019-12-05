using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CodingSeb.Localization
{
    /// <summary>
    /// The base class of the localization system
    /// To Translate a text use Loc.Tr()
    /// </summary>
    public class Loc : INotifyPropertyChanged
    {
        private static readonly object lockObject = new object();

        private string currentLanguage = "en";

        /// <summary>
        /// The current language used for displaying texts with Localization
        /// By default if not set manually is equals to "en" (English)
        /// </summary>
        [DoNotNotify]
        public string CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                if (!AvailableLanguages.Contains(value))
                {
                    AvailableLanguages.Add(value);
                }

                if (!currentLanguage.Equals(value))
                {
                    CurrentLanguageChangingEventArgs changingArgs = new CurrentLanguageChangingEventArgs(currentLanguage, value);
                    CurrentLanguageChangedEventArgs changedArgs = new CurrentLanguageChangedEventArgs(currentLanguage, value);

                    CurrentLanguageChanging?.Invoke(this, changingArgs);

                    if (!changingArgs.Cancel)
                    {
                        currentLanguage = value;
                        CurrentLanguageChanged?.Invoke(this, changedArgs);
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Fired when the current language is about to change
        /// Can be canceled
        /// </summary>
        public event EventHandler<CurrentLanguageChangingEventArgs> CurrentLanguageChanging;

        /// <summary>
        /// Fired when the current language has changed.
        /// </summary>
        public event EventHandler<CurrentLanguageChangedEventArgs> CurrentLanguageChanged;

        /// <summary>
        /// The List of all availables languages where at least one translation is present.
        /// </summary>
        public ObservableCollection<string> AvailableLanguages { get; } = new ObservableCollection<string>();

        /// <summary>
        /// The dictionary that contains all available translations
        /// (TranslationsDictionary[TextId][LanguageId])
        /// </summary>
        public SortedDictionary<string, SortedDictionary<string, LocTranslation>> TranslationsDictionary { get; set; } = new SortedDictionary<string, SortedDictionary<string, LocTranslation>>();

        private static Loc instance;

        /// <summary>
        /// The default instance
        /// </summary>
        public static Loc Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new Loc();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Just For Testing purposes. Prefer the static property Instance
        /// </summary>
        public Loc()
        { }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Translate the given textId in current language.
        /// This method is a shortcut to Instance.Translate
        /// </summary>
        /// <param name="textId">The text to translate identifier</param>
        /// <param name="defaultText">The text to return if no text correspond to textId in the current language</param>
        /// <param name="languageId">The language id in which to get the translation. To Specify it if not CurrentLanguage</param>
        /// <returns>The translated text</returns>
        public static string Tr(string textId, string defaultText = null, string languageId = null)
        {
            return Instance.Translate(textId, defaultText, languageId);
        }

        /// <summary>
        /// Translate the given textId in current language.
        /// </summary>
        /// <param name="textId">The text to translate identifier</param>
        /// <param name="defaultText">The text to return if no text correspond to textId in the current language</param>
        /// <param name="languageId">The language id in which to get the translation. To Specify if not CurrentLanguage</param>
        /// <returns>The translated text</returns>
        public string Translate(string textId, string defaultText = null, string languageId = null)
        {
            if (string.IsNullOrEmpty(textId))
            {
                throw new InvalidOperationException("The textId argument cannot be null or empty");
            }

            if (string.IsNullOrEmpty(defaultText))
            {
                defaultText = textId;
            }

            string result = defaultText;

            if (string.IsNullOrEmpty(languageId))
            {
                languageId = CurrentLanguage;
            }

            CheckMissingTranslation(textId, defaultText);

            if (TranslationsDictionary.ContainsKey(textId)
                && TranslationsDictionary[textId].ContainsKey(languageId))
            {
                result = TranslationsDictionary[textId][languageId].TranslatedText;
            }

            return result;
        }

        /// <summary>
        /// For developpers, for developement and/or debug time.
        /// If set to <c>True</c> Log Out textId asked to be translate but missing in the specified languageId.
        /// Fill the MissingTranslations Dictionary
        /// </summary>
        public bool LogOutMissingTranslations { get; set; }

        public SortedDictionary<string, SortedDictionary<string, string>> MissingTranslations { get; } = new SortedDictionary<string, SortedDictionary<string, string>>();

        private void CheckMissingTranslation(string textId, string defaultText)
        {
            if (LogOutMissingTranslations)
            {
                bool needLogUpdate = false;

                AvailableLanguages.ToList().ForEach(languageId =>
                {
                    if (!TranslationsDictionary.ContainsKey(textId)
                        || !TranslationsDictionary[textId].ContainsKey(languageId))
                    {
                        if (!MissingTranslations.ContainsKey(textId))
                        {
                            MissingTranslations.Add(textId, new SortedDictionary<string, string>());
                        }

                        MissingTranslations[textId][languageId] = $"default text : {defaultText}";

                        needLogUpdate = true;
                    }
                });

                if (needLogUpdate)
                    MissingTranslationFound?.Invoke(this, new LocalizationMissingTranslationEventArgs(this, MissingTranslations, textId));
            }
        }

        /// <summary>
        /// For developpers, for developement and/or debug time.
        /// Fired to inform some translation are missing. 
        /// LogOutMissingTranslations must be set to true
        /// </summary>
        public event EventHandler<LocalizationMissingTranslationEventArgs> MissingTranslationFound;
    }
}
