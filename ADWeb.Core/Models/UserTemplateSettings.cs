using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Models
{
    using ADWeb.Core.Entities;

    public class UserTemplateSettings
    {
        public UserTemplateSettings()
        {
            Groups = new List<string>();
        }

        public bool ChangePasswordAtNextLogon { get; set; }
        public bool UserCannotChangePassword { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool AccountExpires { get; set; }

        public UserExpirationRange? ExpirationRange { get; set; }
        public int? ExpirationValue { get; set; }

        public string DomainOU { get; set; }
        public List<string> Groups { get; set; }
    }
}
