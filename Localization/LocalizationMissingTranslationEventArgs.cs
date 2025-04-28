using System;
using System.Collections.Generic;

namespace CodingSeb.Localization
{
    /// <summary>
    /// The event args that inform that a textId is missing for a language
    /// </summary>
    public class LocalizationMissingTranslationEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="loc">The loc instance where the missing translation has been detected</param>
        /// <param name="missingTranslations">The textId that is missing</param>
        /// <param name="textId">The language in which the textId is missing</param>
        public LocalizationMissingTranslationEventArgs(Loc loc, SortedDictionary<string, SortedDictionary<string, string>> missingTranslations, string textId)
        {
            Loc = loc;
            MissingTranslations = missingTranslations;
            TextId = textId;
        }

        /// <summary>
        /// The loc instance where the missing translation has been detected
        /// </summary>
        public Loc Loc { get; }

        /// <summary>
        /// The dictionnary of all found missing translations
        /// </summary>
        public SortedDictionary<string, SortedDictionary<string, string>> MissingTranslations { get; }

        /// <summary>
        /// The textId that is missing
        /// </summary>
        public string TextId { get; }

        /// <summary>
        /// The language in which the textId is missing
        /// </summary>
        public string LanguageId { get; }
    }
}
