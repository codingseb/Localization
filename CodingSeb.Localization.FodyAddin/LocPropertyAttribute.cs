using System;
using System.Collections.Generic;
using System.Text;

namespace CodingSeb.Localization
{
    /// <summary>
    /// To specify the name of a property defined in the class that has this attribute that return a custom <see cref="Loc"/> instance
    /// for multi-users mode
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LocPropertyAttribute : Attribute
    {
        /// <summary>
        /// To specify the name of a property defined in the class that has this attribute that return a custom <see cref="Loc"/> instance
        /// for multi-users mode
        /// </summary>
        /// <param name="propertyName">the name of the property that get the custom <see cref="Loc"/> instance</param>
        public LocPropertyAttribute(string propertyName)
        {}
    }
}
