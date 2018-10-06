using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Contactini.DAL;
using Contactini.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using IdentitySample.Models;
using System.Threading.Tasks;
using PagedList;
using System.Net.Mail;
using Contactini.Models;

namespace Contactini.Controllers
{
    [Authorize]
    public class MissionsController : Controller
    {
        public MissionsController() { }
        private ContactiniContext db = new ContactiniContext();
        public MissionsController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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
        ///Get connected user profile pic
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
        /// get the list of missions
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
            foreach (var item in db.Addresses.ToList()) //stateList.Add(new SelectListItem { Text = item.State.ToLower().Trim(), Value = item.State });

            {
                var bla = new SelectListItem { Text = item.State.ToLower().Trim(), Value = item.State.ToLower().Trim() };
                if (!stateList.Exists(elem=>elem.Text==bla.Text&&elem.Value==bla.Value))
                stateList.Add(bla);
            }
                //stateList.Add(new SelectListItem { Text = item.State.ToLower().Trim() , Value = item.State });
            ViewBag.Sectors = (IEnumerable<SelectListItem>)sectorList;
            ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            ViewBag.States = (IEnumerable<SelectListItem>)stateList;

            var list = db.Missions.ToList();
            PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
            ViewBag.photoLink = await getPicLink();
            return View(model);
        }
        /// <summary>
        /// get the list of missions result of the research
        /// </summary>
        /// <param name="form"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection form, int page = 1, int pageSize = 4)
        {

            List<SelectListItem> domainList = new List<SelectListItem>();
            List<SelectListItem> sectorList = new List<SelectListItem>();
            List<SelectListItem> stateList = new List<SelectListItem>();
            foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            foreach (var item in db.Addresses.ToList())// stateList.Add(new SelectListItem { Text = item.State, Value = item.State });
            {
                var bla = new SelectListItem { Text = item.State.ToLower().Trim(), Value = item.State.ToLower().Trim() };
                if (!stateList.Exists(elem => elem.Text == bla.Text && elem.Value == bla.Value))
                    stateList.Add(bla);
            }
            ViewBag.Sectors = (IEnumerable<SelectListItem>)sectorList;
            ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            ViewBag.States = (IEnumerable<SelectListItem>)stateList;

            string sector = form["Sectors"].ToString();
            string domain = form["Domains"].ToString();
            string state = form["States"].ToString();
            var list = db.Missions.ToList();
            if (sector != "") list = list.Where(l => l.Sector.Name == sector).ToList();
            if (domain != "") list = list.Where(d => d.Domain.Name == domain).ToList();
            if (state != "") list = list.Where(s => s.Address.State.ToLower().Trim() == state).ToList();
            PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
            if (model == null) ViewBag.message = "aucune résultat !";
            ViewBag.photoLink = await getPicLink();
            return View(model);
        }

