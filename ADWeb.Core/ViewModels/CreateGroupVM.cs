using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.ViewModels
{
    public class CreateGroupVM
    {
        [Display(Name="Group Name")]
        [Required(ErrorMessage="Group Name is required")]
        public string GroupName { get; set; }
        
        [Display(Name="Group Description")]
        public string Description { get; set; }
    }
}
