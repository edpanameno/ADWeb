using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.ViewModels
{
    using ADWeb.Core.ActiveDirectory;

    public class ViewUsersVM
    {
        public List<ADUser> RecentlyCreated { get; set; }
        public List<ADUser> RecentlyUpdated { get; set; }
    }
}
