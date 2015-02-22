using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADWeb.Controllers
{
    using ADWeb.Core.Concrete;
    using ADWeb.Core.Entities;
    using ADWeb.Core.ViewModels;
    using ADWeb.Core.ActiveDirectory;

    [Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            ADDomain domain = new ADDomain();
            ViewUsersVM users = new ViewUsersVM();
            users.RecentlyUpdated = domain.GetUsersByCriteria(AdvancedSearchFilter.WhenChanged, DateTime.Now.AddDays(-7)).Take(10).ToList();
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
                viewModel.Initial = user.Initials;
                viewModel.Surname = user.Surname;
                viewModel.DisplayName = user.DisplayName;
                viewModel.EmailAddress = user.EmailAddress;
                viewModel.Title = user.Title;
                viewModel.Department = user.Department;
                viewModel.PhoneNumber = user.PhoneNumber;
                viewModel.Company = user.Company;
                viewModel.Notes = user.Notes;
                
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
                    var userDbInfo = db.DomainUsers.Where(u => u.UserName == userId).FirstOrDefault();

                    if(userDbInfo != null)
                    {
                        viewModel.DBInfo.HasDBInfo = true;
                        viewModel.DBInfo.Createdby = userDbInfo.CreatedBy;
                        viewModel.DBInfo.WhenCreated = userDbInfo.DateCreated;
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
                domain.UpdateUser(userId);

                TempData["user_updated_successfully"] = userId.GivenName + " " + userId.Surname + "'s account has been successfully updated!";
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

                // Insert the account to the Database. Note: we are only
                // interested in basic information 
                DomainUser user = new DomainUser();
                user.DateCreated = DateTime.Now;
                user.CreatedBy = User.Identity.Name;
                user.Enabled = true;
                user.UserName = userId.Username;
                user.FirstName = userId.FirstName;
                user.MiddleName = userId.MiddleName;
                user.LastName = userId.LastName;

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
    }
}