using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Configuration;
using System.ComponentModel.DataAnnotations;

namespace ADWeb.Core.ActiveDirectory
{
    using ADWeb.Core.Models;
using ADWeb.Core.Entities;

    /// <summary>
    /// Fields that can be used when searching for users. 
    /// </summary>
    public enum SearchField
    {
        [Display(Name="Display Name")]
        DisplayName = 1,
       
        [Display(Name="First Name")]
        FirstName,
        
        [Display(Name="Last Name")]
        LastName,
        
        [Display(Name="Department")]
        Department,
        
        [Display(Name="Title")]
        Title,
         
        [Display(Name="Email")]
        Mail,
        
        [Display(Name="Company")]
        Company,

        [Display(Name="Username")]
        Username
    }

    /// <summary>
    /// Specifies what advanced search filter to use when 
    /// searching for users in the domain using the MyAdvanedFilters
    /// class.
    /// </summary>
    public enum AdvancedSearchFilter
    {
        DateCreated,
        WhenChanged
    }

    /// <summary>
    /// This class will contain the methods that can be used to create, edit
    /// and retrieve objects from active directory
    /// </summary>
    public class ADDomain
    {
        public string ServerName { get; set; }
        public string ServiceUser { get; set; }
        public string ServicePassword { get; set; }
        public string TempUsers { get; set; }
        public string UPNSuffix { get; set; }
        public string GroupsOU { get; set; }
        public string RootDSE { get; set; }

        public ADDomain()
        {
            ServerName = WebConfigurationManager.AppSettings["server_name"];
            ServiceUser = WebConfigurationManager.AppSettings["service_user"];
            ServicePassword = WebConfigurationManager.AppSettings["service_password"];
            TempUsers = WebConfigurationManager.AppSettings["temp_users"];
            UPNSuffix = WebConfigurationManager.AppSettings["upn_suffix"];
            GroupsOU = WebConfigurationManager.AppSettings["groups_ou"];
        }

