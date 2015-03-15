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
                var userTemplate = db.UserTemplate.Where(ut => ut.UserTemplateID == id).FirstOrDefault();
                var ous = db.DomainOU.Where(ou => ou.Enabled == true).ToList();
                
                List<SelectListItem> ouItems = new List<SelectListItem>();
                foreach(var ou in ous)
                {
                    ouItems.Add(new SelectListItem { Text = ou.Name, 
                                                     Value = ou.DomainOUID.ToString(), 
                                                     Selected = userTemplate.DomainOUID == ou.DomainOUID });
                }

                List<SelectListItem> utStatus = new List<SelectListItem>();
                utStatus.Add(new SelectListItem() { Text = "Enabled", Value = "true" });
                utStatus.Add(new SelectListItem() { Text = "Disabled", Value = "false" });

                ViewBag.OUList = ouItems;
                ViewBag.UTStatus = utStatus;

                if(userTemplate != null)
                {
                    return View(userTemplate);
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
        public ActionResult UpdateUserTemplate(UserTemplate id)
        {
            if(ModelState.IsValid)
            {
                using(var db = new ADWebDB())
                {
                    db.UserTemplate.Attach(id);
                    db.Entry(id).Property(ut => ut.Name).IsModified = true;
                    db.Entry(id).Property(ut => ut.Enabled).IsModified = true;
                    db.Entry(id).Property(ut => ut.DomainOUID).IsModified = true;
                    db.Entry(id).Property(ut => ut.PasswordNeverExpires).IsModified = true;
                    db.Entry(id).Property(ut => ut.ChangePasswordAtNextLogon).IsModified = true;
                    db.Entry(id).Property(ut => ut.UserCannotChangePassword).IsModified = true;
                    db.Entry(id).Property(ut => ut.AccountExpires).IsModified = true;
                    db.Entry(id).Property(ut => ut.Notes).IsModified = true;
                    
                    db.SaveChanges();

                    TempData["user_template_updated"] = "The user template '" + id.Name + "' has been successfully update";
                    return RedirectToAction("UserTemplates");
                }
            }
            else
            {
                TempData["error_updating_user_template"] = "Error updating Template";
                return RedirectToAction("ViewUserTemplate", new { id = id.UserTemplateID });
            }
        }
    
        public ActionResult SearchForGroups(string term)
        {
            ADDomain domain = new ADDomain();
            List<string> groupsFound = domain.SearchGroups(term);

            return Json(groupsFound, JsonRequestBehavior.AllowGet);
        }
    }
}