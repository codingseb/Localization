using System;
using System.Collections.Generic;
using System.Text;

namespace CodingSeb.Localization.WPF.Tests.Models
{
    internal class User : NotifyPropertyChangedBaseClass
    {
        public string UserName { get; set; }

        public Loc Loc { get; set; } = new Loc();
    }
}
