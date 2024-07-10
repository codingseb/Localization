using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodingSeb.Localization.WPF.Tests.Models
{
    internal class Person : NotifyPropertyChangedBaseClass
    {
        public string Prefix { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Label { get; set; }

        public int NumberOfHands { get; set; }
    }
}
