using System;

namespace CodingSeb.Localization
{
    /// <summary>
    /// To specify that a property is a localization and need to has a PropertyChanged event when the current language changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalizeAttribute : Attribute
    {
    }
}
