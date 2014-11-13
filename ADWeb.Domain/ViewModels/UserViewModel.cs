using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ADWeb.Domain.ViewModels
{
    /// <summary>
    /// This class will be used to view and edit the basic information
    /// about a user.
    /// </summary>
    public class UserViewModel
    {
        [Display(Name="Username")]
        public string SamAccountName { get; set; }
        
        [Display(Name="First name")] 
        [Required(ErrorMessage="First name is required.")]
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        
        [Display(Name="Last name")] 
        [Required(ErrorMessage="Last name is required.")]
        public string Surname { get; set; }
        
        [Display(Name="Dislay Name")] 
        [Required(ErrorMessage="Display Name is required.")]
        public string DisplayName{ get; set; }
        
        [Display(Name="Dislay Name")] 
        [Required(ErrorMessage="Display Name is required.")]
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        
        [Display(Name="Company")] 
        [Required(ErrorMessage="Company name is required.")]
        public string Company { get; set; }
        public string Notes { get; set; }

        public DateTime WhenChanged { get; set; }

        /// <summary>
        /// The group(s) that a user belongs to in the domain.
        /// </summary>
        public Dictionary<string, string> UserGroups { get; set; }
    }
}