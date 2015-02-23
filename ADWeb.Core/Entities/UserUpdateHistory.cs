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
        ResetPassword,
        UnlockedAccount,
        RenamedUserName
    }

    public class UserUpdateHistory
    {
        public int UserUpdateHistoryID { get; set; }
        public string UserName { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateUpdated { get; set; }
        public UserUpdateType UpdateType { get; set; }
        public string UpdateHistory { get; set; }
    }
}
