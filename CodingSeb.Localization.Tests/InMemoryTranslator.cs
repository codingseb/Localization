using CodingSeb.Localization.Translators;

namespace CodingSeb.Localization.Tests
{
    public class InMemoryTranslator : ITranslator
    {
        public bool CanTranslate(string textId, string languageId)
        {
            return textId.StartsWith("->");
        }

        public string Translate(string textId, string languageId)
        {
            return languageId + " : " + textId.Substring(2);
        }
    }
}
