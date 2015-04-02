using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADWeb.ViewModels
{
    /// <summary>
    /// This class is used to create a User in the domain
    /// </summary>
    public class CreateUserVM
    {
        [Display(Name="Username")]
        [Remote("IsUsernameUnique", "Users", ErrorMessage="This username is already in use. Please type in a different username for this account.")]
        public string Username { get; set; }

        [MinLength(8, ErrorMessage="The Password must be at least 8 Characters")]
        [Display(Name="Password")]
        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name="First name")] 
        [Required(ErrorMessage="First name is required.")]
        public string FirstName { get; set; }
        
        [Display(Name="Middle Name")] 
        public string MiddleName { get; set; }
        
        [Display(Name="Initials")] 
        [StringLength(2, ErrorMessage="Initials can only be one character long")]
        public string Initials { get; set; }
        
        [Display(Name="Last name")] 
        [Required(ErrorMessage="Last name is required.")]
        public string LastName { get; set; }
        
        [Display(Name="Email Address")] 
        public string EmailAddress { get; set; }
        
        [Display(Name="Phone Number")] 
        public string PhoneNumber { get; set; }
        
        [Display(Name="Title")] 
        public string Title { get; set; }
        
        [Display(Name="Deparment")] 
        public string Department { get; set; }

        public int UserTemplateID { get; set; }
    }
}
