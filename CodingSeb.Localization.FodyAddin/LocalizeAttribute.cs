using System;

namespace CodingSeb.Localization
{
    /// <summary>
    /// To specify that a property is a localization and need to has a PropertyChanged event when the current language changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalizeAttribute : Attribute
    {
        /// <summary>
        /// To specify that a property is a localization and need to has a PropertyChanged event when the current language changed
        /// </summary>
        public LocalizeAttribute() { }

        /// <summary>
        /// To specify that a property is a localization and need to has a PropertyChanged event when the current language changed
        /// and inject code in the property of the translation
        /// </summary>
        /// <param name="textId">The Text Id for the translation to inject in this property</param>
        public LocalizeAttribute(string textId)
        {}

        /// <summary>
        /// The default TextTo return if the translation does not exist.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
