using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public enum UserUpdateType
    {
        [Display(Name="User Information")]
        UserInfo = 1,
        
        [Display(Name="Added to Group")]
        AddedToGroup,
        
        [Display(Name="Removed from Group")]
        RemovedFromGroup,
        
        [Display(Name="Disabled Account")]
        Disabled,
        
        [Display(Name="Re-enabled Account")]
        ReEnabled,
        
        [Display(Name="Reset Password")]
        ResetPassword,
        
        [Display(Name="Unlocked Account")]
        UnlockedAccount,
        
        [Display(Name="Renamed Username")]
        RenamedUserName,
        
        /// <summary>
        /// This is used to identify that the user was
        /// added to the DomainUsers table because the
        /// user in the domain was not created thru 
        /// the application (i.e. it existed before or
        /// it was created outside the app)
        /// </summary>
        [Display(Name="Created DB Entry")]
        CreatedDBEntry
    }

    public class UserUpdateHistory
    {
        public UserUpdateHistory()
        {
        }

        public int UserUpdateHistoryID { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateUpdated { get; set; }
        public UserUpdateType UpdateType { get; set; }
        public string Notes { get; set; }
       
        [ForeignKey("DomainUser")]
        public string Username { get; set; }
        public virtual DomainUser DomainUser { get; set; }
    }
}
