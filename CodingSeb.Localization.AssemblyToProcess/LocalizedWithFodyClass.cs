using System.Diagnostics;
using System.Windows;

namespace CodingSeb.Localization.AssemblyToProcess
{
    public class LocalizedWithFodyClass : NotifyPropertyChangedBase
    {
        [Localize]
        public string TestProperty => Loc.Tr("TestLabel");
    }
}
