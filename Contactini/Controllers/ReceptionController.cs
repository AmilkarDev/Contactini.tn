using Contactini.DAL;
using Contactini.Models;
using Contactini.Models.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Controllers
{
        /// <summary>
        /// This controller is for the client's space 
        /// It has the most oprations allowed to be done by a client 
        /// the rest of the operations are listed on the Missions controller
        /// </summary>
    [Authorize(Roles =("Client"))]
    public class ReceptionController : Controller
    {
       
        public ReceptionController() { }
        private ContactiniContext db = new ContactiniContext();
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
       /// <summary>
       /// affiche le liste des prestataires
       /// </summary>
       /// <returns></returns>
        public async Task<ActionResult> presta()
        {
            var prest = new List<PrestaViewModel>();
            foreach (var item in db.ServiceProviders.ToList())
            {
                var user = UserManager.FindByEmail(item.Email);
                var rolesForUser = UserManager.GetRoles(user.Id);
                if (rolesForUser.Contains("ServiceProvider") && user !=null && item.Domain != null && item.sector != null)
                {
                    PrestaViewModel ps = new PrestaViewModel();
                    
                    ps.domain = item.Domain.Name;
                    ps.sector = item.sector.Name;
                    ps.photoLink = item.photoLink;
                    ps.stars = item.Stars;
                    ps.titre = item.Titre;
                    ps.fullName = item.FullName;
                    ps.Nstars = 5 - item.Stars;
                    if (ps.photoLink == null) ps.photoLink = "~/Content/Images/Unknown.png";
                    if (item.Diponibility == false) ps.dispo = "Non Disponible";
                    else ps.dispo = "Disponible";
                    prest.Add(ps);
                }
            }
            ViewBag.photoLink = await getPicLink();
            return View(prest);
        }
        /// <summary>
        /// shos the list of the realisation done by the service provider 
        /// show an interface with ab utton for the client to validate and provide his review for the 
        /// service provider job and efforts that are presented on the realisation
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Validations()
        {
            ViewBag.photoLink = await getPicLink();
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return HttpNotFound();
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            Client cl = db.Clients.Where(c => c.Email == user.Email).FirstOrDefault();
            var list = cl.Realisations.ToList();
            return View(list);

        }
        /// <summary>
        /// shows a popup for the customer so he could review the service provider ,
        /// If he aready reviewed the service provider , he could find on the popup the review he made and change it if he want
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Validate(int id)
        {
            Realisation realisation = db.Realisations.Find(id);
            return View(realisation);
        }
        [HttpPost]
        public ActionResult Validate(Realisation realisation)
        {           
            Realisation rr = db.Realisations.Where(r => r.ID == realisation.ID).FirstOrDefault();
            rr.Validation = true;
            rr.Stars = realisation.Stars;
            rr.Opinion = realisation.Opinion;
            db.SaveChanges();
            return RedirectToAction("Index","missions");
        }
        /// <summary>
        /// Not used for the actual situation of our platform , but it's supposed to provide the connected user as a client about
        ///his rights ,his duties when using our app and also tutoring him about how to use the application , notifu him about 
        ///updated versions of our app , changes we have made , and tell him to contact the administrators of the app if any error occurs
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> About()
        {

            ViewBag.photoLink = await getPicLink();
            return View();
        }
        /// <summary>
        /// supposed to show a contact form so the client could contact the administrators about any issue
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Contact()
        {

            ViewBag.photoLink = await getPicLink();
            return View();
        }
        /// <summary>
        /// Supposed to show the review of the customer that he already gave
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