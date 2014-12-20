using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ADWeb.Core.ActiveDirectory;

namespace ADWeb.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        public ActionResult Index()
        {
            ADDomain domain = new ADDomain();
            List<ADGroup> groups = domain.GetAllActiveGroups();

            return View(groups);
        }

        public ActionResult ViewGroup(string groupId)
        {
            ADDomain domain = new ADDomain();
            ADGroup group = domain.GetGroupByName(groupId);

            return View(group);
        }
    }
}