using System;
using CodingSeb.Localization;

namespace CodingSeb.Localization.AssemblyToProcess
{
    public class LocalizedWithFodyClass
    {
        [Localize]
        public string TestProperty => Loc.Tr("TestLabel");
    }
}
