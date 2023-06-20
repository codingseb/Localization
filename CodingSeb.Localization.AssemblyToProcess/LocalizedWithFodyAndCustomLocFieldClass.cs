using System.Diagnostics;
using System.Windows;

namespace CodingSeb.Localization.AssemblyToProcess
{
    [LocField(nameof(customLoc))]
    public class LocalizedWithFodyAndCustomLocFieldClass : NotifyPropertyChangedBase
    {
        [Localize]
        public string TestProperty => customLoc.Translate("TestLabel");

        [Localize(nameof(TextIdInAttribute))]
        public string TextIdInAttribute { get; set; }

        private Loc customLoc = new Loc();
    }
}