        public void CreateUserWithTemplate(User user, UserTemplateSettings userTemplateSettings)
        {
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, userTemplateSettings.DomainOU, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser newUser = new ADUser(context))
                {
                    newUser.SamAccountName = user.Username;
                    newUser.GivenName = user.FirstName;
                    newUser.MiddleName = user.MiddleName;
                    newUser.Surname = user.LastName;
                    newUser.EmailAddress = user.EmailAddress;
                    newUser.PhoneNumber = user.PhoneNumber;
                    newUser.Title = user.Title;
                    newUser.Department = user.Department;
                    newUser.Notes = "Created by ADWeb on " + DateTime.Now.ToString() + ".";
                    newUser.DisplayName = user.LastName + ", " + user.FirstName + " " + user.Initials;
                    newUser.UserPrincipalName = user.Username + UPNSuffix;
                    newUser.Enabled = true;

                    // Settings from the User template
                    newUser.UserCannotChangePassword = userTemplateSettings.UserCannotChangePassword;
                   
                    if(userTemplateSettings.ChangePasswordAtNextLogon)
                    {
                        // This will force the user to change their password
                        // the next time they login
                        newUser.ExpirePasswordNow();
                    }

                    newUser.PasswordNeverExpires = userTemplateSettings.PasswordNeverExpires;

                    if(userTemplateSettings.AccountExpires)
                    {
                        // We have to determine how long until the user's account
                        // will expire in relation to the date that it is being created.
                        DateTime? expirationDate = new DateTime();
                             
                        switch(userTemplateSettings.ExpirationRange)
                        {
                            case UserExpirationRange.Days:
                                expirationDate = DateTime.Now.AddDays(userTemplateSettings.ExpirationValue.Value);
                                break;
                            case UserExpirationRange.Weeks:
                                int totalDays = 7 * userTemplateSettings.ExpirationValue.Value;
                                expirationDate = DateTime.Now.AddDays(totalDays);
                                break;
                            case UserExpirationRange.Months:
                                expirationDate = DateTime.Now.AddMonths(userTemplateSettings.ExpirationValue.Value);
                                break;
                            case UserExpirationRange.Years:
                                expirationDate = DateTime.Now.AddYears(userTemplateSettings.ExpirationValue.Value);
                                break;
                            default:
                                break;
                        }

                        newUser.AccountExpirationDate = expirationDate;
                    }

                    newUser.SetPassword(user.Password);
                    newUser.Save();

                    // Now now have to add the user to the groups associated with the user template.
                    // Note: We are using RootDSE for now because we are looking at the whole domain.
                    // This will need to be changed later on so that only certain OU's will be searched 
                    // for groups
                    using(PrincipalContext groupContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
                    {
                        foreach(var grp in userTemplateSettings.Groups)
                        {
                            using(GroupPrincipal group = GroupPrincipal.FindByIdentity(groupContext, grp))
                            {
                                if(group != null)
                                {
                                    group.Members.Add(newUser);
                                    group.Save();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CreateUser(User user)
        {
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, TempUsers, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser newUser = new ADUser(context))
                {
                    newUser.SamAccountName = user.Username;
                    newUser.GivenName = user.FirstName;
                    newUser.MiddleName = user.MiddleName;
                    newUser.Surname = user.LastName;
                    newUser.EmailAddress = user.EmailAddress;
                    newUser.PhoneNumber = user.PhoneNumber;
                    newUser.Title = user.Title;
                    newUser.Department = user.Department;
                    newUser.Notes = "Created by ADWeb on " + DateTime.Now.ToString();
                    newUser.DisplayName = user.LastName + ", " + user.FirstName + " " + user.Initials;
                    newUser.UserPrincipalName = user.Username + UPNSuffix;
                    newUser.Enabled = true;

                    newUser.SetPassword(user.Password);
                    newUser.Save();
                }
            }
        }

        public void CreateGroup(Group group)
        {
            // By default, all groups will go into the GroupsOU
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, GroupsOU, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(GroupPrincipalEx newGroup = new GroupPrincipalEx(context))
                {
                    newGroup.Name = group.GroupName;
                    newGroup.SamAccountName = group.GroupName;
                    newGroup.Info = group.Description;
                    newGroup.Save();
                }
            }
        }

        public ADUser GetUserByID(string userId)
        {
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                // This object is not being disposed of because it will
                // given me an error message if I do so. I am thinking 
                // that this could cause issues later on so I need to 
                // make sure that the object is disposed of correctly
                // when being used.
                ADUser user = ADUser.FindByIdentity(context, userId);
                return user;
            }
        }

        /// <summary>
        /// This method retrieves the list of users using the specified SearchFilter durin
        /// the last number of days from the date that this is called on.
        /// </summary>
        /// <param name="day">The number of days to go back on and check to see what users
        /// were created during this time frame.</param>
        /// <returns></returns>
        public List<ADUser> GetUsersByCriteria(AdvancedSearchFilter filter, DateTime day)
        {
            List<ADUser> users = new List<ADUser>();
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser userFilter = new ADUser(context))
                {
                    switch(filter)
                    {
                        case AdvancedSearchFilter.DateCreated:
                            userFilter.MyAdvancedFilters.CreatedInTheLastDays(day, MatchType.GreaterThanOrEquals);
                            break;
                        case AdvancedSearchFilter.WhenChanged:
                            userFilter.MyAdvancedFilters.WhenChangedInLastDays(day, MatchType.GreaterThanOrEquals);
                            break;
                        default:
                            break;
                    }

                    using(PrincipalSearcher searcher = new PrincipalSearcher(userFilter))
                    {
                        ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 1000;
                        var searchResults = searcher.FindAll().ToList();

                        foreach(Principal user in searchResults)
                        {
                            ADUser usr = user as ADUser;

                            // We are filtering out users who don't have a first name
                            // Though this has the issue of filtering out accounts
                            if(String.IsNullOrEmpty(usr.GivenName))
                            {
                                continue;
                            }

                            users.Add(usr);
                        }
                    }
                }
            }

            return users.OrderByDescending(u => u.WhenChanged).ToList();
        }

