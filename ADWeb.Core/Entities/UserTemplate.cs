using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public class UserTemplate
    {
        public int UserTemplateID { get; set; }
        public bool Enabled { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

        [Display(Name="Template Name")]
        [Required(ErrorMessage="User Template Name Required")]
        public string Name { get; set; }
        
        public bool? ChangePasswordAtNextLogon { get; set; }
        public bool? UserCannotChangePassword { get; set; }
        public bool? PasswordNeverExpires { get; set; }
        public bool? AccountExpires { get; set; }
        public string Notes { get; set; }

        [ForeignKey("DomainOU")]
        public int DomainOUID { get; set; }

        /// <summary>
        /// Specifies the OU that the users will be stored in when they
        /// are created with the user template.
        /// </summary>
        public virtual DomainOU DomainOU { get; set; }

        public virtual ICollection<DomainGroup> Groups { get; set; }
    }
}
