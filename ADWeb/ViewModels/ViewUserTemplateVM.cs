using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADWeb.ViewModels
{
    using ADWeb.Core.Entities;

    /// <summary>
    /// This class will be used to view an individual User Template.
    /// </summary>
    public class ViewUserTemplateVM
    {
        public ViewUserTemplateVM()
        {
            Groups = new List<string>();
        }

        public UserTemplate UserTemplate { get; set; }
        public List<DomainOU> OrganizationalUnits { get; set; }
        public List<string> Groups { get; set; }
    }
}