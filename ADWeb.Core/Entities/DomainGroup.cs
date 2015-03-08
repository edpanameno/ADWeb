using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public class DomainGroup
    {
        public int DomainGroupID { get; set; }
        public bool Enabled { get; set; }
        
        [Display(Name="Group Name")]
        [Required(ErrorMessage="Group Name is required")]
        public string Name { get; set; }
        
        [Display(Name="Distiguished Name")]
        [Required(ErrorMessage="Distinguished Name is required")]
        public string DN { get; set; }

        [ForeignKey("UserTemplate")]
        public int UserTemplateID { get; set; }
        public virtual UserTemplate UserTemplate { get; set; }
    }
}
