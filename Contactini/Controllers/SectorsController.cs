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
using PagedList;

namespace Contactini.Controllers
{
    /// <summary>
    /// this controller provide operation that will be done on sectors
    /// </summary>
    [Authorize(Roles = ("Admin"))]
    public class SectorsController : Controller
    {
        private ContactiniContext db = new ContactiniContext();
        public SectorsController() { }
        public SectorsController(ApplicationUserManager userManager)
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
        /// get the connected user profile pic
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
        /// <summary>
        /// get list of sectors and the selected sector's list of domains
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="domainID"></param>
        /// <returns></returns>
        // GET: Sectors
        [HttpGet]
        public async Task<ActionResult> Index(int? Id , int? domainID)
         {

            var viewModel = new SectorViewModel();
            viewModel.sectors = db.Sectors
                .Include(d => d.Domain);
            if (Id != null)
            {
                ViewBag.sectorId = Id.Value;
                viewModel.domains = db.Sectors.Where(s => s.ID == Id).Single().Domain;
            }
            if (domainID != null)
            {
                ViewBag.domainId = domainID.Value;
                viewModel.missions = db.Domains.Where(s => s.domainId == domainID).Single().Missions;
            }
            TempData["vm"] = viewModel;
            ViewBag.photoLink = await getPicLink();
            return View(viewModel);
        }

        // GET: Sectors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sectors.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            return View(sector);
        }
        /// <summary>
        /// add new sector
        /// </summary>
        /// <returns></returns>
        // GET: Sectors/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sectors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //Method Create for the sectors
        public ActionResult Create([Bind(Include = "ID,Name")] Sector sector)
        {
            if (ModelState.IsValid)
            {
                db.Sectors.Add(sector);
                db.SaveChanges();
                return RedirectToAction("Index","Sectors", new { id = sector.ID });
            }

            return View(sector);
        }

        // GET: Sectors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sectors.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            return View(sector);
        }
        /// <summary>
        /// sector update
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        // POST: Sectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Sector sector)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sector).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sector);
        }
        /// <summary>
        /// delete a sector
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Sectors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sector sector = db.Sectors.Find(id);
            if (sector == null)
            {
                return HttpNotFound();
            }
            return View(sector);
        }

        // POST: Sectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sector sector = db.Sectors.Find(id);
            db.Sectors.Remove(sector);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// missions of s pecific domain
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> Missions(int id,int page = 1, int pageSize = 4)
        {
            Domain dd = db.Domains.Find(id);
            ViewBag.Name = dd.Name;
            var missions = db.Missions.Where(x => x.Domain.domainId == id).ToList();
            PagedList<Mission> model = new PagedList<Mission>(missions, page, pageSize);
            ViewBag.photoLink = await getPicLink();
            return View(model);
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
