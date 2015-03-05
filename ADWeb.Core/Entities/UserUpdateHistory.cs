using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public enum UserUpdateType
    {
        UserInfo = 1,
        AddedToGroup,
        RemovedFromGroup,
        Disabled,
        ReEnabled,
        ResetPassword,
        UnlockedAccount,
        RenamedUserName,
        
        /// <summary>
        /// This is used to identify that the user was
        /// added to the DomainUsers table because the
        /// user in the domain was not created thru 
        /// the application (i.e. it existed before or
        /// it was created outside the app)
        /// </summary>
        CreatedDBEntry
    }

    public class UserUpdateHistory
    {
        public UserUpdateHistory()
        {
            DomainUser = new DomainUser();
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