        /// <summary>
        /// shows the details of a mission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Missions/Details/5
        public ActionResult Details(int? id)
        {
            if (Request.IsAjaxRequest())
            {

           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission mission = db.Missions.Find(id);
            if (mission == null)
            {
                return HttpNotFound();
            }
            return PartialView(mission);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        /// <summary>
        /// Add a new mission/new Offer
        /// </summary>
        /// <returns></returns>
        // GET: Missions/Create
        public async    Task<ActionResult> Create()
        {
            List<SelectListItem> domainList = new List<SelectListItem>();
            List<SelectListItem> sectorList = new List<SelectListItem>();
            foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem {Text=item.Name,Value=item.Name });
            foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            ViewBag.Sectors =  (IEnumerable<SelectListItem>)sectorList;
            ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            string ss= await getPicLink();
            ViewBag.photoLink = ss;
            return View();
        }

        // POST: Missions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( Mission mission,FormCollection form, HttpPostedFileBase upload)
        {
            //if (ModelState.IsValid)
            //{
            //try { 
                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new Photo
                    {
                        Name = System.IO.Path.GetFileName(upload.FileName),
                        Link = "~/Content/Images/" + upload.FileName,

                    };
                    //   photo.Link = "~/Helpers/" + photo.Name;
                    upload.SaveAs(Server.MapPath(photo.Link));
                    db.Photos.Add(photo);
                    mission.PhotoLink = photo.Link;
                }
                else
                {
                    mission.PhotoLink = "~/Content/Images/jobOffer.jpg";
                }
                Address ad = mission.Address;
                db.Addresses.Add(ad);
                //Finding the client who posted this mission , so to add this mission to his list of propopsitions
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
                Client cust = new Client();
                cust = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (cust == null)
                {
                    // return HttpNotFound();
                    return Content("Although you have the customer role but you are not registerd as a Client in our database , please contact the administrators for a quick Fix , Thanks");
                }
                //adding the mssion state and publishDate which are auomatic not manual !!
                mission.PublishDate = DateTime.Now.Date;
                mission.State = Status.Ouverte;
                string sector = form["Sector"].ToString();
                string domain = form["Domain"].ToString();
                Domain dd = db.Domains.Where(x => x.Name == domain).FirstOrDefault();
                Sector ss = db.Sectors.Single(x => x.Name == sector);
                mission.Domain = dd;
                mission.Sector = ss;
                cust.Missions = new List<Mission>();
                cust.Missions.Add(mission);
                dd.Missions.Add(mission);
                dd.MissionCount = dd.Missions.Count();
                db.Missions.Add(mission);
                db.SaveChanges();
               return RedirectToAction("Index");
            //}

            //catch
            //{
            //    List<SelectListItem> domainList = new List<SelectListItem>();
            //    List<SelectListItem> sectorList = new List<SelectListItem>();
            //    foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            //    foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            //    ViewBag.Sectors = (IEnumerable<SelectListItem>)sectorList;
            //    ViewBag.Domains = (IEnumerable<SelectListItem>)domainList;
            //    return View(mission);
            //}
            
        }
        /// <summary>
        /// Modify an existing mission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Missions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission mission = db.Missions.Find(id);

            if (mission == null)
            {
                return HttpNotFound();
            }
            string ss = await getPicLink();
            ViewBag.photoLink = ss;


            var dom = db.Domains.ToList();
            var sec = db.Sectors.ToList();
            if (mission.Domain == null || mission.Sector == null)
            {
                mission.Domain = db.Domains.Where(d => d.Name == "aucun").FirstOrDefault();
                mission.Sector = db.Sectors.Where(s => s.Name == "aucun").FirstOrDefault();
            }

            ViewBag.Domains = new SelectList(dom, "Name", "Name", mission.Domain.Name);                                              //(IEnumerable<SelectListItem>)domainList;
            ViewBag.Sectors = new SelectList(sec, "Name", "Name", mission.Sector.Name);


