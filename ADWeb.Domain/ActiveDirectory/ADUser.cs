using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace ADWeb.Domain.ActiveDirectory
{
    [DirectoryObjectClass("user")]
    [DirectoryRdnPrefix("CN")]
    public class ADUser : UserPrincipal
    {
        public List<string> UserGroups { get; set; }

        public ADUser(PrincipalContext context) : base(context) {}
        [DirectoryProperty("company")]
        public string Company
        {
            get
            {
                if(ExtensionGet("company").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("company")[0];
                }
            }

            set
            {
                ExtensionSet("company", value);
            }
        }

        [DirectoryProperty("department")]
        public string Department
        {
            get
            {
                if(ExtensionGet("department").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("department")[0];
                }
            }

            set
            {
                ExtensionSet("department", value);
            }
        }

        [DirectoryProperty("title")]
        public string Title
        {
            get
            {
                if(ExtensionGet("title").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("title")[0];
                }
            }

            set
            {
                ExtensionSet("title", value);
            }
        }

        [DirectoryProperty("telephoneNumber")]
        public string PhoneNumber
        {
            get
            {
                if(ExtensionGet("telephoneNumber").Length != 1)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)ExtensionGet("telephoneNumber")[0];
                }
            }

            set
            {
                ExtensionSet("telephoneNumber", value);
            }
        }

        [DirectoryProperty("info")]
        public string Notes
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

        /// <summary>
        /// This method is used to get the list of groups that this
        /// UserPrincipal object belongs to. We need to call this 
        /// method so that we can store the list of groups in a 
        /// List<string> object because the PrincipalContext gets
        /// disposed of otherwise and we cannot grab this d
        /// </summary>
        public void GetUserGroups()
        {
            UserGroups = new List<string>();

            foreach(var group in GetAuthorizationGroups())
            {
                UserGroups.Add(group.Name);
            }
        }

        // The new keyword here hides the static method FindByIdentity of 
        // the UserPrincipal class.
        public static new ADUser FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (ADUser)FindByIdentityWithType(context, typeof(ADUser), identityValue);
        }

        public static new ADUser FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (ADUser)FindByIdentityWithType(context, typeof(ADUser), identityType, identityValue);
        }
    }
}
