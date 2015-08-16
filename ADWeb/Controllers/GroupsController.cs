using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

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
            return View(groups.OrderByDescending(g => g.MemberCount).ToList());
        }

        public ActionResult ViewGroup(string group)
        {
            ADDomain domain = new ADDomain();
            ADGroup groupInfo = domain.GetGroupByName(group);
            ViewGroupVM groupVM = Mapper.Map<ViewGroupVM>(groupInfo);
            groupVM.OldGroupName = groupInfo.GroupName;

            return View(groupVM);
        }

        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup(CreateGroupVM group)
        {
            if(ModelState.IsValid)
            {
                Group newGroup = Mapper.Map<Group>(group);

                ADDomain domain = new ADDomain();
                domain.CreateGroup(newGroup);
                
                TempData["group_created_successfully"] = newGroup.GroupName + " has been created successfully!";
                return RedirectToAction("ViewGroup", new { group = newGroup.GroupName });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateGroup(ViewGroupVM group)
        {
            if(ModelState.IsValid)
            {
                ADGroup groupInfo = Mapper.Map<ADGroup>(group);
                ADDomain domain = new ADDomain();

                if(group.GroupName != group.OldGroupName)
                {
                    // The user has changed the group name 
                    domain.UpdateGroup(groupInfo, group.OldGroupName);
                }
                else
                {
                    domain.UpdateGroup(groupInfo);
                }
                    
                TempData["group_updated_successfully"] = "The group '" + groupInfo.GroupName + "' has been successfully updated!";
                return RedirectToAction("Index", "Groups");
            }
            else
            {
                return View();
            }
        }

        public ActionResult IsGroupnameUnique(string groupName)
        {
            if(!string.IsNullOrEmpty(groupName))
            {
                ADDomain domain = new ADDomain();
                bool isGroupFound = domain.IsGroupnameUnique(groupName);
                return Json(isGroupFound, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This is called when a user trying to update the name of an existing
        /// group. This will verify that the group name that they have typed is
        /// unique to the domain.
        /// </summary>
        /// <param name="newGroupName"></param>
        /// <returns></returns>
        public ActionResult ValidateRenamingGroup(string GroupName, string oldGroupName)
        {
            // If both the GroupName (the current group name) and the old one are the
            // same then the user is not tyring to rename the group when they hit the
            // Update Group button
            if(GroupName == oldGroupName)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            // If we get this far, then it means that the user has typed in 
            // a different name then the old group name. we now have to check
            // and make sure that the name is unique in the domain.
            if(!string.IsNullOrEmpty(GroupName))
            {
                ADDomain domain = new ADDomain();
                bool isGroupFound = domain.IsGroupnameUnique(GroupName);
                return Json(isGroupFound, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}