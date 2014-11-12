using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ADWeb.Controllers
{
    using ADWeb.Domain.ViewModels;
    using ADWeb.Domain.ActiveDirectory;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                if(Membership.ValidateUser(model.Username, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                    if(Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && 
                       returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Unable to Login. The username or password is incorect.");
                }
            }

            return View();        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult MySettings()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult QuickSearch(SearchUsersModel model)
        {
            if(ModelState.IsValid)
            {
                ADDomain domain = new ADDomain();
                List<ADUser> users = domain.QuickSearch(model.SearchValue);
                ViewBag.SearchValue = model.SearchValue;
                return View("QuickSearchResults", users);
            }
            
            return View(model);
        }

        public ActionResult GettingStarted()
        {
            return View();
        }
        
        public ActionResult Help()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}