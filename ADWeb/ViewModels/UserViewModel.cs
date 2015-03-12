using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ADWeb.Core.Entities;

namespace ADWeb.ViewModels
{
    /// <summary>
    /// This class will be used to view and edit the basic information
    /// about a user.
    /// </summary>
    public class UserViewModel
    {
        public UserViewModel()
        {
            DBInfo = new UserDBInfo();
            UserHistory = new List<UserUpdateHistory>();
        }

        [Display(Name="Username")]
        public string SamAccountName { get; set; }

        [Display(Name="First name")] 
        [Required(ErrorMessage="First name is required.")]
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        
        [Display(Name="Initials")]
        [StringLength(2, ErrorMessage="The initial can only be one character long.")]
        public string Initial { get; set; }
        
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
        public DateTime WhenCreated { get; set; }
        public string LogonCount { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// The group(s) that a user belongs to in the domain.
        /// </summary>
        public Dictionary<string, string> UserGroups { get; set; }

        public UserDBInfo DBInfo { get; set; }
        public List<UserUpdateHistory> UserHistory { get; set; }
    }

    /// <summary>
    /// This class will be used to store information about the user
    /// that is stored in the database.
    /// </summary>
    public class UserDBInfo
    {
        public string Createdby { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}