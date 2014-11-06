using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ADWeb.Controllers
{
    using System.Web.Security;
    using ADWeb.ViewModels;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Start");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Login(string returnUrl)
        {
            if(!String.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnurl)
        {
            if(ModelState.IsValid)
            {
                if(Membership.ValidateUser(model.Username, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);

                    if(Url.IsLocalUrl(returnurl) && returnurl.Length > 1 && 
                       returnurl.StartsWith("/") && !returnurl.StartsWith("//") && !returnurl.StartsWith("/\\"))
                    {
                        return Redirect(returnurl);
                    }
                    else
                    {
                        return RedirectToAction("Start", "Home");
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
        public ActionResult Start()
        {
            return View();
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
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}