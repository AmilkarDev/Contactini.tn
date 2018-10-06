using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Contactini.DAL;
using Contactini.Models.Entities;
using System.IO;
using Microsoft.AspNet.Identity;
using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;

namespace Contactini.Controllers
{
    [Authorize(Roles = ("ServiceProvider"))]
    public class RealisationsController : Controller
    {
        public RealisationsController() { }
        private ContactiniContext db = new ContactiniContext();
        public RealisationsController(ApplicationUserManager userManager)
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
        /// get list of all the realisations
        /// </summary>
        /// <returns></returns>
        // GET: Realisations
        public async Task<ActionResult> Index()
        {
            ViewBag.photoLink = await getPicLink();
            return View(await db.Realisations.ToListAsync());
        }
        /// <summary>
        /// gte list of he connected user's realisations
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> myRealisations()
        {
            ViewBag.photoLink = await getPicLink();
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
            var real = sp.Realisations.ToList();
            return View(real);
        }
        /// <summary>
        /// et details of a specific realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Realisations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Realisation realisation = await db.Realisations.FindAsync(id);
            if (realisation == null)
            {
                return HttpNotFound();
            }
            return View(realisation);
        }
        /// <summary>
        /// add new realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Realisations/Create
        public async Task<ActionResult> Create(int id)
        {
            ViewBag.photoLink = await getPicLink();
            Mission ms = db.Missions.Find(id);
            Realisation rl = db.Realisations.Where(x => x.Mission.ID == ms.ID).FirstOrDefault();
            if (rl != null) { 
                ViewBag.popup = "vous avez déja créer la réalisation de cette mission , Vous pouvez seulement la modifier ";
                return View("popup");
            }
            return View();
        }

        // POST: Realisations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Description,TakenTime")] Realisation realisation,int id, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var photo = new Photo
                    {
                        Name = Path.GetFileName(file.FileName),
                        Link = "~/Content/Images/" + file.FileName,
                    };
                    //   photo.Link = "~/Helpers/" + photo.Name;
                    file.SaveAs(Server.MapPath(photo.Link));
                    db.Photos.Add(photo);
                    if (realisation.Photos == null) realisation.Photos = new List<Photo>();
                    realisation.Photos.Add(photo);
                }
            }
            Mission ms = db.Missions.Find(id);
            realisation.Mission = ms;
            realisation.Client = ms.Client;
            realisation.ServiceProvider = ms.ServiceProvider;
            db.Realisations.Add(realisation);
            db.SaveChanges();
            string ss = await getPicLink();
            ViewBag.photoLink = ss;
            return RedirectToAction("Index");
        }
            catch (Exception ex) {
                return View(realisation);
    }

}
        /// <summary>
        /// Edit1 isn't used in this app but it's supposed to modify a realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Realisations/Edit/5
        public async Task<ActionResult> Edit1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission ms =await db.Missions.FindAsync(id);
            Realisation realisation = await db.Realisations.Where(r => r.Mission.ID == ms.ID).FirstOrDefaultAsync();
           // Realisation realisation = await db.Realisations.FindAsync(id);
            if (realisation == null)
            {
                return HttpNotFound();
            }
            int j = 0;
            List<string> ls = new List<string>();ls.Add(""); ls.Add(""); ls.Add(""); ls.Add("");
            foreach (var item in realisation.Photos)
            {
                ls[j] = item.Link;
                j++; 
                
            }
            int i = 0;
            for (i = 0; i < 4; i++)
            {
                if (ls[i] == "") ls[i] = "~/Content/Images/default.jpg";               
            }
            ViewBag.LS = ls;
            return View(realisation);
        }

        // POST: Realisations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit1(Realisation realisation, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                Realisation rl = db.Realisations.Where(r => r.ID == realisation.ID).FirstOrDefault();
                int i = 0;
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var photo = new Photo
                        {
                            Name = Path.GetFileName(file.FileName),
                            Link = "~/Content/Images/" + file.FileName,
                        };
                        file.SaveAs(Server.MapPath(photo.Link));
                        db.Photos.Add(photo);
                        if (rl.Photos == null) rl.Photos = new List<Photo>();
                        List<Photo> ll = new List<Photo>(4);
                        ll = rl.Photos.ToList();
                        if (i >= ll.Count())
                        {
                            ll.Add(photo);
                        }
                        ll[i] = photo;
                        rl.Photos = ll;
                    }
                    i++;
                }
                rl.TakenTime = realisation.TakenTime;
                rl.Description = realisation.Description;
                await db.SaveChangesAsync();
                return RedirectToAction("myMissions", "Business");
            }
            return View(realisation);
        }
        /// <summary>
        /// Modify a realisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.photoLink = await getPicLink();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Realisation realisation = new Realisation();
           
            //Mission ms = await db.Missions.FindAsync(id);
            // realisation = await db.Realisations.Where(r => r.Mission.ID == ms.ID).FirstOrDefaultAsync();
          
            realisation = await db.Realisations.FindAsync(id);
           
            if (realisation == null)
            {
                return HttpNotFound();
            }
            int j = 0;
            List<string> ls = new List<string>(); ls.Add(""); ls.Add(""); ls.Add(""); ls.Add("");
            foreach (var item in realisation.Photos)
            {
                ls[j] = item.Link;
                j++;

            }
            int i = 0;
            for (i = 0; i < 4; i++)
            {
                if (ls[i] == "") ls[i] = "~/Content/Images/default.jpg";
            }
            ViewBag.LS = ls;
            return View(realisation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Realisation realisation, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                Realisation rl = db.Realisations.Where(r => r.ID == realisation.ID).FirstOrDefault();
                int i = 0;
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var photo = new Photo
                        {
                            Name = Path.GetFileName(file.FileName),
                            Link = "~/Content/Images/" + file.FileName,
                        };
                        file.SaveAs(Server.MapPath(photo.Link));
                        db.Photos.Add(photo);
                        if (rl.Photos == null) rl.Photos = new List<Photo>();
                        List<Photo> ll = new List<Photo>(4);
                        ll = rl.Photos.ToList();
                        if (i >= ll.Count())
                        {
                            ll.Add(photo);
                        }
                        ll[i] = photo;
                        rl.Photos = ll;
                    }
                    i++;
                }
                rl.TakenTime = realisation.TakenTime;
                rl.Description = realisation.Description;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(realisation);
        }
            /// <summary>
            /// delete realisation
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        // GET: Realisations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Realisation realisation = await db.Realisations.FindAsync(id);
            if (realisation == null)
            {
                return HttpNotFound();
            }
            
            return View(realisation);
        }

        // POST: Realisations/Delete/5
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> deleteRealisation(int id)
        {
            Realisation realisation = await db.Realisations.FindAsync(id);
            foreach (var item in realisation.Photos.ToList())
            {
                db.Photos.Remove(item);
            }
            db.Realisations.Remove(realisation);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// free unused resources
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
    }
}
