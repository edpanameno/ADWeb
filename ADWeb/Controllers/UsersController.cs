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
        public ActionResult UpdateUser(ADUser userId)
        {
            if(ModelState.IsValid)
            {
                ADDomain domain = new ADDomain();
                domain.UpdateUser(userId);

                ADUser user = domain.GetUserByID(userId.SamAccountName);

                TempData["user_update_successull"] = userId + " has been successfully updated";
                return RedirectToAction("ViewUser", user);
            }

            return View();
        }
    }
}