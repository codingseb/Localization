using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CodingSeb.Localization;

namespace CodingSeb.Localization.AssemblyToProcess
{
    public class LocalizedWithFodyClass : NotifyPropertyChangedBase
    {
        [Localize]
        public string TestProperty => Loc.Tr("TestLabel");
    }
}
