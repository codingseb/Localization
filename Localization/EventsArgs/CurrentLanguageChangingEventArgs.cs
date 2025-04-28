using System.ComponentModel;

namespace CodingSeb.Localization
{
    /// <summary>
    /// The event arg to inform that the language is about to change
    /// and offer to cancel the change
    /// </summary>
    public class CurrentLanguageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oldLanguageId">The Language Id before the change</param>
        /// <param name="newLanguageId">The Language Id after the change</param>
        public CurrentLanguageChangingEventArgs(string oldLanguageId, string newLanguageId)
        {
            OldLanguageId = oldLanguageId;
            NewLanguageId = newLanguageId;
        }

        /// <summary>
        /// The Language Id before the change
        /// </summary>
        public string OldLanguageId { get; }

        /// <summary>
        /// The Language Id after the change
        /// </summary>
        public string NewLanguageId { get; }
    }
}
