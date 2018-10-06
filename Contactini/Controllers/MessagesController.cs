using Contactini.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Contactini.Controllers
{

    /// <summary>
    /// Not implemented yet , watch this controller for future update
    /// supposed to show the exchange of messages in our platform
    /// </summary>
    public class MessagesController : Controller
    {
        ContactiniContext db = new ContactiniContext();
        // GET: Messages
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Received()
        {
            var mes = db.Messages.ToList();
            return ViewBag(mes);

        }
        public ActionResult SentMessages()
        {
            var mes = db.Messages.ToList();
            return ViewBag(mes);

        }
    }
}