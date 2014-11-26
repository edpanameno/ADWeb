using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.ActiveDirectory
{
    /// <summary>
    /// Used to display user information for groups
    /// </summary>
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
        public List<ADUserQuickView> Members { get; set; }
    }
}
