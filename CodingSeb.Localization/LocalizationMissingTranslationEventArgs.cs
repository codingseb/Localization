using System;
using System.Collections.Generic;

namespace CodingSeb.Localization
{
    public class LocalizationMissingTranslationEventArgs : EventArgs
    {
        public LocalizationMissingTranslationEventArgs(Loc loc, SortedDictionary<string, SortedDictionary<string, string>> missingTranslations, string textId)
        {
            Loc = loc;
            MissingTranslations = missingTranslations;
            TextId = textId;
        }

        public Loc Loc { get; private set; }

        public SortedDictionary<string, SortedDictionary<string, string>> MissingTranslations { get; private set; }

        public string TextId { get; private set; }

        public string LanguageId { get; private set; }
    }
}
