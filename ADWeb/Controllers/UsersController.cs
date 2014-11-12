using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADWeb.Domain.ActiveDirectory;

namespace ADWeb.Controllers
{
    using ADWeb.ViewModels;

    [Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
                
                viewModel.UserGroups = domain.GetUserGroupsByUserId(userId);

                user.Dispose();
                return View(viewModel);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(UserViewModel user)
        {
            if(ModelState.IsValid)
            {
                ADDomain domain = new ADDomain();
                ADUser usr = domain.GetUserByID(user.SamAccountName);

                // Update the user with the information that has been entered
                // in the form
                usr.GivenName = user.GivenName;
                usr.MiddleName = user.MiddleName;
                usr.Surname = user.Surname;
                usr.DisplayName = user.DisplayName;
                usr.EmailAddress = user.EmailAddress;
                usr.Title = user.Title;
                usr.Department = user.Department;
                usr.PhoneNumber = user.PhoneNumber;
                usr.Company = user.Company;
                usr.Notes = user.Notes;

                domain.UpdateUser(usr);


                TempData["user_update_successull"] = usr.SamAccountName + " has been successfully updated";
                return RedirectToAction("ViewUser", user.SamAccountName);
            }

            return View();
        }
    }
}