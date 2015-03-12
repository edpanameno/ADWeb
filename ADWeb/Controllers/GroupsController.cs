using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADWeb.Controllers
{
    using AutoMapper;
    using ADWeb.ViewModels;
    using ADWeb.Core.ActiveDirectory;
    using ADWeb.Core.Models;

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

        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup(CreateGroupVM groupId)
        {
            if(ModelState.IsValid)
            {
                Group group = Mapper.Map<Group>(groupId);

                ADDomain domain = new ADDomain();
                domain.CreateGroup(group);
                
                TempData["group_created_successfully"] = group.GroupName + " has been created successfully!";
                return RedirectToAction("ViewGroup", new { groupId = group.GroupName });
            }

            return View();
        }
    }
}