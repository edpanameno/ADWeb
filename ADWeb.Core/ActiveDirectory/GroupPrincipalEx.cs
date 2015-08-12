using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.ActiveDirectory
{
    /// <summary>
    /// Custom GroupPrincipal object that will be used to interact with
    /// and modify groups in the active directory domain
    /// </summary>
    [DirectoryObjectClass("group")]
    [DirectoryRdnPrefix("CN")]
    public class GroupPrincipalEx : GroupPrincipal
    {
        public GroupPrincipalEx(PrincipalContext context) : base(context) { }
        
        /// <summary>
        /// This is the property that shows up as Notes under a group and the one
        /// property that we actually want to put some notes in so that it's 
        /// easier for users to know what this group does (and or what it's used
        /// for).
        /// </summary>
        [DirectoryProperty("info")]
        public string Info 
        {
            get
            {
                if(ExtensionGet("info").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("info")[0];
                }
            }
            set
            {
                ExtensionSet("info", value);
            }
        }

        public static new GroupPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (GroupPrincipalEx)FindByIdentityWithType(context, typeof(GroupPrincipalEx), identityValue);
        }

        public static new GroupPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (GroupPrincipalEx)FindByIdentityWithType(context, typeof(GroupPrincipalEx), identityType, identityValue);
        }
    }
}
