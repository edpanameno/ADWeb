using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ADWeb.ViewModels
{
    using System.Web.Mvc;
    using ADWeb.Core.ActiveDirectory;

    public class ViewGroupVM
    {
        public ViewGroupVM()
        {
            Members = new List<ADUserQuickView>();
        }

        [Display(Name="Group Name")]
        [Required(ErrorMessage="Group Name is required")]
        [Remote("ValidateRenamingGroup", "Groups", ErrorMessage="This group name is already in use. Please use a different group name.", AdditionalFields="OldGroupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// This is used to determine if the user is trying to change
        /// the name of the group.
        /// </summary>
        public string OldGroupName { get; set; }
        
        [Display(Name="Group Description")]
        public string Description { get; set; }

        /// <summary>
        /// The number of objects that are a member of this
        /// group
        /// </summary>
        public int MemberCount { get; set; }
        public string DN { get; set; }
        public List<ADUserQuickView> Members{ get; set; }
    }
}