using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADWeb.Domain.ActiveDirectory;

namespace ADWeb.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            ADDomain domain = new ADDomain();
            List<ADUser> users = domain.GetAllUsers();

            return View(users);
        }

        public ActionResult ViewUser(string userId)
        {
            ADDomain domain = new ADDomain();
            ADUser user = domain.GetUserByID(userId);
            return View(user);
        }
    }
}