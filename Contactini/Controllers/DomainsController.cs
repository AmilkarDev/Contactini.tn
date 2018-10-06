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

namespace Contactini.Controllers
{
    [Authorize(Roles =("Admin"))]
    public class DomainsController : Controller
    {
        private ContactiniContext db = new ContactiniContext();

        // GET: Domains
        public ActionResult Index()
        {
            return View(db.Domains.ToList());
        }

        // GET: Domains/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Domain domain = db.Domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }
        /// <summary>
        /// Domain create in relation with the selected sector
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Domains/Create
        [ChildActionOnly]
        public ActionResult Create(int? id)
        {
            
            
            TempData["ss"] = id;
            return View();
           
        }
       
        // POST: Domains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase upload, Domain domain)
        {
            int id = (int)TempData["ss"];
            Sector ss = db.Sectors.Where(s => s.ID == id).Single();
            if (ModelState.IsValid)
            {
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
                    Domain dm = new Domain
                    {
                        Name = domain.Name,
                        PhotoLink = photo.Link
                        
                    };
                    db.Domains.Add(dm);
                    ss.Domain.Add(dm);
                    db.SaveChanges();
                    return RedirectToAction("Index","Sectors", new { id = id });
                }


                return View();
            }

            return View(domain);
        }
        
      
       
        // GET: Domains/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Domain domain = db.Domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        // POST: Domains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Domain domain, HttpPostedFileBase uploadd)
        {
            if (ModelState.IsValid)
            {
                Domain dom = db.Domains.Find(domain.domainId);
                if (uploadd != null && uploadd.ContentLength > 0)
                {
                    var photo = new Photo
                    {
                        Name = System.IO.Path.GetFileName(uploadd.FileName),
                        Link = "~/Content/Images/" + uploadd.FileName,

                    };
                    //   photo.Link = "~/Helpers/" + photo.Name;
                    uploadd.SaveAs(Server.MapPath(photo.Link));
                    db.Photos.Add(photo);
                    dom.PhotoLink = photo.Link;
                  
                }
                dom.Name = domain.Name;
               // int id = db.Sectors.Where(s => s.Domain.Contains(domain)).FirstOrDefault().ID;
               // db.Entry(domain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Sectors");


            }
            return View(domain);
        }

        // GET: Domains/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Domain domain = db.Domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        // POST: Domains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Domain domain = db.Domains.Find(id);
            db.Domains.Remove(domain);
            db.SaveChanges();
            return RedirectToAction("Index","Sectors");
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
