using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADWeb.ViewModels
{
    using ADWeb.Core.Entities;

    /// <summary>
    /// This is used to create and view user templates
    /// </summary>
    public class UserTemplateVM
    {
        public UserTemplateVM()
        {
            Groups = new List<string>();
        }

        [Remote("IsTemplateNameUnique", "Admin", ErrorMessage="Template Name is not unique. Please enter a unique Template name.")]
        public string Name { get; set; }
        public int DomainOUID { get; set; }
        
        public bool ChangePasswordAtNextLogon { get; set; }
        public bool UserCannotChangePassword { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool AccountExpires { get; set; }
        
        public UserExpirationRange? ExpirationRange { get; set; }
        public int? ExpirationValue { get; set; }
        
        [AllowHtml]
        public string Notes { get; set; }
        public List<DomainOU> OrganizationalUnits { get; set; }
        public List<string> Groups { get; set; }
    }
}