        /// <summary>
        /// Returns a list of groups that the user belongs to.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetCurrentUserGroups(string userId)
        {
            List<string> groups = new List<string>();
            
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser user = ADUser.FindByIdentity(context, userId))
                {
                    foreach(var grp in user.GetAuthorizationGroups())
                    {
                        // Note: We are not doing any filtering on groups 
                        // (i.e. we are not filtering out Domain Users 
                        // and other builtin groups). The reason why we
                        // are not doing this filtering here is because 
                        // we want to get the list of all groups for the
                        // user regardless of where they are so that we can
                        // make sure that we don't try to add the user back 
                        // to this same group. I don't want those nasty error
                        // messages showing up for the users!
                        groups.Add(grp.Name);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// Returns the list of groups that the user belongs to. This method returns
        /// a Dictionary<string,string> where the key is the name of the group and the
        /// value is the description of each group.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetUserGroupsByUserId(string userId)
        {
            Dictionary<string, string> groups = new Dictionary<string, string>();

            // This seems a bit over excessive when we could have used the 
            // existing non-disposed of object. But for now, I am going to 
            // be using this and will create a ticket to come back and 
            // address what I beleive will be an issue with this application
            // (connecting to resources when using an existing one would 
            // suffice as well).
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser user = ADUser.FindByIdentity(context, userId))
                {
                    foreach(var grp in user.GetAuthorizationGroups())
                    {
                        // We don't want to show the Users and Domain Users groups
                        // to the users of the application.
                        if(grp.Name == "Users" || grp.Name == "Domain Users")
                        {
                            continue;
                        }
                        
                        groups.Add(grp.Name, grp.Description);
                    }
                }
            }

            return groups;
        }

