using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CodingSeb.Localization.WPF.Tests.Models
{
    internal class Notification : NotifyPropertyChangedBaseClass
    {
        public string TextIdLabel { get; set; }

        public int NumberOfNotification { get; set; }

        public string State { get; set; }
    }
}