            //PopulateSectorsDropDownList(mission.Sector.ID);
            //PopulateDomainsDropDownList(mission.Domain.domainId);
            return View(mission);
        }
       /// <summary>
       /// Provide list of domains
       /// </summary>
       /// <param name="selectedDomain"></param>
        private void PopulateDomainsDropDownList(object selectedDomain = null)
        {
            var domainsQuery = from d in db.Domains
                               orderby d.Name
                               select d;
            ViewBag.Domains = new SelectList(domainsQuery, "domainId", "Name", selectedDomain);
        }
        /// <summary>
        /// provide list of sectors
        /// </summary>
        /// <param name="selectedSector"></param>
        private void PopulateSectorsDropDownList(object selectedSector = null)
        {
            var sectorsQuery = from d in db.Sectors
                                   orderby d.Name
                                   select d;
            ViewBag.Sectors = new SelectList(sectorsQuery, "ID", "Name", selectedSector);
        }
        // POST: Missions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mission mission, HttpPostedFileBase upload, FormCollection form, params string[] Sectors)
        {


            /*I dunnà why the fuck it get the edited related data from the form only before the Model State ,
             * It says that the model state doesn't Keep the reinitialaized data 
              like those from the dropdownListFor , therefore fuck the ModelState ,we're gonna keep it outside the code*/
            //if (ModelState.IsValid)
            //{
            try { 
                Mission mi = db.Missions.Where(x => x.ID == mission.ID).FirstOrDefault();
                mi.Domain = db.Domains.Where(d => d.domainId == mission.Domain.domainId).FirstOrDefault();
                mi.Sector = db.Sectors.Where(s => s.ID == mission.Sector.ID).FirstOrDefault();
                mi.Description = mission.Description;
                mi.Title = mission.Title;
                mi.PublishDate = mission.PublishDate.Date;
                mi.StartDate = mission.StartDate;
                mi.Duration = mission.Duration;
                mi.State = mission.State;
                mi.PhotoLink = mission.PhotoLink;
                if (upload != null && upload.ContentLength > 0)
                {
                    var photo = new Photo
                    {
                        Name = System.IO.Path.GetFileName(upload.FileName),
                        Link = "~/Content/Images/" + upload.FileName,
                    };
                    upload.SaveAs(Server.MapPath(photo.Link));
                    db.Photos.Add(photo);
                    mi.PhotoLink = photo.Link;
                }
                else
                {
                    mi.PhotoLink = mission.PhotoLink;
                }
                //Sectors = Sectors ?? new string[] { };
                Domain dd = db.Domains.Where(x => x.Name == mission.Domain.Name).FirstOrDefault();
                Sector sr = db.Sectors.Single(x => x.Name == mission.Sector.Name);
                mi.Domain = dd;
                mi.Sector = sr;
                db.SaveChanges();
                return RedirectToAction("MyMissions");
            }
            //}
            catch  {
                var dom = db.Domains.ToList();
                var sec = db.Sectors.ToList();
                if (mission.Domain == null || mission.Sector == null)
                {
                    mission.Domain = db.Domains.Where(d => d.Name == "aucun").FirstOrDefault();
                    mission.Sector = db.Sectors.Where(s => s.Name == "aucun").FirstOrDefault();
                }

                ViewBag.Domains = new SelectList(dom, "Name", "Name", mission.Domain.Name);
                ViewBag.Sectors = new SelectList(sec, "Name", "Name", mission.Sector.Name);
                return View(mission);
            }
        }
       /// <summary>
       /// delete an offer/mission
       /// </summary>
       /// <param name="Id"></param>
       /// <returns></returns>
        [HttpPost]
     //   [ValidateAntiForgeryToken]
        public ActionResult DeleteMission(int Id)
        {
            //try
            //{
                Mission mission = db.Missions.Find(Id);
                db.Missions.Remove(mission);
                var cd = db.Candidatures.Where(c => c.Mission.ID == Id).ToList();
                foreach (var item in cd) db.Candidatures.Remove(item);
                db.SaveChanges();
                return Content(Boolean.TrueString);
                //return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return Content(Boolean.FalseString);
            //}
        }
        /// <summary>
        /// shows the mission of the client already connected (connected user)
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MyMissions()
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
            Client cust = new Client();
            cust = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
            List<Mission> missions = new List<Mission>();
                missions = cust.Missions.ToList();
            ViewBag.photoLink = await getPicLink();
            return View(missions);
        }
        /// <summary>
        /// shows the apps done for the a specific mission by the service providers of the platform
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Candidatures(int? Id)
        {
            Mission mis = db.Missions.Where(x => x.ID == Id).FirstOrDefault();
            ViewBag.photoLink = await getPicLink();
            if (mis == null)
            {
                return Content("aucune Mission Sélectionnée !!");
            }
            return View(mis.Candidatures.ToList());
        }
        /// <summary>
        /// shows app details/détails du candidature
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult App_Details(int? id)
        {
            if (Request.IsAjaxRequest())
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidature ca = db.Candidatures.Where(x => x.ID == id).FirstOrDefault();
            if (ca == null)
            {
                return HttpNotFound();
            }
            return PartialView(ca);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        /// <summary>
        /// supposed to modify an app for the job but that's not allowed in our platform
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AppEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidature ca = db.Candidatures.Find(id);
            if (ca == null)
            {
                return HttpNotFound();
            }
            return View(ca);
            
        }
        [HttpPost]
        public ActionResult AppEdit([Bind(Include = "ID,Texte,Title,State")] Candidature candidature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Candidatures");
            }
            return View(candidature);
        }
        /// <summary>
        /// accept an application
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult AppAccept(int? Id)
        {
            if (Id == null)
            {
                return View("Error");
            }
            Candidature cd = db.Candidatures.Find(Id);
            if (cd.State == State.Acceptée) ViewBag.message = " Candidature déja accepté , On a notifié le prestataire";
            if (cd.State == State.Rejetée) ViewBag.message = "Candidature déja Rejeté : vous ppouvez pas l'accepter maintenant ";
            if (cd.State == State.En_Attente)
            {
                cd.State = State.Acceptée;
                ViewBag.message = "Candidature accepté ! Le prestataire sera notifié ";
                cd.Mission.State = Status.Fermée;
                /*ajouté aujourdh'ui pour mes mission coté prestataire*/
                cd.Mission.ServiceProvider = cd.ServiceProvider;
                ServiceProvider sp = cd.ServiceProvider;
                sp.Missions.Add(cd.Mission);
                db.SaveChanges();
            }
           
            return View();
        }
        /// <summary>
        /// Reject an application for a specific job
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult AppReject(int? Id)
        {
            if(Id == null)
            {
                return View("Error");
            }
            Candidature cd = db.Candidatures.Find(Id);
            if (cd.State == State.Rejetée) ViewBag.message = "Candidature déja Rejeté !";
            if (cd.State == State.Acceptée) ViewBag.message = "Candidature déja Accepté ! vous pouvez pas la rejeter maintenet ";
            if (cd.State == State.En_Attente)
            {
                cd.State = State.Rejetée;
                db.SaveChanges();
                ViewBag.message = "  Candidature Rejeté : Le prestataire sera notifié !";
            }
            return View();
        }
        /// <summary>
        /// bla bla bla bla
        /// </summary>
        /// <returns></returns>
        public ActionResult Apply()
        {
            return View();
        }
        //C'est pour contcter le prestataire en utilisant son candidature
        /// <summary>
        /// after visiting the service provider cv and motivation letter , 
        /// client will contact him for an interview via our messging system
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Send(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Candidature cr = db.Candidatures.Find(id);
            ServiceProvider sp = cr.ServiceProvider;
            TempData["sp"] = sp;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Send(Message model)
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
            Client cust = new Client();
            cust = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
            string cemail = cust.Email;
            string name = cust.FullName;
            ServiceProvider sp = (ServiceProvider)TempData["sp"];
            string email = sp.Email;
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress(emailAddress);  // replace with valid value
                message.Subject = model.Title;
                message.Body = string.Format(body, name, cemail, model.Content);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.gmail.com",587))
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
                    //return View("Sent");
                 return Json("Message envoyé",JsonRequestBehavior.AllowGet);
                }
            }
            return Json("On a rencontré une problème à envoyer votre message ! veuillez essayez de nouveau dans quelques minutes .Merci");
        }
        /// <summary>
        /// from clinet space a client could contact another client provider of a job posted in our platform
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> contactClient(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            //finding the connected user
           
            string identif = User.Identity.GetUserId();
            if (identif == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(identif);
            if (user == null)
            {
                return HttpNotFound();
            }
            string emailAddress = user.Email;


            Mission m = db.Missions.Find(id);
            Client cl = m.Client;
            //check if the client email is the same as the connected user email , 
            //means if the user is the one who posted the mission
            if (cl.Email == emailAddress)
            {
                ViewBag.popup = "ce formulaire s'affiche pour que nos clients puissent contacter les propriétaires des missions affichées !! vous ètes le propriétaire du mission sujet ";
                return View("popup");

            }

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
            Client cust = new Client();
            cust = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
            string cemail = cust.Email;
            string name = cust.FullName;
            Client cl = (Client)TempData["cl"];
            string email = cl.Email;
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress(emailAddress);  // replace with valid value
                message.Subject = model.Title;
                message.Body = string.Format(body, name, cemail, model.Content);
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
        /// bla bla bla bla
        /// </summary>
        /// <returns></returns>
        public ActionResult Sent()
        {
            return View();
        }
        /// <summary>
        /// liberating ressources of the app that are unmanaged or not needed anymore
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// shows the cv of service provider who applied for the job
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActionResult Findex(string email)
        {
            Session["email"] = email;
            ServiceProvider sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            ViewBag.photoLink = sp.photoLink;
            return View(sp);
        }
        /// <summary>
        /// shows list of service providers
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> presta()
        {
            //var prest = new List<PrestaViewModel>();
            //foreach (var item in db.ServiceProviders.ToList())
            //{
            //    if (item.Domain != null && item.sector != null)
            //    {
            //        PrestaViewModel ps = new PrestaViewModel();

            //        ps.domain = item.Domain.Name;
            //        ps.sector = item.sector.Name;
            //        ps.photoLink = item.photoLink;
            //        ps.stars = item.Stars;
            //        ps.titre = item.Titre;
            //        if (item.Titre == null) ps.titre = "Non spécifié";
            //        ps.fullName = item.FullName;
            //        ps.Nstars = 5 - item.Stars;
            //        if (ps.photoLink == null) ps.photoLink = "~/Content/Images/Unknown.png";
            //        if (item.Diponibility == false) ps.dispo = "Non Disponible";
            //        else ps.dispo = "Disponible";
            //        prest.Add(ps);
            //    }
            //}
            ViewBag.photoLink = await getPicLink();
            return View(db.ServiceProviders.ToList());
        }
        /// <summary>
        /// get the domains depending on the chosen sector
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FillDomains(string sector)
        {
            var secteur = db.Sectors.Where(s => s.Name == sector).FirstOrDefault();
            List<Domain> domains = secteur.Domain.ToList();
            List<string> dvm = new List<string>();
            foreach (var item in domains)
            {
                dvm.Add(item.Name);
            }
            return Json(dvm, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get the favorites registered service providers
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> myFav(int page = 1, int pageSize = 6)
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
                Client cl = new Client();
                cl = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (cl == null)
                {
                    return Content("Malgré que vous avez accés à l'espace client , vous etes pas enregistré dans notre base de données ,vauillez contactez les administrateurs pour fixer la probleme ! merci");
                }
                var list = cl.favPresta;
                PagedList<ServiceProvider> model = new PagedList<ServiceProvider>(list, page, pageSize);
                ViewBag.photoLink = await getPicLink();
                return View(model);
            }
            catch
            {
                return View("Error");
            }
        }
        /// <summary>
        /// add a service provider to the favorite list
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
                Client cl = new Client();
                cl = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (cl == null)
                {
                    return Content("Malgré que vous avez accés à l'espace client , vous etes pas enregistré dans notre base de données ,vauillez contactez les administrateurs pour fixer la probleme ! merci");
                }
                ServiceProvider sp = db.ServiceProviders.Where(x => x.ID == Id).FirstOrDefault();
                if (cl.favPresta.Contains(sp))
                {
                    ViewBag.popup = "Prestataire déja enregistré";
                    return View("popup");
                }
                cl.favPresta.Add(sp);
                db.SaveChanges();
                ViewBag.popup = "Prestataire ajouté au liste des favoris";
                return View("popup");
            }
            catch
            {
                ViewBag.popup = "Erreur survenu";
                return View("popup");
            }
        }
        /// <summary>
        /// remove a service provider from he fav list
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ActionResult> removeFav(int Id)
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
                Client cl = new Client();
                cl = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
                if (cl == null)
                {
                    return Content("Malgré que vous avez accés à l'espace client , vous etes pas enregistré dans notre base de données ,vauillez contactez les administrateurs pour fixer la probleme ! merci");
                }
                ServiceProvider sp = db.ServiceProviders.Where(x => x.ID == Id).FirstOrDefault();
                if (!cl.favPresta.Contains(sp))
                {
                    ViewBag.popup = "Prestataire déja supprimé de votre favoris.";
                    return View("popup");
                }
                cl.favPresta.Remove(sp);
                db.SaveChanges();
                ViewBag.popup = "Prestataire supprimé de votre favoris";
                return View("popup");
            }
            catch
            {
                ViewBag.popup = "Erreur survenu";
                return View("popup");
            }
        }
        /// <summary>
        /// show the details of a service provider
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DetailsPresta(int Id)
        {
            ServiceProvider sp = db.ServiceProviders.Where(x => x.ID == Id).FirstOrDefault();
            return View(sp);
        }

        /// <summary>
        /// contact a service provider
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult contactPresta(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            ServiceProvider sp = db.ServiceProviders.Find(id);
            TempData["sp"] = sp;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> contactPresta(Message model)
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
            Client cust = new Client();
            cust = db.Clients.Where(x => x.Email == emailAddress).FirstOrDefault();
            string cemail = cust.Email;
            string name = cust.FullName;
            ServiceProvider sp = (ServiceProvider)TempData["sp"];
            string email = sp.Email;
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress(emailAddress);  // replace with valid value
                message.Subject = model.Title;
                message.Body = string.Format(body, name, cemail, model.Content);
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
                    //return View("Sent");
                    return Json("Message envoyé", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("something went wrong");
        }
    }
}
