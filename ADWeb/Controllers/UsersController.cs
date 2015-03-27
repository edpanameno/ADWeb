using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace ADWeb.Controllers
{
    using ADWeb.Core.DAL;
    using ADWeb.Core.Entities;
    using ADWeb.Core.Models;
    using ADWeb.Core.ActiveDirectory;
    using ADWeb.ViewModels;
    using AutoMapper;

    [Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            ADDomain domain = new ADDomain();
            ViewUsersVM users = new ViewUsersVM();
            users.RecentlyCreated = domain.GetUsersByCriteria(AdvancedSearchFilter.DateCreated, DateTime.Now.AddDays(-7)).Take(10).ToList();

            return View(users);
        }

        public ActionResult ViewUser(string userId)
        {
            ADDomain domain = new ADDomain();
            ADUser user = domain.GetUserByID(userId);
            
            if(user != null)
            {
                UserViewModel viewModel = new UserViewModel();
                viewModel.SamAccountName = user.SamAccountName;
                viewModel.GivenName = user.GivenName;
                viewModel.MiddleName = user.MiddleName;
                viewModel.Surname = user.Surname;
                viewModel.DisplayName = user.DisplayName;
                viewModel.EmailAddress = user.EmailAddress;
                viewModel.Title = user.Title;
                viewModel.Department = user.Department;
                viewModel.PhoneNumber = user.PhoneNumber;
                viewModel.Company = user.Company;
                viewModel.Notes = user.Notes;
                viewModel.Enabled = user.Enabled;
                viewModel.ExpirationDate = user.AccountExpirationDate;
                
                // We are not using the WhenCreated field form the DomainUser
                // table in the database because each user object in the domain
                // should have a value for this property.
                viewModel.WhenCreated = user.WhenCreated;
                
                // The WhenChanged property comes straight from Active Directory
                // I may have to come back to this and use data from the database
                // to get the date when an account was last changed. The reason for
                // this is because if a user logins, this value is updated to reflect
                // this and it's not really a change in my mind. Any changes that are
                // done on a user account (thru the application) should be the real 
                // indicators when an account was changed (and also indicate what type
                // of change happened).
                viewModel.WhenChanged = user.WhenChanged.ToLocalTime();
                viewModel.LogonCount = user.LogonCount.ToString();

                using(var db = new ADWebDB())
                {
                    var userDbInfo = db.DomainUsers.Where(u => u.Username == userId).FirstOrDefault();

                    if(userDbInfo != null)
                    {
                        // If this part of the code is reached, then it means that the user
                        // currently being viewed is was created inside of the application and
                        // thus has an entry in the DomainUsers table.
                        var domainUser = domain.GetUserByID(userDbInfo.CreatedBy);
                        viewModel.DBInfo.Createdby = userDbInfo.CreatedBy;
                        viewModel.DBInfo.WhenCreated = userDbInfo.DateCreated;
                        viewModel.UserHistory = userDbInfo.UpdateHistory.OrderByDescending(u => u.DateUpdated).ToList();
                    }
                    else
                    {
                        viewModel.DBInfo.Createdby = "Unknown";
                    }
                }
                
                viewModel.UserGroups = domain.GetUserGroupsByUserId(userId);

                user.Dispose();
                return View(viewModel);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(UserViewModel userId)
        {
            if(ModelState.IsValid)
            {
                ADDomain domain = new ADDomain();
                StringBuilder msg = new StringBuilder();
                bool userInfoUpdate = false;

                msg.Append("<ul class=\"update-details\">");

                // We first need to record what was changed on the user
                // account by comparing what the user currently has on 
                // Active Directory to what is being passed to this method
                ADUser currentUser = domain.GetUserByID(userId.SamAccountName);
                
                // Now we compare and record what has changed! Note:
                // this method will not be used when a user's active
                // directory account is updated. Because changing the
                // username has to be done with caution, a new method
                // just for that action will be created.
                if(!currentUser.GivenName.Equals(userId.GivenName))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>First Name Changed from '" + currentUser.GivenName + "' to '" + userId.GivenName + "'</li>");
                }

                if(!currentUser.Surname.Equals(userId.Surname))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>Last Name Changed from '" + currentUser.Surname + "' to '" + userId.Surname + "'</li>");
                }

                if(!currentUser.MiddleName.Equals(userId.MiddleName))
                {
                    if(!String.IsNullOrEmpty(userId.MiddleName))
                    {
                        userInfoUpdate = true;
                        msg.Append("<li>Middle Name Changed from '" + currentUser.MiddleName + "' to '" + userId.MiddleName + "'</li>");
                    }
                }

                if(!currentUser.DisplayName.Equals(userId.DisplayName))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>Display Name Changed from '" + currentUser.DisplayName + "' to '" + userId.DisplayName + "'</li>");
                }
                
                if(!currentUser.EmailAddress.Equals(userId.EmailAddress))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>Email Address Changed from '" + currentUser.EmailAddress + "' to '" + userId.EmailAddress + "'</li>");
                }
                
                if(!currentUser.PhoneNumber.Equals(userId.PhoneNumber))
                {
                    if(!String.IsNullOrEmpty(userId.PhoneNumber))
                    {
                        userInfoUpdate = true;
                        msg.Append("<li>Phone Number Changed from '" + currentUser.PhoneNumber + "' to '" + userId.PhoneNumber + "'</li>");
                    }
                }
                
                if(!currentUser.Title.Equals(userId.Title))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>Title Changed from '" + currentUser.Title + "' to '" + userId.Title + "'</li>");
                }
                
                if(!currentUser.Company.Equals(userId.Company))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>Company Changed from '" + currentUser.Company + "' to '" + userId.Company + "'</li>");
                }
                
                if(!currentUser.Department.Equals(userId.Department))
                {
                    userInfoUpdate = true;
                    msg.Append("<li>Department Changed from '" + currentUser.Department + "' to '" + userId.Department + "'</li>");
                }
                
                if(!currentUser.Notes.Equals(userId.Notes))
                {
                    if(!String.IsNullOrEmpty(userId.Notes))
                    {
                        userInfoUpdate = true;
                        msg.Append("<li>Notes Changed from '" + currentUser.Notes + "' to '" + userId.Notes + "'</li>");
                    }
                }

                msg.Append("</ul>");

                ADWeb.Core.Models.User user = Mapper.Map<User>(userId);

                domain.UpdateUser(user);

                // There is a possiblity that a user may accidentally hit the update
                // button but nothing has changed in the user's information. If this
                // happens, we don't want anything to be written to the database. The
                // following if condition checks for this scenario.
                if(userInfoUpdate)
                {
                    using(var db = new ADWebDB())
                    {
                        ADUser loggedInUser = domain.GetUserByID(User.Identity.Name);

                        // Before adding a new update history for this user, we first have
                        // to check to see if this account has an entry in the DomainUsers
                        // table. If it doesn't then we'll go ahead and create one. If it does,
                        // then we'll just insert the update history to the table.
                        var userDbInfo = db.DomainUsers.Where(u => u.Username == userId.SamAccountName).FirstOrDefault();
                        
                        if(userDbInfo == null)
                        {
                            // If we have reached this part of the code then it means that
                            // we have come accross a user that was not created thru the 
                            // application and thus has no entry in the DomainUsers table.
                            // We are going to be creating an entry into this table, but 
                            // we are not going to use the currently logged in user who is 
                            // viewing this account as the person that created the account.
                            // The reason I don't want to store the username is because the 
                            // entry was not created by the user, instead it was created by
                            // the system. I am going to be making this a unique value just
                            // in case we need to use this later on for reports.
                            string createdBy = "System Generated";

                            DomainUser newUser = new DomainUser();
                            newUser.CreatedBy = createdBy;
                            newUser.Username = currentUser.SamAccountName;
                            newUser.DateCreated = currentUser.WhenCreated;

                            // Entry that identifies this as a user who we just inserted
                            // an entry to the DomainUsers table for.
                            UserUpdateHistory newUserHistory = new UserUpdateHistory();
                            newUserHistory.UpdatedBy = createdBy;
                            newUserHistory.Username = userId.SamAccountName;
                            newUserHistory.UpdateType = UserUpdateType.CreatedDBEntry;
                            newUserHistory.DateUpdated = DateTime.Now;
                            newUserHistory.Notes = "<ul class=\"update-details\"><li>New User Added to table by the system.</li></ul>";

                            // This is the actual changes that were made for this user
                            // when the update user button was clicked on and submitted for
                            // this request.
                            UserUpdateHistory userChange = new UserUpdateHistory();
                            userChange.UpdatedBy = loggedInUser.GivenName + " " + loggedInUser.Surname;
                            userChange.Username = userId.SamAccountName;
                            userChange.UpdateType = UserUpdateType.UserInfo;

                            // I am adding 10 milli seconds to this value so that when we 
                            // retrieve it from the database this will show up later on
                            // as we order the results from this table based on the
                            // date updated field!
                            userChange.DateUpdated = DateTime.Now.AddMilliseconds(10);
                            userChange.Notes = msg.ToString();
                            
                            db.DomainUsers.Add(newUser);
                            db.UserUpdateHistory.Add(newUserHistory);
                            db.UserUpdateHistory.Add(userChange);
                            db.SaveChanges();
                        }
                        else
                        {
                            UserUpdateHistory userChange = new UserUpdateHistory();
                            userChange.UpdatedBy = loggedInUser.GivenName + " " + loggedInUser.Surname;
                            userChange.Username = userId.SamAccountName;
                            userChange.UpdateType = UserUpdateType.UserInfo;
                            userChange.DateUpdated = DateTime.Now;
                            userChange.Notes = msg.ToString();

                            db.UserUpdateHistory.Add(userChange);
                        }

                        db.SaveChanges();
                    }
                    
                    TempData["user_updated_successfully"] = userId.GivenName + " " + userId.Surname + "'s account has been successfully updated!";
                }
                else
                {
                    TempData["user_updated_successfully"] = "No updates were done for " + userId.GivenName + " " + userId.Surname + "'s account.";
                }

                return RedirectToAction("ViewUser", new { userId = userId.SamAccountName });
            }

            return View();
        }
    
        public ActionResult RenameUser(string userId)
        {
            return View();
        }
    
        public ActionResult CreateUser()
        {
            using(var db = new ADWebDB())
            {
                List<SelectListItem> userTemplates = new List<SelectListItem>();
                
                var templates = db.UserTemplate.Where(u => u.Enabled).ToList();
                foreach(var template in templates)
                {
                    userTemplates.Add(new SelectListItem() { Value = template.UserTemplateID.ToString(), Text = template.Name});
                }
                
                ViewBag.UTList = userTemplates;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(CreateUserVM userId)
        {
            if(ModelState.IsValid)
            {
                using(var db = new ADWebDB())
                {
                    ADWeb.Core.Models.User newUser = Mapper.Map<User>(userId);
                    ADDomain domain = new ADDomain();

                    // Get User Template Settings so that we can use it to create
                    // the user.
                    UserTemplate userTemplate = db.UserTemplate.Find(userId.UserTemplateID);
                    
                    UserTemplateSettings userTemplateSettings = new UserTemplateSettings();
                    userTemplateSettings.ChangePasswordAtNextLogon = userTemplate.ChangePasswordAtNextLogon;
                    userTemplateSettings.UserCannotChangePassword = userTemplate.UserCannotChangePassword;
                    userTemplateSettings.PasswordNeverExpires = userTemplate.PasswordNeverExpires;
                    userTemplateSettings.AccountExpires = userTemplate.AccountExpires;
                    userTemplateSettings.ExpirationRange = userTemplate.ExpirationRange;
                    userTemplateSettings.ExpirationValue = userTemplate.ExpirationValue;
                    userTemplateSettings.DomainOU = userTemplate.DomainOU.DistinguishedName;

                    // When getting the groups associated with a user template, we 
                    // are only interested in getting those groups that are active (i.e.
                    // they have not been removed by the admins of the application). If this is 
                    // not done, then there will be an error if a group happens to have been
                    // added, removed and then added again by one of the administrators. This should
                    // be a rare occurrance, but we have to check just to make sure no errors occur
                    // when creating user accounts.
                    foreach(var group in userTemplate.Groups.Where(u => u.Enabled == true).ToList())
                    {
                        userTemplateSettings.Groups.Add(group.Name);
                    }

                    domain.CreateUserWithTemplate(newUser, userTemplateSettings);
                    ADUser currentUser = domain.GetUserByID(User.Identity.Name);

                    // Insert the account to the Database. Note: we are only
                    // interested in basic information 
                    DomainUser newDomainUser = new DomainUser();
                    newDomainUser.DateCreated = DateTime.Now;
                    newDomainUser.CreatedBy = currentUser.GivenName + " " + currentUser.Surname;
                    newDomainUser.Username = newUser.Username;

                    db.DomainUsers.Add(newDomainUser);
                    db.SaveChanges();
                    
                    TempData["user_created_successfully"] = newUser.FirstName + " " + newUser.LastName + " has been created successfully!";
                    return RedirectToAction("ViewUser", new { userId = userId.Username });
                }

            }
            
            return View();
        }

        public ActionResult RecentlyUpdated()
        {
            ADDomain domain = new ADDomain();
            List<ADUser> users = domain.GetUsersByCriteria(AdvancedSearchFilter.WhenChanged, DateTime.Now.AddDays(-14));
            
            return View(users);
        }

        public ActionResult RecentlyCreated()
        {
            ADDomain domain = new ADDomain();
            List<SelectListItem> days = new List<SelectListItem>();

            for(int i = 1; i <= 31; i++)
            {
                if(i == 14)
                {
                    // We are selecting the default number of days to 
                    // look for users recently created.
                    days.Add(new SelectListItem() { Text = i.ToString() + " days", Value = i.ToString(), Selected = true });
                }
                else
                {
                    days.Add(new SelectListItem() { Text = i.ToString() + " days", Value = i.ToString() });
                }
            }

            ViewBag.days = days;
            List<ADUser> users = domain.GetUsersByCriteria(AdvancedSearchFilter.DateCreated, DateTime.Now.AddDays(-14));
            
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult RefreshRecentlyCreated(int days = 7)
        {
            int formattedDays = days * (-1);
            ADDomain domain = new ADDomain();
            List<ADUser> users = domain.GetUsersByCriteria(AdvancedSearchFilter.DateCreated, DateTime.Now.AddDays(formattedDays)).ToList();

            users.OrderBy(u => u.WhenCreated);
            
            return PartialView("_FilteredUsers", users);
        }
    
        public ActionResult Search()
        {
            SearchUsersModel searchUsers = new SearchUsersModel();
            searchUsers.SearchField = SearchField.DisplayName;
            
            return View(searchUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult SearchUsers(SearchUsersModel search)
        {
            ADDomain domain = new ADDomain();
            List<ADUser> users = domain.SearchUsers(search.SearchValue, search.SearchField);
            ViewBag.SearchTerm = search.SearchValue;

            return PartialView("_UserSearchResults", users);
        }
        
        public ActionResult SearchForGroups(string term)
        {
            ADDomain domain = new ADDomain();
            List<string> groupsFound = domain.SearchGroups(term);

            return Json(groupsFound, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserToGroups(string SamAccountName, List<string> Groups)
        {
            ADDomain domain = new ADDomain();

            // The current implementation of this feature will allow users to type in
            // a few characters of a group name and if any matches are found then those
            // matches are displayed to the user. There is a chance that the user may just
            // type in the name of the group he/she wants to add and not use any of the
            // returned results. For this reason, I am doing an extra check on the domain
            // to make sure that whatever has been entered are valid groups. If a group 
            // name has been entered that is not valid (i.e. it doesn't exist) then that
            // group will not be added to the following list. After generating this list,
            // we are doing an extra check to see if it's empty (which can theoretically 
            // happen) and if so then we just re-direct the user back to the ViewUser page
            // and send a long a message of the issue why no group(s) were added to the user.
            List<string> validatedGroups = domain.ValidateGroups(Groups);

            if(validatedGroups.Count == 0)
            {
                TempData["invalid_groups"] = @"Invalid Group Names. The group(s) you tried to add are not valid group name. 
                                               Please check the name of the group and try again.";
                return RedirectToAction("ViewUser", new { userId = SamAccountName});
            }

            // There is the posibility that a group that the user already belongs
            // to is part of the groups list being passed to this method. I have to 
            // get a list of the current groups that this user belongs to and before
            // adding any of the groups that have been passed to this method, I must
            // make sure that it doesn't already exist. If it does, then the group
            // trying to be added will just be discarded.
            List<string> currentGroups = domain.GetCurrentUserGroups(SamAccountName);
            
            // This will hold the list of groups that will be added to the
            // user account.
            List<string> newGroupsToAdd = new List<string>();

            foreach(var group in validatedGroups)
            {
                if(!currentGroups.Contains(group))
                {
                    newGroupsToAdd.Add(group);
                }
            }

            // If we are adding a group (or list of groups) that the user already
            // belongs to then none of these group should be added.
            if(newGroupsToAdd.Count == 0)
            {
                TempData["no_groups_added"] = "No Groups have been added to this user as the user already is part of the groups submitted.";
                return RedirectToAction("ViewUser", new { userId = SamAccountName});
            }

            // At this time we have filtered out the groups so that only 
            // new groups are added to this user
            domain.AddUserToGroups(SamAccountName, newGroupsToAdd);

            // Now we have to log this action so that it shows up on the
            // change history for this user
            using(var db = new ADWebDB())
            {
                ADUser loggedInUser = domain.GetUserByID(User.Identity.Name);
                
                // The following code generates the update details for this action  
                StringBuilder updateNotes = new StringBuilder();
                updateNotes.Append("<p>The following groups have been added to this user:</p>");
                updateNotes.Append("<ul class=\"update-details\">");
               
                foreach(var group in newGroupsToAdd)
                {
                    updateNotes.Append("<li>" + group + "</li>");
                }

                updateNotes.Append("</ul>");
                    
                UserUpdateHistory newGroupHistory = new UserUpdateHistory();
                newGroupHistory.UpdatedBy = loggedInUser.GivenName + " " + loggedInUser.Surname;
                newGroupHistory.DateUpdated = DateTime.Now;
                newGroupHistory.UpdateType = UserUpdateType.AddedToGroup;
                newGroupHistory.Notes = updateNotes.ToString();
                newGroupHistory.Username = SamAccountName;

                // Before adding this update history entry into the database
                // we have to check for the possibility of the user having no
                // entry in the DomainUsers table.
                DomainUser user = db.DomainUsers.Find(SamAccountName);

                if(user != null)
                {
                    // The user has an existing entry in the DomainUser
                    // table
                    db.UserUpdateHistory.Add(newGroupHistory);
                    db.SaveChanges();
                }
                else
                {
                    DomainUser newDomainUser = new DomainUser();
                    newDomainUser.DateCreated = DateTime.Now;
                    newDomainUser.CreatedBy = loggedInUser.GivenName + " " + loggedInUser.Surname;
                    newDomainUser.Username = SamAccountName;

                    db.DomainUsers.Add(newDomainUser);
                    db.SaveChanges();

                    // Entry that identifies this as a user who we just inserted
                    // an entry to the DomainUsers table for.
                    UserUpdateHistory newUserHistory = new UserUpdateHistory();
                    newUserHistory.UpdatedBy = "System Generated";
                    newUserHistory.Username = SamAccountName;
                    newUserHistory.UpdateType = UserUpdateType.CreatedDBEntry;
                    newUserHistory.DateUpdated = DateTime.Now;
                    newUserHistory.Notes = "<ul class=\"update-details\"><li>New User Added to table by the system.</li></ul>";

                    db.UserUpdateHistory.Add(newUserHistory);
                    db.UserUpdateHistory.Add(newGroupHistory);
                    db.SaveChanges();
                }
                
                TempData["groups_added_successfully"] = "Groups have been added successfully to this user!";
                return RedirectToAction("ViewUser", new { userId = SamAccountName});
            }
        }
    }
}
