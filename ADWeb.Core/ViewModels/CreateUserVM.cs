using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.ViewModels
{
    /// <summary>
    /// This class is used to create a User in the domain
    /// </summary>
    public class CreateUserVM
    {
        [Display(Name="Username")]
        [Required(ErrorMessage="Username is required")]
        public string Username { get; set; }

        [Display(Name="Password")]
        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name="Confirm Password")]
        [Compare("Password", ErrorMessage="Password and Confirm Password must be the same!")]
        [Required(ErrorMessage="Confirm Password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        
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
        
    }
}
