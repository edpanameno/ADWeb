using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADWeb.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}