using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Contactini.DAL;
using Contactini.Models.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Contactini.Controllers
{
    /// <summary>
    /// Not user controller ! just some template scaffolded by EF asp mvc
    /// </summary>
    [Authorize]
    public class CandidaturesController : Controller
    {
        public CandidaturesController() { }
        private ContactiniContext db = new ContactiniContext();
        public CandidaturesController(ApplicationUserManager userManager)
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
        // GET: Candidatures
        public ActionResult Index()
        {
            return View(db.Candidatures.ToList());
        }

        // GET: Candidatures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidature candidature = db.Candidatures.Find(id);
            if (candidature == null)
            {
                return HttpNotFound();
            }
            return View(candidature);
        }

        // GET: Candidatures/Create
        public ActionResult Create(int Id)
        {
            return View();
        }

        // POST: Candidatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Texte,Title")] Candidature candidature , int Id)
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
                sp.Candidatures.Add(candidature);

                Mission mis = db.Missions.Where(x => x.ID == Id).FirstOrDefault();
                candidature.Mission = mis;
                db.Candidatures.Add(candidature);
                db.SaveChanges();
                return RedirectToAction("Index","Missions");
            }

            return View(candidature);
        }

        // GET: Candidatures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidature candidature = db.Candidatures.Find(id);
            if (candidature == null)
            {
                return HttpNotFound();
            }
            return View(candidature);
        }

        // POST: Candidatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Texte,Title")] Candidature candidature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(candidature);
        }

        // GET: Candidatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidature candidature = db.Candidatures.Find(id);
            if (candidature == null)
            {
                return HttpNotFound();
            }
            return View(candidature);
        }

        // POST: Candidatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Candidature candidature = db.Candidatures.Find(id);
            db.Candidatures.Remove(candidature);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
