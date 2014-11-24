using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.ActiveDirectory
{
    public class ADUserQuickView
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
    }

    /// <summary>
    /// This class represents a group principal in the domain.
    /// </summary>
    public class ADGroup 
    {
        public string GroupName { get; set; }
        public Dictionary<string, string> Members { get; set; }
    }
}
