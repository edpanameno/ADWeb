using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADWeb.Core.ActiveDirectory;
using ADWeb.Core.ViewModels;

namespace ADWeb.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            ADDomain domain = new ADDomain();
            List<ADUser> users = domain.LastUpdatedUsers(DateTime.Now.AddDays(-7));
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
                viewModel.WhenChanged = user.WhenChanged.ToLocalTime();
                viewModel.WhenCreated = user.WhenCreated.ToLocalTime();
                
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

                TempData["user_updated_successfully"] = userId.GivenName + "'s account has been successfully updated!";
                return RedirectToAction("ViewUser", new { userId = userId.SamAccountName });
            }

            return View();
        }
    
        public ActionResult RenameUser(string userId)
        {
            return View();
        }
    }
}