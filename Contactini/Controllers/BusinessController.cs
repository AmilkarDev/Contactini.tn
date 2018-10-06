using Contactini.DAL;
using Contactini.Models.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Controllers
{
    [Authorize]
    public class BusinessController : Controller
    {
        public BusinessController() { }
        private ContactiniContext db = new ContactiniContext();
        public BusinessController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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
        /// <summary>
        /// get user profile picture
        /// </summary>
        /// <returns></returns>
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
        // GET: Missions
        /// <summary>
        ///Get List of missions
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 4)
        {
            List<SelectListItem> domainList = new List<SelectListItem>();
            List<SelectListItem> sectorList = new List<SelectListItem>();
            List<SelectListItem> stateList = new List<SelectListItem>();
            foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Addresses.ToList()) stateList.Add(new SelectListItem { Text = item.State.ToLower() , Value = item.State });
            ViewBag.Sectors = (IEnumerable<SelectListItem>)sectorList;
            ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            ViewBag.States = (IEnumerable<SelectListItem>)stateList.Distinct();
            var list = db.Missions.ToList();
            PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
            ViewBag.photoLink = await getPicLink();
            return View(model);
        }
        /// <summary>
        /// Get list of missions result of the research
        /// </summary>
        /// <param name="form"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection form,int page = 1, int pageSize = 4 )
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
        /// new app for a specific job
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async     Task<ActionResult> Apply(int Id)
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
            string emailAddress = user.Email;
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
            if (sp == null)
            {
                return Content("although you have access to service Provider Space , but you are not registered as Service Provider in Our dtabse , Please contact our Platform administrators for a quick Fix , Thanks!");
            }
            Mission mis = db.Missions.Where(x => x.ID == Id).FirstOrDefault();

            if (mis.Candidatures != null)
            {
                Candidature cd = mis.Candidatures.Where(x => x.ServiceProvider == sp).FirstOrDefault();
                if (cd != null)
                {
                    return View("AlreadyApplied");
                }
            }
            return View();
        }
        [HttpPost]
        public  async Task<ActionResult> Apply([Bind(Include = "ID,Texte,Title")] Candidature candidature, int Id)
        {
            if (ModelState.IsValid)
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
                string emailAddress = user.Email;
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (sp == null)
                {
                    return Content("although you have access to service Provider Space , but you are not registered as Service Provider in Our dtabse , Please contact our Platform administrators for a quick Fix , Thanks!");
                }
                sp.Candidatures.Add(candidature);
                Mission mis = db.Missions.Where(x => x.ID == Id).FirstOrDefault();
                candidature.ServiceProvider = sp;
                candidature.Mission = mis;
                candidature.AppDate = DateTime.Today ;
                candidature.State = State.En_Attente;
                db.Candidatures.Add(candidature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(candidature);

        }

        /// <summary>
        /// get the connected SP's apps 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MyApps()
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
            string emailAddress = user.Email;
            ServiceProvider sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
            if (sp == null)
            {
                return Content("although you have access to service Provider Space , but you are not registered as Service Provider in Our dtabse , Please contact our Platform administrators for a quick Fix , Thanks!");
            }
            ViewBag.photoLink = await getPicLink();
            return View(sp.Candidatures);
        }
        /// <summary>
        /// remove an applicatioon for a job
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteApp(int Id)
        {
            try
            {
                Candidature cd  = db.Candidatures.Find(Id);
                db.Candidatures.Remove(cd);
                db.SaveChanges();
                return Content(Boolean.TrueString);
                
            }
            catch
            {
                return Content(Boolean.FalseString);
            }
        }
        /// <summary>
        /// add a mission to the list of fav mission of the connected SP
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ActionResult> addToFav(int Id)
        {
            try
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
                string emailAddress = user.Email;
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (sp == null)
                {
                    return Content("although you have access to service Provider Space , but you are not registered as Service Provider in Our dtabse , Please contact our Platform administrators for a quick Fix , Thanks!");
                }
                Mission mis = db.Missions.Where(x => x.ID == Id).FirstOrDefault();
                if (sp.favMissions.Contains(mis))
                {
                    ViewBag.popup = "Mission déja enregistré";
                    return View("popup");
                }
                sp.favMissions.Add(mis);
                db.SaveChanges();
                ViewBag.popup = "Mission ajouté au liste des favoris";
                return View("popup");
                }
                    catch
                    {
                        ViewBag.popup = "Erreur survenu";
                         return View("popup");
                    }
                }
        /// <summary>
        /// Get list of Fav missions of the connected Sp
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> myFav(int page = 1, int pageSize = 4)
        {
            try
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
                string emailAddress = user.Email;
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (sp == null)
                {
                    return Content("although you have access to service Provider Space , but you are not registered as Service Provider in Our dtabse , Please contact our Platform administrators for a quick Fix , Thanks!");
                }
                var list = sp.favMissions;
                PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
                ViewBag.photoLink = await getPicLink();
                return View(model);
            }
            catch
            {
                return View("Error");
            }
        }
        /// <summary>
        /// contact the client who posted the mission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult contactClient(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Mission m = db.Missions.Find(id);
            Client cl = m.Client;
            TempData["cl"] = cl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> contactClient(Message model)
        {
            if (model == null)
            {
                return View("Error");
            }
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
            string emailAddress = user.Email;
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
            string pemail = sp.Email;
            string name = sp.FullName;
            Client cl = (Client)TempData["cl"];
            string email = cl.Email;
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress(emailAddress);  // replace with valid value
                message.Subject = model.Title;
                message.Body = string.Format(body, name, pemail, model.Content);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    var credential = new NetworkCredential
                    {
                        UserName = "melek.ferhi@gmail.com",  // replace with valid value
                        Password = "vqtwbjnupxoelbgw"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(message);
                    return Json("Message envoyé", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("On a rencontré une problème à envoyer votre message ! veuillez essayez de nouveau dans quelques minutes .Merci");
        }

        /// <summary>
        /// after finishing a job , Sp would contact the client to demand a validation of his realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult demandValid(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Realisation r = db.Realisations.Find(id);
            Client cl = r.Client;
            TempData["cl"] = cl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> demandValid(Message model)
        {
            if (model == null)
            {
                return View("Error");
            }
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
            string emailAddress = user.Email;
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
            string pemail = sp.Email;
            string name = sp.FullName;
            Client cl = (Client)TempData["cl"];
            string email = cl.Email;
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress(emailAddress);  // replace with valid value
                message.Subject = model.Title;
                message.Body = string.Format(body, name, pemail, model.Content);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    var credential = new NetworkCredential
                    {
                        UserName = "melek.ferhi@gmail.com",  // replace with valid value
                        Password = "vqtwbjnupxoelbgw"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(message);
                    return Json("Message envoyé", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("On a rencontré une problème à envoyer votre message ! veuillez essayez de nouveau dans quelques minutes .Merci");
        }
        /// <summary>
        /// Gets missions in which the connected sp got accepted
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> myMissions()
        {
            string id = User.Identity.GetUserId();
            ViewBag.photoLink = await getPicLink();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(x => x.Email == emailAddress).FirstOrDefault();
            if (sp == null)
            {
                return Content("although you have access to service Provider Space , but you are not registered as Service Provider in Our dtabse , Please contact our Platform administrators for a quick Fix , Thanks!");
            }

            var list = sp.Missions.ToList();
            return View(list);
        }
        /// <summary>
        /// get the details of a specific realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Consult(int? id)
        {
            ViewBag.photoLink = await getPicLink();
            Mission ms = await db.Missions.FindAsync(id);
            if (ms == null)
            {
                ViewBag.popup = "Erreur à trouver la mission sélectionnée !";
                return View("popup1");
            }
            Realisation realisation = db.Realisations.Where(r => r.Mission.ID == ms.ID).FirstOrDefault();
            if (realisation == null)
            {
                ViewBag.popup = "Y'a aucune réalisation affectée à cette mission , lorsque vous termineé votre mission veuillez créer une nouvelle réalisation";
                return View("popup1");
            }
            return View(realisation);
        }
        /// <summary>
        /// shows details for the sp about the SP space and what to do && the way to get it done
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> About()
        {

            ViewBag.photoLink = await getPicLink();
            return View();
        }
        /// <summary>
        /// contact the administrators
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Contact()
        {

            ViewBag.photoLink = await getPicLink();
            return View();
        }
        /// <summary>
        /// Get the situation of hiw app ( accepted , rejected or waiting)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult state(int? id)
        {
            if (id == null) { 
                ViewBag.popup = "Une erreur survenu durant la procédure , essayez de nouveau dans quelques minutes";
            return View("popup");
            }
            Candidature cd = db.Candidatures.Find(id);
            ViewBag.popup = "Votre candidature est " + cd.State.ToString();
            return View("popup");
        }
        /// <summary>
        /// get the review given by the client for a specific realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult noteClient(int? id)
        {
            if (id == null)
            {
                ViewBag.popup = "aucune réalisation sélectionnée";
                return View("popup");
            }
            Realisation rl = db.Realisations.Find(id);
            if (rl == null)
            {
                ViewBag.popup = "aucune réalisation trouvée";
                return View("popup");
            }
            if ((rl.Opinion == null) && (rl.Stars == null))
            {

                ViewBag.popup = "Réalisation pas encore validée par le client";
                return View("popup");
            }
            if ((rl.Opinion == null) && (rl.Stars != null))
            {
                ViewBag.popup = "Aucune revue de la part du client";
                return View("popup");
            }
                return View(rl);
        }
    }
}