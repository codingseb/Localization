using System;

namespace CodingSeb.Localization
{
    /// <summary>
    /// To specify the name of the method that trigger the PropertyChanged event if not a standard one.
    /// The method will be use to triggger PropertyChanged when Current Language changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyChangedTriggerMethodNameForLocalization : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodName">The name of the method that trigger the PropertyChanged event</param>
        public PropertyChangedTriggerMethodNameForLocalization(string methodName)
        {}
    }
}
