using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADWeb.ViewModels
{
    public class CreateGroupVM
    {
        [Display(Name="Group Name")]
        [Required(ErrorMessage="Group Name is required")]
        [Remote("IsGroupnameUnique", "Groups", ErrorMessage="A group with this name already exists in the domain. Please choose a different group name.")]
        public string GroupName { get; set; }
        
        [Display(Name="Group Description")]
        public string Description { get; set; }
    }
}
