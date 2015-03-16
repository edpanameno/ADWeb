using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADWeb.ViewModels
{
    using ADWeb.Core.Entities;

    /// <summary>
    /// This class will be used to create a user template object
    /// </summary>
    public class CreateUserTemplateVM
    {
        public CreateUserTemplateVM()
        {
            UserTemplate = new UserTemplate();
            Groups = new List<string>();
        }

        public UserTemplate UserTemplate { get; set; }
        public List<DomainOU> OrganizationalUnits { get; set; }
        public List<string> Groups { get; set; }
    }
}