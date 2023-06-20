using System.Diagnostics;
using System.Windows;

namespace CodingSeb.Localization.AssemblyToProcess
{
    [LocPropertyAttribute(nameof(CustomLoc))]
    public class LocalizedWithFodyAndCustomLocPropertyClass : NotifyPropertyChangedBase
    {
        [Localize]
        public string TestProperty => CustomLoc.Translate("TestLabel");

        [Localize(nameof(TextIdInAttribute))]
        public string TextIdInAttribute { get; set; }

        public Loc CustomLoc { get; set; } = new Loc();
    }
}
