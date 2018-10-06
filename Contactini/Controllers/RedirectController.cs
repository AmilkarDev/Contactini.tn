using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Controllers
{
    /// <summary>
    /// Ce controller est utilisé dans le but de redirection des utilisateurs entre les espaces accessibles
    /// This controller is used for redirection between spces accesible by the connected user
    /// </summary>
    public class RedirectController : Controller
    {
        public RedirectController() { }
        List<string> roles;
        public RedirectController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /// <summary>
        /// Next method find out the connected user and his roles , if he has only one role , it redirects him to his space
        /// otherwise it presents hime with dropdownList of his roles so he could access which space he want
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            //the fix for the default access
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            roles = new List<string>();
            string id = User.Identity.GetUserId();
            if (id == null)
            {               
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string str = "";
            if (user.PhotoLink != null)   str = user.PhotoLink ;
            else str = "~/Content/Images/Unknown.png" ;
            ViewBag.photoLink = str;
            int s = 0;
            if (User.IsInRole("Admin"))
            {
                roles.Add("Admin");
                s++;
            }

            if (User.IsInRole("Client"))
            {
                s++;
                roles.Add("Client");
            }
            if (User.IsInRole("ServiceProvider"))
            {
                roles.Add("ServiceProvider");
                s++;
            }
            Session["roles"] = roles;
            if (s > 1)
                return RedirectToAction("ContinueAs","Redirect", new { message = str });
            else
            {


                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin", new { message = str });
                }

                if (User.IsInRole("Client"))
                    return RedirectToAction("Index", "Missions", new { message = str });

                if (User.IsInRole("ServiceProvider"))
                    return RedirectToAction("Index", "Business", new { message = str });

                return RedirectToAction("Index", "Home");

            }
        }
        /// <summary>
        /// show an interface with a list of roles of the connected user so he choose which space to access (customer space /ServiceProvider/
        /// Admin space )
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult ContinueAs(string message)
        {
            List<string> roles = Session["roles"] as List<string>;
           
            ViewBag.roles = new SelectList(roles);
            ViewBag.photoLink = message;
            return View();
        }
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> ContinueAs(FormCollection form)
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string ss = "";
            if (user.PhotoLink != null) ss = user.PhotoLink;
            else ss = "~/Content/Images/Unknown.png";
           // ViewBag.photoLink = ss;
            string str = form["roles"].ToString();
            if (str == "Admin")
                return RedirectToAction("Index", "Admin", new { message = ss });
            if (str == "Client")
                return RedirectToAction("Index", "Missions", new { message = ss });
            if (str == "ServiceProvider")
                return RedirectToAction("Index", "Business", new { message = ss });
            return View();
        }
        /// <summary>
        /// Next method is used for the same purposes of the index , the only difference that it's called only when the user 
        /// is already connected and using his account , so it could redirect him to other spaces or
        /// show him that he has access only to his specific accessible space
        /// and 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Redirect()
        {
            roles = new List<string>();
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string str = "";
            if (user.PhotoLink != null) str = user.PhotoLink;
            else str = "~/Content/Images/Unknown.png";
            ViewBag.photoLink = str;
            int s = 0;
            if (User.IsInRole("Admin"))
            {
                roles.Add("Admin");
                s++;
            }

            if (User.IsInRole("Client"))
            {
                s++;
                roles.Add("Client");
            }
            if (User.IsInRole("ServiceProvider"))
            {
                roles.Add("ServiceProvider");
                s++;
            }
            Session["roles"] = roles;
            if (s > 1)
                return RedirectToAction("ContinueAs", "Redirect", new { message = str });
            else
            {
                //if (User.IsInRole("Admin"))
                //{
                //    return RedirectToAction("Index", "Admin", new { message = str });
                //}

                //if (User.IsInRole("Client"))
                //    return RedirectToAction("Index", "Missions", new { message = str });

                //if (User.IsInRole("ServiceProvider"))
                //    return RedirectToAction("Index", "Business", new { message = str });

                return RedirectToAction("NoRedirect", "Redirect", new { message = str });

            }
        }
        /// <summary>
        /// presents the user with  a popup showing that he has only one role so he has access only to the space he's already in
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async  Task<ActionResult> NoRedirect(string message)
        {
            ViewBag.photoLink = await getPicLink();
            if (User.IsInRole("Admin"))
            {
                ViewBag.popup = "Vous avez seulement accés à l'espace Admin";
                return View();
            }
            if (User.IsInRole("Client"))
            {
                ViewBag.popup = "Vous avez seulement accés à l'espace Client";
                return View();
            }
            if (User.IsInRole("ServiceProvider"))
            {
                ViewBag.popup = "Vous avez seulement accés à l'espace Prestataire";
                return View();
            }
            
            return View();
        }
        /// <summary>
        /// next method is called almost by every method in the app and implemented by all the controllers because it provides us with the
        /// profile picture of the current user that we need in order to show it on the menu next to his username
        /// </summary>
        public async Task<string> getPicLink()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return "Problem Identifying the connected user";
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return "Problem Identifying the connected user";
            }
            string ss = "";
            if (user.PhotoLink != null) ss = user.PhotoLink;
            else ss = "~/Content/Images/Unknown.png";
            return ss;
        }

    }
}