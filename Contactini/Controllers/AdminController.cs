using Contactini.DAL;
using Contactini.Models.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public AdminController() { }
        private ContactiniContext db = new ContactiniContext();
        public AdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;

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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        //Get connected user profile pic
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
        /// <summary>
        /// shows List of missions
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        // GET: Missions
        [HttpGet]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 4)
        {
            List<SelectListItem> domainList = new List<SelectListItem>();
            List<SelectListItem> sectorList = new List<SelectListItem>();
            List<SelectListItem> stateList = new List<SelectListItem>();
            foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Addresses.ToList()) stateList.Add(new SelectListItem { Text = item.State.ToLower(), Value = item.State });
            ViewBag.Sectors = (IEnumerable<SelectListItem>)sectorList;
            ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            ViewBag.States = (IEnumerable<SelectListItem>)stateList.Distinct();

            var list = db.Missions.ToList();
            PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
            ViewBag.photoLink = await getPicLink();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection form, int page = 1, int pageSize = 4)
        {

            List<SelectListItem> domainList = new List<SelectListItem>();
            List<SelectListItem> sectorList = new List<SelectListItem>();
            List<SelectListItem> stateList = new List<SelectListItem>();
            foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Addresses.ToList()) stateList.Add(new SelectListItem { Text = item.State, Value = item.State });
            ViewBag.Sectors = (IEnumerable<SelectListItem>)sectorList;
            ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            ViewBag.States = (IEnumerable<SelectListItem>)stateList;

            string sector = form["Sectors"].ToString();
            string domain = form["Domains"].ToString();
            string state = form["States"].ToString();
            var list = db.Missions.ToList();
            if (sector != "") list = list.Where(l => l.Sector.Name == sector).ToList();
            if (domain != "") list = list.Where(d => d.Domain.Name == domain).ToList();
            if (state != "") list = list.Where(s => s.Address.State == state).ToList();
            PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
            if (model == null) ViewBag.message = "aucune résultat !";
            ViewBag.photoLink = await getPicLink();
            return View(model);
        }
        /// <summary>
        /// delete a mission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteMission(int id)
        {
            try
            {
                Mission mission = db.Missions.Find(id);
                db.Missions.Remove(mission);
                var cd = db.Candidatures.Where(c => c.Mission.ID == id).ToList();
                foreach (var item in cd) db.Candidatures.Remove(item);
                db.SaveChanges();
                return Content(Boolean.TrueString);
                //return RedirectToAction("Index");
            }
            catch
            {
                return Content(Boolean.FalseString);
            }
        }

        /// <summary>
        /// tells info about the admi space what should he do and how get it done 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> About()
        {

            ViewBag.photoLink = await getPicLink();
            return View();
        }
        /// <summary>
        /// contact a specific user about some issue
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Contact()
        {

            ViewBag.photoLink = await getPicLink();
            return View();
        }
    }
}