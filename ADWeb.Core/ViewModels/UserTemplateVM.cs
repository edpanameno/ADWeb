using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.ViewModels
{
    using ADWeb.Core.Entities;

    public class UserTemplateVM
    {
        public UserTemplate UserTemplate { get; set; }
        public List<DomainOU> OrganizationalUnits { get; set; }
        public List<DomainGroup> DomainGroups { get; set; }

        public List<UserTemplate> ActiveUserTemplates { get; set; }
        public List<UserTemplate> DisabledUserTemplates { get; set; }
    }
}
