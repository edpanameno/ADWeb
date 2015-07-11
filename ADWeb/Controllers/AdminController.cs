using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ADWeb.Controllers
{
    using ADWeb.Core.DAL;
    using ADWeb.Core.Entities;
    using ADWeb.Core.ActiveDirectory;
    using ADWeb.ViewModels;
    using ADWeb.Core.Models;
    using AutoMapper;

    [Authorize]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OU()
        {
            using(var db = new ADWebDB())
            {
                OUViewModel ouVM = new OUViewModel();
                ouVM.ActiveOUs = new List<DomainOU>();
                ouVM.DisabledOUs = new List<DomainOU>();

                ouVM.ActiveOUs = db.DomainOU.Where(ou => ou.Enabled).ToList();
                ouVM.DisabledOUs = db.DomainOU.Where(ou => ou.Enabled == false).ToList();

                return View(ouVM);
            }
        }

        public ActionResult ViewOU(string id)
        {
            using(var db = new ADWebDB())
            {
                var organizationalUnit = db.DomainOU.Where(o => o.Name == id).FirstOrDefault();

                if(organizationalUnit != null)
                {
                    List<SelectListItem> enabledList = new List<SelectListItem>();
                    enabledList.Add(new SelectListItem { Text = "Enabled", Value = "true" });
                    enabledList.Add(new SelectListItem { Text = "Disabled", Value = "false" });

                    ViewBag.EnabledList = enabledList;
                    return View(organizationalUnit);
                }
                else
                {
                    // If the user has clicked on (or typed in) an invalid OU
                    // we are going to re-direct them to the OU page and let them
                    // know that this was not a valid OU.
                    TempData["invalid_ou"] = "The OU " + id + " is not valid.";
                    return RedirectToAction("OU");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOU(DomainOU id)
        {
            if(ModelState.IsValid)
            {
                using(var db = new ADWebDB())
                {
                    db.Entry(id).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["ou_updated"] = "The OU " + id.Name + " has been successfully updated!";
                    return RedirectToAction("OU");
                }
            }
            else
            {
                ModelState.AddModelError("", "Unable to update the Organizational Unit.");
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOU(OUViewModel id)
        {
            if(ModelState.IsValid)
            {
                using(var db = new ADWebDB())
                {
                    id.NewOU.Enabled = true;

                    db.DomainOU.Add(id.NewOU);
                    db.SaveChanges();

                    TempData["ou_created"] = "The Organizationl Unit " + id.NewOU.Name + " has been created successfully!";
                    return RedirectToAction("OU");
                }
            }
            else
            {
                ModelState.AddModelError("", "Unable to create Organizational Unit.");
            }
            
            return View("OU");
        }
    
        public ActionResult UserTemplates()
        {
            using(var db = new ADWebDB())
            {
                UserTemplateVM templateVM = new UserTemplateVM();
                templateVM.ActiveUserTemplates = db.UserTemplate.Where(u => u.Enabled).ToList();
                templateVM.DisabledUserTemplates = db.UserTemplate.Where(u => u.Enabled == false).ToList(); 
                
                return View(templateVM);
            }
        }

        public ActionResult CreateUserTemplate()
        {
            using(var db = new ADWebDB())
            {
                CreateUserTemplateVM userTemplateVM = new CreateUserTemplateVM();
                userTemplateVM.OrganizationalUnits = db.DomainOU.Where(o => o.Enabled == true).ToList();
                userTemplateVM.ExpirationRange = UserExpirationRange.Days;

                List<SelectListItem> ouItems = new List<SelectListItem>();
                foreach(var ou in userTemplateVM.OrganizationalUnits)
                {
                    ouItems.Add(new SelectListItem { Text = ou.Name, Value = ou.DomainOUID.ToString() });
                }

                ViewBag.OUList = ouItems;

                return View(userTemplateVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUserTemplate(CreateUserTemplateVM id)
        {
            if(ModelState.IsValid)
            {
                var template = Mapper.Map<UserTemplate>(id);

                using(var db = new ADWebDB())
                {
                    template.Enabled = true;

                    if(string.IsNullOrEmpty(id.Notes))
                    {
                        id.Notes = "No Notes Entered for this User Template.";
                    }

                    db.UserTemplate.Add(template);
                    db.SaveChanges();

                    // We now have to iterate thru the list of groups that
                    // this user template has been configured to have so that
                    // we can add this information to the database. But we have 
                    // to also be careful to make sure that only valid groups are
                    // added to the database because we are allowing users to enter
                    // the name of the group(s) they are looking for, there is a
                    // possibility that a group may not exist in the domain.
                    ADDomain domain = new ADDomain();
                    ADGroup group;

                    // There is a possibility that the users will not add any groups
                    // when they first create the user template. We have to check for
                    // this now so that we can avoid a nasty error message!
                    if(id.Groups.Count > 0)
                    {
                        foreach(var grp in id.Groups)
                        {
                            group = domain.GetGroupBasicInfo(grp);

                            // We have to check if this group is in the domain, if it
                            // is then we would have retrieved a name for the group.
                            // If it's not a valid name, then the group name will be
                            // blank and thus this is a group that doesn't exit in  
                            // the domain.
                            if(!string.IsNullOrWhiteSpace(group.GroupName))
                            {
                                db.UserTemplateGroup.Add(new UserTemplateGroup()
                                                        {
                                                            Enabled = true,
                                                            Name = group.GroupName,
                                                            DistinguishedName = group.DN,
                                                            UserTemplateID = template.UserTemplateID
                                                        });
                                db.SaveChanges();
                            }
                        }
                    }

                    TempData["user_template_created"] = "The user template '" + template.Name + "' has been created successfully!";

                    return RedirectToAction("UserTemplates");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult ViewUserTemplate(int id)
        {
            using(var db = new ADWebDB())
            {
                ViewUserTemplateVM utVM = new ViewUserTemplateVM();

                utVM.UserTemplate = db.UserTemplate.Where(ut => ut.UserTemplateID == id).FirstOrDefault();
                var ous = db.DomainOU.Where(ou => ou.Enabled == true).ToList();
                
                List<SelectListItem> ouItems = new List<SelectListItem>();
                foreach(var ou in ous)
                {
                    ouItems.Add(new SelectListItem { Text = ou.Name, 
                                                     Value = ou.DomainOUID.ToString(), 
                                                     Selected = utVM.UserTemplate.DomainOUID == ou.DomainOUID });
                }

                List<SelectListItem> utStatus = new List<SelectListItem>();
                utStatus.Add(new SelectListItem() { Text = "Enabled", Value = "true" });
                utStatus.Add(new SelectListItem() { Text = "Disabled", Value = "false" });
                
                ViewBag.OUList = ouItems;
                ViewBag.UTStatus = utStatus;

                // We are only interested in seeing groups that are enabled. The users have the
                // ability to remove groups that have been added to this template, at which time
                // those groups have their Enabled property set to false.
                utVM.UserTemplate.Groups = utVM.UserTemplate.Groups.Where(g => g.Enabled).ToList();

                if(utVM.UserTemplate != null)
                {
                    return View(utVM);
                }
                else
                {
                    TempData["invalid_user_template"] = "Invalid User template ID";
                    return RedirectToAction("UserTemplates");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserTemplate(ViewUserTemplateVM id)
        {
            if(ModelState.IsValid)
            {
                using(var db = new ADWebDB())
                {
                    db.Entry(id.UserTemplate).State = EntityState.Modified;
                    
                    if(string.IsNullOrEmpty(id.UserTemplate.Notes))
                    {
                        id.UserTemplate.Notes = "No Notes entered for this template.";
                    }

                    db.SaveChanges();
                    
                    // We need to check to see if a new group (or groups) have been 
                    // added to this user template. If so then we'll add the groups!
                    if(id.Groups.Count > 0)
                    {
                        ADDomain domain = new ADDomain();
                        ADGroup group;

                        // We also have to check that the group(s) being added to this
                        // user template don't alreay exist. If it does, then it will 
                        // not be added. For us to do this check, we have to get the list
                        // of groups first. Also, please note that we have to check that we
                        // only get active groups! 
                        var existingGroups = db.UserTemplateGroup
                                               .Where(u => u.UserTemplateID == id.UserTemplate.UserTemplateID && u.Enabled == true)
                                               .Select(u => u.Name).ToList();

                        foreach(var grp in id.Groups)
                        {

                            // This is where we check if this user template already has
                            // the group that is being added. If it does, then we simply
                            // continue out of this iteration of the foreach loop and go on 
                            // to the next group being added.
                            if(existingGroups.Contains(grp))
                            {
                                continue;
                            }

                            group = domain.GetGroupBasicInfo(grp);

                            // We have to check if this group is in the domain, if it
                            // is then we would have retrieved a name for the group.
                            // If it's not a valid name, then the group name will be
                            // blank and thus this is a group that doesn't exit in  
                            // the domain.
                            if(!string.IsNullOrWhiteSpace(group.GroupName))
                            {
                                db.UserTemplateGroup.Add(new UserTemplateGroup()
                                                        {
                                                            Enabled = true,
                                                            Name = group.GroupName,
                                                            DistinguishedName = group.DN,
                                                            UserTemplateID = id.UserTemplate.UserTemplateID
                                                        });
                                db.SaveChanges();
                            }
                        }
                    }

                    TempData["user_template_updated"] = "The user template '" + id.UserTemplate.Name + "' has been successfully updated!";
                    return RedirectToAction("UserTemplates");
                }
            }
            else
            {
                TempData["error_updating_user_template"] = "Error updating Template";
                return RedirectToAction("ViewUserTemplate", new { id = id.UserTemplate.UserTemplateID });
            }
        }
    
        public ActionResult SearchForGroups(string term)
        {
            ADDomain domain = new ADDomain();
            List<string> groupsFound = domain.SearchGroups(term);

            return Json(groupsFound, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void RemoveGroupFromUserTemplate(string groupID)
        {
            using(var db =  new ADWebDB())
            {
                UserTemplateGroup group = db.UserTemplateGroup.Find(Int32.Parse(groupID));
                if(group != null)
                {
                    group.Enabled = false;
                    db.SaveChanges();
                }
            }
        }
    
        /// <summary>
        /// This method checks to see if the name of the template being created
        /// is unique. If the name of the template already exists, then this wil.
        /// return false and the user will not be able to create the template. 
        /// If the name is unique, then the user will be able to continue creating
        /// the template.
        /// </summary>
        /// <param name="TemplateName"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult IsTemplateNameUnique(string Name)
        {
            using(var db = new ADWebDB())
            {
                var doesTemplateExist = db.UserTemplate.Any(t => t.Name == Name);
                
                if(doesTemplateExist)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
