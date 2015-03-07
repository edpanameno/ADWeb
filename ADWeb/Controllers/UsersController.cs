using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADWeb.Controllers
{
    using ADWeb.Core.DAL;
    using ADWeb.Core.Entities;
    using ADWeb.Core.ViewModels;
    using ADWeb.Core.ActiveDirectory;
    using System.Text;

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
                domain.UpdateUser(userId);

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(CreateUserVM userId)
        {
            if(ModelState.IsValid)
            {
                ADDomain domain = new ADDomain();
                domain.CreateUser(userId);
                ADUser domainUser = domain.GetUserByID(User.Identity.Name);

                // Insert the account to the Database. Note: we are only
                // interested in basic information 
                DomainUser user = new DomainUser();
                user.DateCreated = DateTime.Now;
                user.CreatedBy = domainUser.GivenName + " " + domainUser.Surname;
                user.Username = userId.Username;

                using(var db = new ADWebDB())
                {
                    db.DomainUsers.Add(user);
                    db.SaveChanges();
                }

                TempData["user_created_successfully"] = userId.FirstName + " " + userId.LastName + " has been created successfully!";
                return RedirectToAction("ViewUser", new { userId = userId.Username });
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
    }
}