        public void UpdateUser(User updatedUser)
        {
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser user = ADUser.FindByIdentity(context, updatedUser.SamAccountName))
                {
                    if(user != null)
                    {
                        user.GivenName = updatedUser.GivenName;
                        user.MiddleName = updatedUser.MiddleName;
                        user.Surname = updatedUser.Surname;
                        user.DisplayName = updatedUser.DisplayName;
                        user.EmailAddress = updatedUser.EmailAddress;
                        user.Title = updatedUser.Title;
                        user.Department = updatedUser.Department;
                        user.PhoneNumber = updatedUser.PhoneNumber;
                        user.Company = updatedUser.Company;
                        user.Notes = updatedUser.Notes;

                        user.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the specified group in the domain. If the oldGroupName parameter
        /// is not null then it means that the name of the group has changed.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="oldGroupName"></param>
        public void UpdateGroup(ADGroup group, string oldGroupName = null)
        {
            using(PrincipalContext groupContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                if(!string.IsNullOrWhiteSpace(oldGroupName))
                {
                    using(GroupPrincipalEx adGroup = GroupPrincipalEx.FindByIdentity(groupContext, oldGroupName))
                    {
                        if(adGroup != null)
                        {
                            // If we have gotten to this section, then it means that the name of the
                            // group has been changed by the user. If so then we'll have to use the 
                            // underlying DirectoryEntry objec to rename the account. Note: the format
                            // for the new name has to start with 'cn=<new_group_name>' or else the 
                            // code would throw an error message.
                            var groupEntry = (DirectoryEntry)adGroup.GetUnderlyingObject();
                            groupEntry.Rename("cn=" + group.GroupName);
                            groupEntry.CommitChanges();

                            // These are just two additioanl properties that also have
                            // to change but we don't have to use the underlying object
                            // to make the change.
                            adGroup.SamAccountName = group.GroupName;
                            adGroup.DisplayName = group.GroupName;
                            
                            // The user may have also changed the description, if so then
                            // let's update this just in case so that nothing is lost.
                            //adGroup.Description = group.Description;
                            adGroup.Info = group.Description;
                            adGroup.Save();
                        }
                    }
                }
                else
                {
                    // Only the description of the group will be changing
                    using(GroupPrincipalEx adGroup = GroupPrincipalEx.FindByIdentity(groupContext, group.GroupName))
                    {
                        if(adGroup != null)
                        {
                            adGroup.Info = group.Description;
                            adGroup.Save();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches users using the display name value of all users in 
        /// the domain.
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public List<ADUser> QuickSearch(string searchString)
        {
            List<ADUser> users = new List<ADUser>();

            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                ADUser userFilter = new ADUser(context)
                {
                    DisplayName = "*" + searchString + "*"
                    /*SamAccountName = "*" + searchString + "*",
                    EmailAddress = "*" + searchString + "*",
                    Description = "*" + searchString + "*",
                    Notes = "*" + searchString + "*"*/
                };

                using(PrincipalSearcher searcher = new PrincipalSearcher(userFilter))
                {
                    ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 1000;
                    var searchResults = searcher.FindAll().ToList();

                    foreach(Principal user in searchResults)
                    {
                        ADUser usr = user as ADUser;
                        users.Add(usr);
                    }
                }
            }

            return users.OrderBy(u => u.Surname).ToList();
        }

        public List<ADUser> SearchUsers(string searchString, SearchField searchField)
        {
            List<ADUser> users = new List<ADUser>();
            
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                ADUser userFilter = new ADUser(context);
                
                switch(searchField)
                {
                    case SearchField.DisplayName:
                        userFilter.DisplayName = "*" + searchString + "*";
                        break;
                    case SearchField.FirstName:
                        userFilter.GivenName = "*" + searchString + "*";
                        break;
                    case SearchField.LastName:
                        userFilter.Surname = "*" + searchString + "*";
                        break;
                    case SearchField.Department:
                        userFilter.Department = "*" + searchString + "*";
                        break;
                    case SearchField.Title:
                        userFilter.Title = "*" + searchString + "*";
                        break;
                    case SearchField.Mail:
                        userFilter.EmailAddress = "*" + searchString + "*";
                        break;
                    case SearchField.Company:
                        userFilter.Company = "*" + searchString + "*";
                        break;
                    case SearchField.Username:
                        userFilter.SamAccountName = "*" + searchString + "*";
                        break;
                    default:
                        break;
                }

                using(PrincipalSearcher searcher = new PrincipalSearcher(userFilter))
                {
                    ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 1000;
                    var searchResults = searcher.FindAll().ToList();

                    foreach(Principal user in searchResults)
                    {
                        ADUser usr = user as ADUser;
                        users.Add(usr);
                    }
                }
            }

            return users.OrderBy( u => u.Surname).ToList();
        }

        public List<string> SearchGroups(string searchString)
        {
            List<string> groups = new List<string>();
            
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                GroupPrincipal groupFilter = new GroupPrincipal(context)
                {
                    Name = "*" + searchString + "*"
                };

                using(PrincipalSearcher groupSearcher = new PrincipalSearcher(groupFilter))
                {
                    ((DirectorySearcher)groupSearcher.GetUnderlyingSearcher()).PageSize = 1000;
                    var searchResults = groupSearcher.FindAll().ToList();

                    foreach(GroupPrincipal group in searchResults)
                    {
                        // We are going to ignore any built in groups that 
                        // we don't want users to be part of such as 'Domain
                        // Users' (user are added to this group automatically),
                        // 'Domain Admins', etc.
                        if(group.Name.Contains("Domain"))
                        {
                            continue;
                        }

                        groups.Add(group.Name);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// Gets all the users in the domain.
        /// </summary>
        /// <returns></returns>
        public List<ADUser> GetAllUsers()
        {
            List<ADUser> users = new List<ADUser>();
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                ADUser userFilter = new ADUser(context);

                using(PrincipalSearcher searcher = new PrincipalSearcher(userFilter))
                {
                    ((DirectorySearcher)searcher.GetUnderlyingSearcher()).PageSize = 1000;
                    var searchResults = searcher.FindAll().ToList();

                    foreach(Principal user in searchResults)
                    {
                        ADUser usr = user as ADUser;
                        users.Add(usr);
                    }
                }
            }

            return users;
        }
    
        /// <summary>
        /// Gets group information and members from the domain
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public ADGroup GetGroupByName(string groupName)
        {
            ADGroup group = new ADGroup();

            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(GroupPrincipalEx adGroup = GroupPrincipalEx.FindByIdentity(context, groupName))
                {
                    group.GroupName = adGroup.Name;
                    group.Description = adGroup.Info;

                    group.Members = new List<ADUserQuickView>();

                    // We use the OfType<T> method to be able to get more information about
                    // the members of this group. This will give us additional information 
                    // about the user account that would not otherwise be available by 
                    // not doing this.
                    var searchResults = adGroup.GetMembers().OfType<UserPrincipal>();
                    
                    foreach(var user in searchResults)
                    {
                        if(!String.IsNullOrEmpty(user.DisplayName))
                        {
                            group.Members.Add(new ADUserQuickView() { UserName = user.SamAccountName, FirstName = user.GivenName, LastName = user.Surname, IsEnabled = user.Enabled } );
                        }
                        else
                        {
                            group.Members.Add(new ADUserQuickView() { UserName = user.SamAccountName, FirstName = user.SamAccountName + " (username)", LastName = user.SamAccountName + "(username)", IsEnabled = user.Enabled });
                        }
                    }

                    return group;
                }
            }
        }

        public ADGroup GetGroupBasicInfo(string groupName)
        {
            ADGroup group = new ADGroup();

            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(GroupPrincipal adGroup = GroupPrincipal.FindByIdentity(context, groupName))
                {
                    group.GroupName = adGroup.Name;
                    group.DN = adGroup.DistinguishedName;
                }
            }

            return group;
        }

        /// <summary>
        /// Gets all of the active groups in the domain
        /// </summary>
        /// <returns></returns>
        public List<ADGroup> GetAllActiveGroups()
        {
            List<ADGroup> groups = new List<ADGroup>();

            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, GroupsOU, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                GroupPrincipal groupFilter = new GroupPrincipal(context);

                using(PrincipalSearcher searcher = new PrincipalSearcher(groupFilter))
                {
                    var results = searcher.FindAll().ToList();

                    foreach(Principal grp in results)
                    {
                        GroupPrincipal group = grp as GroupPrincipal;
                        groups.Add(new ADGroup { GroupName = group.Name, MemberCount = group.Members.Count, DN = group.DistinguishedName, Description = group.Description });
                    }
                }
            }

            return groups.OrderBy(g => g.GroupName).ToList();
        }
    
        public void AddUserToGroups(string userId, List<string> groups)
        {
            using(PrincipalContext userContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                ADUser user = ADUser.FindByIdentity(userContext, userId);

                if(user != null)
                {
                    foreach(var grp in groups)
                    {
                        using(PrincipalContext groupContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
                        {
                            GroupPrincipal group = GroupPrincipal.FindByIdentity(groupContext, grp);
                            
                            if(group != null)
                            {
                                if(!user.IsMemberOf(group))
                                {
                                    group.Members.Add(user);
                                    group.Save();
                                }
                            }
                        }
                    }
                }
            }
        }
    
        /// <summary>
        /// This function is used to validate that the group names exist in the
        /// domain.
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public List<string> ValidateGroups(List<string> groups)
        {
            List<string> validatedGroups = new List<string>();
            
            using(PrincipalContext groupContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                GroupPrincipal group;  
                foreach(var grp in groups)
                {
                    group = GroupPrincipal.FindByIdentity(groupContext, grp);
                    
                    if(group != null)
                    {
                        validatedGroups.Add(group.Name);
                    }
               }
            }
            
            return validatedGroups;
        }

        /// <summary>
        /// Checks to see if the given username is unique in the domain.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsUsernameUnique(string userName)
        {
            using(PrincipalContext userContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                var user = UserPrincipal.FindByIdentity(userContext, userName);
                return (user == null);
            }
        }

        /// <summary>
        /// Checks to see if a give group name is unique in the domain.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool IsGroupnameUnique(string groupName)
        {
            using(PrincipalContext groupContext = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                var group = GroupPrincipal.FindByIdentity(groupContext, groupName);
                return (group == null);
            }
        }
    }
}
