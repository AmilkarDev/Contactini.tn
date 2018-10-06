using Contactini.DAL;
using Contactini.Models;
using Contactini.Models.Entities;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    /// <summary>
    /// this controller is the reception controller of visitor who aren't logged in or doesn't have an account yet
    /// </summary>
    public class HomeController : Controller
    {
        public HomeController() { }
        private ContactiniContext db = new ContactiniContext();
        /// <summary>
        /// Depends on whther the user is connected or not , if connected redirect him to the client space otherwise it redirects him to the
        /// reception page where ther is infos about our plaform
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated) {
                //return View(db.Missions.ToList());
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Missions");
            }
        }

        /// <summary>
        /// this is the method used for the contact form on the home index page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Send(Message model)
        {
            model.ReceiverEmail = "melek.ferhi@yahoo.com";
            string emailAddress = model.senderEmail;
            string email = model.ReceiverEmail;
            model.Title = "Message de la part de Contactini.tn";
            try
            {
                var body = "<p>Email From: {0} ({1})({2}) </p><p>Message:</p><p>{3}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress(emailAddress);  // replace with valid value
                message.Subject = model.Title;
                message.Body = string.Format(body, model.senderName, model.senderEmail, model.senderPhone, model.Content);
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
            catch
            {
                return Json("something went wrong");
            }
        }

        /// <summary>
        /// Shows the list of service providers in our platform
        /// </summary>
        /// <returns></returns>
        public  ActionResult prestaList()
        {
            if (Request.IsAuthenticated)
            {
                /*fixed tonight , replace reception by missions and everything is gonna be fine*/
                return RedirectToAction("Presta", "Missions");
            }
            var prest = new List<PrestaViewModel>();
            foreach (var item in db.ServiceProviders.ToList())
            {
                if (item.Domain != null && item.sector != null)
                {
                    PrestaViewModel ps = new PrestaViewModel();
                    ps.domain = item.Domain.Name;
                    ps.sector = item.sector.Name;
                    ps.photoLink = item.photoLink;
                    ps.stars = item.Stars;                   
                    ps.fullName = item.FullName;
                    ps.Nstars = 5 - item.Stars;
                    if (ps.photoLink == null) ps.photoLink = "~/Content/Images/Unknown.png";
                    if (item.Diponibility == false) ps.dispo = "Non Disponible";
                    else ps.dispo = "Disponible";
                    prest.Add(ps);
                }
            }
            return View(prest);
        }
        /// <summary>
        /// Show list of missions in our platform
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult MissionsList(int page = 1, int pageSize = 4)
        {
            if (Request.IsAuthenticated)
            {
               return RedirectToAction("Index", "Missions");
            }
            var list = db.Missions.ToList();
            PagedList<Mission> model = new PagedList<Mission>(list, page, pageSize);
            return View(model);
        }
        /// <summary>
        /// shows a popup for the non connected user when they want to know more more about a mission or about a service provider
        /// the popup whould redirect hime to login or create a new account
        /// </summary>
        /// <returns></returns>
        public ActionResult Connect()
        {
            ViewBag.popup = "Impossible de consulter plus de détails sans avoir connecter ! veuillez connecter ou créer un compte";
            return View("popup");
        }
    }
}
