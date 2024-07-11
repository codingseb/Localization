﻿namespace CodingSeb.Localization.Translators
{
    /// <summary>
    /// This translator use the Dictionary loaded by LocalizationLoader and FileLoaders to translate text.
    /// This is the default Translator
    /// </summary>
    public class FilesDictionaryTranslator : ITranslator
    {
        /// <summary>
        /// To ask if this translator can translate the given textId and languageId
        /// </summary>
        /// <param name="textId">the text id of the translation</param>
        /// <param name="languageId">the language Id of the translation</param>
        /// <returns><c>true</c> if it can translate.Otherwise <c>false</c></returns>
        public bool CanTranslate(string textId, string languageId) => languageId != null && Loc.TranslationsDictionary.ContainsKey(textId ?? string.Empty)
                && Loc.TranslationsDictionary[textId].ContainsKey(languageId);

        /// <summary>
        /// Translate the given textId and languageId
        /// </summary>
        /// <param name="textId">the text id to translate</param>
        /// <param name="languageId">the languageId in which to translate</param>
        /// <returns>The text of the translated textId in the languageId, or null if it can't translate.</returns>
        public string Translate(string textId, string languageId) => CanTranslate(textId, languageId) ? Loc.TranslationsDictionary[textId][languageId].TranslatedText : null;
    }
}
