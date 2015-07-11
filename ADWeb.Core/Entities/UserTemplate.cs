using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADWeb.Core.Entities
{
    /// <summary>
    /// This determines how long an account can be active
    /// from the date of creation for the user account.
    /// </summary>
    public enum UserExpirationRange
    {
        Days = 0,
        Weeks,
        Months,
        Years
    }

    public class UserTemplate
    {
        public int UserTemplateID { get; set; }
        public bool Enabled { get; set; }
        
        [Display(Name="Template Name")]
        [Required(ErrorMessage="User Template Name Required")]
        public string Name { get; set; }
        
        public bool ChangePasswordAtNextLogon { get; set; }
        public bool UserCannotChangePassword { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool AccountExpires { get; set; }

        public UserExpirationRange? ExpirationRange { get; set; }
        public int? ExpirationValue { get; set; }
       
        [AllowHtml]
        public string Notes { get; set; }

        [ForeignKey("DomainOU")]
        public int DomainOUID { get; set; }

        /// <summary>
        /// Specifies the OU that the users will be stored in when they
        /// are created with the user template.
        /// </summary>
        public virtual DomainOU DomainOU { get; set; }

        public virtual ICollection<UserTemplateGroup> Groups { get; set; }
    }
}
