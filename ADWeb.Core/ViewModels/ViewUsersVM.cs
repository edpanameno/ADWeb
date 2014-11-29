using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADWeb.Core.ActiveDirectory;

namespace ADWeb.Core.ViewModels
{
    public class ViewUsersVM
    {
        public List<ADUser> RecentlyCreated { get; set; }
        public List<ADUser> RecentlyUpdated { get; set; }
    }
}
