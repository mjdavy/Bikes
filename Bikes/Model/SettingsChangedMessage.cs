using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bikes.Model
{
    public class SettingsChangedMessage
    {
        public SettingsChangedMessage(string setting)
        {
            this.ChangedSetting = setting;
        }

        public string ChangedSetting
        {
            get;
            set;
        }
    }
}
