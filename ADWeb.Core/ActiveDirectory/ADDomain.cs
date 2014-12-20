using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web.Configuration;

namespace ADWeb.Core.ActiveDirectory
{
    using ADWeb.Core.ViewModels;

    /// <summary>
    /// Fields that can be used when searching for users. 
    /// </summary>
    public enum SearchField
    {
        DisplayName = 1,
        FirstName,
        LastName,
        Department,
        Title,
        Mail
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

        public ADDomain()
        {
            ServerName = WebConfigurationManager.AppSettings["server_name"];
            ServiceUser = WebConfigurationManager.AppSettings["service_user"];
            ServicePassword = WebConfigurationManager.AppSettings["service_password"];
            TempUsers = WebConfigurationManager.AppSettings["temp_users"];
            UPNSuffix = WebConfigurationManager.AppSettings["upn_suffix"];
            GroupsOU = WebConfigurationManager.AppSettings["groups_ou"];
        }

        public void CreateUser(CreateUserVM user)
        {
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, TempUsers, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser newUser = new ADUser(context))
                {
                    newUser.SamAccountName = user.Username;
                    newUser.GivenName = user.FirstName;
                    newUser.Initials = user.Initials;
                    newUser.Surname = user.LastName;
                    newUser.EmailAddress = user.EmailAddress;
                    newUser.PhoneNumber = user.PhoneNumber;
                    newUser.Title = user.Title;
                    newUser.Department = user.Department;
                    newUser.Notes = "created by ADWeb on " + DateTime.Now.ToString();
                    newUser.DisplayName = user.LastName + ", " + user.FirstName + " " + user.Initials;
                    newUser.UserPrincipalName = user.Username + UPNSuffix;
                    newUser.Enabled = true;

                    newUser.SetPassword(user.Password);
                    newUser.Save();
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

        public void UpdateUser(UserViewModel updatedUser)
        {
            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(ADUser user = ADUser.FindByIdentity(context, updatedUser.SamAccountName))
                {
                    if(user != null)
                    {
                        user.GivenName = updatedUser.GivenName;
                        user.Initials = updatedUser.Initial;
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
    
        public ADGroup GetGroupByName(string groupName)
        {
            ADGroup group = new ADGroup();

            using(PrincipalContext context = new PrincipalContext(ContextType.Domain, ServerName, null, ContextOptions.Negotiate, ServiceUser, ServicePassword))
            {
                using(GroupPrincipal adGroup = GroupPrincipal.FindByIdentity(context, groupName))
                {
                    group.GroupName = adGroup.Name;
                    group.Members = new List<ADUserQuickView>();

                    // We use the OfType<T> method to be able to get more information about
                    // the members of this group. This will give us 
                    var searchResults = adGroup.GetMembers().OfType<UserPrincipal>();
                    
                    foreach(var user in searchResults)
                    {
                        if(!String.IsNullOrEmpty(user.DisplayName))
                        {
                            group.Members.Add(new ADUserQuickView() { UserName = user.SamAccountName, FirstName = user.GivenName, LastName = user.Surname } );
                        }
                        else
                        {
                            group.Members.Add(new ADUserQuickView() { UserName = user.SamAccountName, FirstName = user.SamAccountName + " (username)", LastName = user.SamAccountName + "(username)" });
                        }
                    }

                    return group;
                }
            }
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
    }
}
