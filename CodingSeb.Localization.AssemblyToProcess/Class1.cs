using System;
using CodingSeb.Localization;

namespace CodingSeb.Localization.AssemblyToProcess
{
    public class Class
    {
        [Localize]
        public string TestProperty => Loc.Tr("TestLabel");
    }
}
