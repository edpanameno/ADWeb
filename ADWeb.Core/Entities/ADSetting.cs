using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    /// <summary>
    /// Holds Active Directory Settings that will be used
    /// throughout the application
    /// </summary>
    public class ADSetting
    {
        public int ADSettingID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
