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
        public ActionResult CreateOU(DomainOU newOU)
        {
            if(ModelState.IsValid)
            {
                using(var db = new ADWebDB())
                {
                    newOU.Enabled = true;

                    db.DomainOU.Add(newOU);
                    db.SaveChanges();

                    TempData["ou_created"] = "The Organizationl Unit " + newOU.Name + " has been created successfully!";
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
                userTemplateVM.UserTemplate.ExpirationRange = UserExpirationRange.Days;

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
                using(var db = new ADWebDB())
                {
                    id.UserTemplate.Enabled = true;
                    db.UserTemplate.Add(id.UserTemplate);
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
                                                            UserTemplateID = id.UserTemplate.UserTemplateID
                                                        });
                                db.SaveChanges();
                            }
                        }
                    }

                    TempData["user_template_created"] = "The user template '" + id.UserTemplate.Name + "' has been created successfully!";

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

                // I am calling the ToList method here so that we can get a list groups
                // associated with this User Template. If we don'd do this here, then
                // I cannot get access to this list from the View (I get a message that
                // the context has already been disposed of and therefore cannot access
                // this information). Calling this method here should not be that big of 
                // hit performance wise as I don't expect user templates to have a lot of
                // groups associated with them.
                utVM.UserTemplate.Groups.ToList();

                ViewBag.OUList = ouItems;
                ViewBag.UTStatus = utStatus;

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
                    db.UserTemplate.Attach(id.UserTemplate);
                    db.Entry(id.UserTemplate).Property(ut => ut.Name).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.Enabled).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.DomainOUID).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.PasswordNeverExpires).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.ChangePasswordAtNextLogon).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.UserCannotChangePassword).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.AccountExpires).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.ExpirationRange).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.ExpirationValue).IsModified = true;
                    db.Entry(id.UserTemplate).Property(ut => ut.Notes).IsModified = true;
                    
                    db.SaveChanges();
                    
                    // We need to check to see if a new group (or groups) have been 
                    // added to this user template. If so then we'll add the group!
                    if(id.Groups.Count > 0)
                    {
                        ADDomain domain = new ADDomain();
                        ADGroup group;

                        // We also have to check that the group(s) being added to this
                        // user template don't alreay exist. If it does, then it will 
                        // not be added. For us to do this check, we have to get the list
                        // of groups first. Also, please note that we have to check that we
                        // only get active groups! 
                        var existingGroups = db.UserTemplateGroup.Where(u => u.UserTemplateID == id.UserTemplate.UserTemplateID && u.Enabled == true)
                                                                 .Select(u => u.Name).ToList();

                        foreach(var grp in id.Groups)
                        {

                            // This is where we check if this user template already has
                            // the group that is being added. If it does, then we simply
                            // break out of this iteration of the foreach loop and go on 
                            // to the next group being added.
                            if(existingGroups.Contains(grp))
                            {
                                break;
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

                    TempData["user_template_updated"] = "The user template '" + id.UserTemplate.Name + "' has been successfully update";
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
                group.Enabled = false;
                db.SaveChanges();
            }
        }
    }
}
