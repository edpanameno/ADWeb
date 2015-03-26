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
    public class DomainOU
    {
        public int DomainOUID { get; set; }
        
        [Display(Name="Distinguished Name")]
        [Required(ErrorMessage="Distinguished name is required")]
        public string DistinguishedName { get; set; }
        
        [Required(ErrorMessage="Organizational Unit must have a name.")]
        public string Name { get; set; }
        
        [Required]
        public bool Enabled { get; set; }
        
        [Required(ErrorMessage="Notes for this Organizational Unit are Required.")]
        public string Notes { get; set; }
    }
}
