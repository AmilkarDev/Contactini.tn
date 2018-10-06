using Contactini.DAL;
using Contactini.Models;
using Contactini.Models.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Controllers
{
    public class myCVController : Controller
    {
        /// <summary>
        /// controller providing all operations related to the SP cv 
        /// the non commented methods are self explanatory by their name
        /// </summary>
        public myCVController() { }
        private ContactiniContext db = new ContactiniContext();
        public myCVController(ApplicationUserManager userManager)
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
        /// show the private version of the connected sp's cv where he could update /delete/add...
        /// </summary>
        /// <returns></returns>
        // GET: myCV
        public async Task<ActionResult> Index()
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
            Session["email"] = emailAddress;
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == emailAddress).FirstOrDefault();
            return View(sp);
        }
        /// <summary>
        /// shows the public version of cv so to get printed
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async  Task<ActionResult> Findex(string email)
        {
            ViewBag.photoLink = await getPicLink();
            Session["email"] = email;
            ServiceProvider sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            ViewBag.photoLinkp = sp.photoLink;
            return View(sp);
        }
        [HttpGet]
        [ChildActionOnly]
        public ActionResult Education()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            
            return View(sp.Educations);
        }
        [HttpGet]
        [ChildActionOnly]
        public ActionResult EduCV()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();

            return View(sp.Educations);
        }
        [HttpGet]
        public async Task<ActionResult> EduCreate()
        {
            ViewBag.photoLink = await getPicLink();
            return View();
        }
        [HttpPost]
        public ActionResult EduCreate(Education edu)
        {
            if (ModelState.IsValid)
            {
                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                edu.ServiceProvider = sp;
                sp.Educations.Add(edu);
                db.Educations.Add(edu);
                db.SaveChanges();
               return RedirectToAction("Index");
            }
            
            return View(edu);
        }

        [HttpGet]
        public async Task<ActionResult> EduUpdate(int id)
        {
            ViewBag.photoLink = await getPicLink();
            Education edu = db.Educations.Find(id);
            return View(edu);
        }
        [HttpPost]
        public ActionResult EduUpdate(Education edu)
        {
            Education ed = db.Educations.Where(e => e.ID == edu.ID).FirstOrDefault();
            ed.City = edu.City;
            ed.Country = edu.Country;
            ed.Degree = edu.Degree;
            ed.FromYear = edu.FromYear;
            ed.ToYear = edu.ToYear;
            ed.TitleOfDiploma = edu.TitleOfDiploma;
            ed.InstituteUniversity = edu.InstituteUniversity;

            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            Education edd = sp.Educations.Where(e => e.ID == edu.ID).FirstOrDefault();
            edd.City = edu.City;
            edd.Country = edu.Country;
            edd.Degree = edu.Degree;
            edd.FromYear = edu.FromYear;
            edd.ToYear = edu.ToYear;
            edd.TitleOfDiploma = edu.TitleOfDiploma;
            edd.InstituteUniversity = edu.InstituteUniversity;

            db.SaveChanges();
            return RedirectToAction("Index");
        }


     
        public ActionResult EduDelete(int id)
        {
            Education ed = db.Educations.Find(id);
            db.Educations.Remove(ed);
           
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [ChildActionOnly]
        public ActionResult Experience()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.workExperiences);
        }
        [ChildActionOnly]
        public ActionResult ExpCV()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.workExperiences);
        }
        [HttpGet]
        public async Task<ActionResult> ExpCreate()
        {
            ViewBag.photoLink = await getPicLink();
            return View();
        }
        [HttpPost]
        public ActionResult ExpCreate(workExperience exp)
        {
            if (ModelState.IsValid)
            {
                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                exp.ServiceProvider = sp;
                sp.workExperiences.Add(exp);
                db.WorkExperiences.Add(exp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exp);
        }

        [HttpGet]
        public async Task<ActionResult> ExpUpdate(int id)
        {
            ViewBag.photoLink = await getPicLink();
            workExperience exp = db.WorkExperiences.Find(id);
            return View(exp);
        }
        [HttpPost]
        public ActionResult ExpUpdate(workExperience exp)
        {
            workExperience wp = db.WorkExperiences.Where(w => w.ID == exp.ID).FirstOrDefault();
            wp.Company = exp.Company;
            wp.Country = exp.Country;
            wp.Description = exp.Description;
            wp.FromYear = exp.FromYear;
            wp.ToYear = exp.ToYear;
            wp.Title = exp.Title;


            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            workExperience wx = sp.workExperiences.Where(e => e.ID == exp.ID).FirstOrDefault();
            wx.Company = exp.Company;
            wx.Country = exp.Country;
            wx.Description = exp.Description;
            wx.FromYear = exp.FromYear;
            wx.ToYear = exp.ToYear;
            wx.Title = exp.Title;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        public ActionResult ExpDelete(int id)
        {
            workExperience wp = db.WorkExperiences.Find(id);
            db.WorkExperiences.Remove(wp);
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult Skills()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.Skills);
        }
        [ChildActionOnly]
        public ActionResult SkiCV()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.Skills);
        }
        [HttpGet]
        public async Task<ActionResult> SkiCreate()
        {
            ViewBag.photoLink = await getPicLink();
            return View();
        }
        [HttpPost]
        public ActionResult SkiCreate(Skill ski)
        {
            //if (ModelState.IsValid)
            //{
                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                ski.ServiceProvider = sp;
                sp.Skills.Add(ski);
                db.Skills.Add(ski);
                db.SaveChanges();
                return RedirectToAction("Index");
            //}
            //return View(ski);
        }

        [HttpGet]
        public async Task<ActionResult> SkiUpdate(int Id)
        {
            ViewBag.photoLink = await getPicLink();
            Skill sk = db.Skills.Find(Id);
            return View(sk);
        }
        [HttpPost]
        public ActionResult SkiUpdate(Skill ski)
        {
            try
            {
                Skill sk = db.Skills.Where(s => s.ID == ski.ID).FirstOrDefault();
                sk.SkillName = ski.SkillName;
                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                Skill ss = sp.Skills.Where(e => e.ID == ski.ID).FirstOrDefault();
                ss.SkillName = ski.SkillName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(ski);
            }
        }

        
        public ActionResult SkiDelete(int id)
        {
            Skill sk = db.Skills.Find(id);
            db.Skills.Remove(sk);
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult Certificates()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.Certifications);
        }
        [ChildActionOnly]
        public ActionResult CertCV()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.Certifications);
        }

        [HttpGet]
        public async Task<ActionResult> CertCreate()
        {
            List<string> ls = new List<string>(); ls.Add("Débutant"); ls.Add("Intermediaire"); ls.Add("Avance");
            ViewBag.Levels = new SelectList(ls);
            ViewBag.photoLink = await getPicLink();
            return View();
        }

        [HttpPost]
        public ActionResult CertCreate(Certification cert, FormCollection form)
        {

            try
            {
              //  string certifLevel = form["Levels"].ToString();
                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                cert.ServiceProvider = sp;
               // cert.LevelCertification = certifLevel;
                sp.Certifications.Add(cert);
                db.Certifications.Add(cert);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(cert);

            }
        }

        [HttpGet]
        public async Task<ActionResult> CertUpdate(int id)
        {
            Certification cert = db.Certifications.Find(id);
            ViewBag.photoLink = await getPicLink();
            return View(cert);
        }
        [HttpPost]
        public ActionResult CertUpdate(Certification cc)
        {
            try
            {
                Certification cer = db.Certifications.Where(c => c.ID == cc.ID).FirstOrDefault();
                cer.CertificationAuthority = cc.CertificationAuthority;
                cer.CertificationName = cc.CertificationName;
                cer.Level = cc.Level;
                cer.FromYear = cc.FromYear;

                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                Certification ss = sp.Certifications.Where(e => e.ID == cc.ID).FirstOrDefault();
                ss.CertificationAuthority = cc.CertificationAuthority;
                ss.CertificationName = cc.CertificationName;
                ss.Level = cc.Level;
                ss.FromYear = cc.FromYear;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(cc);
            }



        }

        public ActionResult CertDelete(int id)
        {
            Certification cert = db.Certifications.Find(id);
            db.Certifications.Remove(cert);
           
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult Langues()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.Languages);
        }
        /// <summary>
        /// show languages on the final cv version
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult LangCV()
        {
            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            return View(sp.Languages);
        }
        /// <summary>
        /// add a new language
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> LangCreate()
        {
            ViewBag.photoLink = await getPicLink();
            List<string> ls = new List<string>(); ls.Add("Débutant"); ls.Add("Intermediaire"); ls.Add("Avance");
            ViewBag.Levels = new SelectList(ls);
            return View();
        }
        [HttpPost]
        public ActionResult LangCreate(Language ll)
        {
            try
            {
                //string langLevel = form["Levels"].ToString();
                string email = (string)Session["email"];
                ServiceProvider sp = new ServiceProvider();
                sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
                ll.ServiceProvider = sp;     
                sp.Languages.Add(ll);
                db.Languages.Add(ll);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(ll);
            }

        }
        /// <summary>
        /// uodate infos about a language
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> LangUpdate(int Id)
        {
            ViewBag.photoLink = await getPicLink();
            Language la = db.Languages.Find(Id);
            return View(la);
        }
        [HttpPost]
        public ActionResult LangUpdate(Language la)
        {
            try { 
            Language ll = db.Languages.Where(l => l.ID == la.ID).FirstOrDefault();
            ll.LangProficiency = la.LangProficiency;
            ll.LanguageName = la.LanguageName;

            string email = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            sp = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            Language ls = sp.Languages.Where(e => e.ID == la.ID).FirstOrDefault();
            ls.LangProficiency = la.LangProficiency;
            ls.LanguageName = la.LanguageName;
                db.SaveChanges();
            return RedirectToAction("Index");
        }
            catch
            {
                return View(la);
            }
        }
        ¨/// <summary>
        /// delete a language
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LangDelete(int id)
        {
            Language la = db.Languages.Find(id);
            db.Languages.Remove(la);
            
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        /// <summary>
        /// Personal info update of the connected SP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async  Task<ActionResult> UpdatePersInfo()
        {
            string email = (string)Session["email"];
            var user = UserManager.Users.Where(u => u.Email == email).FirstOrDefault();
            ServiceProvider spr = new ServiceProvider();
            spr = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            ViewBag.photoLink = await getPicLink();
            return View(spr);
        }
        [HttpPost]
        public async Task<ActionResult> UpdatePersInfo(ServiceProvider sp)
        {
            string email = (string)Session["email"];
            var user = UserManager.Users.Where(u => u.Email == email).FirstOrDefault();
            ServiceProvider spr = new ServiceProvider();
            spr = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            //if (ModelState.IsValid)
            //{
                //spr.Address = sp.Address;
                spr.DateOfBirth = sp.DateOfBirth;
                spr.Titre = sp.Titre;
                spr.Diponibility = sp.Diponibility;
                spr.EducationalLevel = sp.EducationalLevel;
                spr.Email = sp.Email;
                spr.FacebookProfil = sp.FacebookProfil;
                spr.TwitterProfil = sp.TwitterProfil;
                spr.UserName = sp.UserName;
                spr.LinkedInProfil = sp.LinkedInProfil;
                spr.PhoneNum = sp.PhoneNum;
                spr.Stars = sp.Stars;
                spr.FullName = sp.FullName;
                spr.HasDrivingLicence = sp.HasDrivingLicence;
                spr.HasPassport = sp.HasPassport;
                spr.HasACar = sp.HasACar;
                db.SaveChanges();

                //user.StreetAddress = sp.Address.StreetAddress;
                //user.City = sp.Address.City;
                //user.State = sp.Address.State;
                //user.Country = sp.Address.Country;
                //user.PostalCode = sp.Address.PostalCode;

                user.Email = sp.Email;
                user.UserName = sp.UserName;
                user.FullName = sp.FullName;
                user.PhoneNumber = sp.PhoneNum;
                var rs = await UserManager.UpdateAsync(user);
                if (!rs.Succeeded)
                {
                    ModelState.AddModelError("", rs.Errors.First());
                    return View();
                }

            //}

            return RedirectToAction("Index");
        }
        /// <summary>
        /// Description update of SP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DescUpdate()
        {
            List<SelectListItem> domainList = new List<SelectListItem>();
            List<SelectListItem> sectorList = new List<SelectListItem>();
            

            
            string ss = await getPicLink();
            ViewBag.photoLink = ss;

            string email = (string)Session["email"];
            ServiceProvider spr = new ServiceProvider();
            spr = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            SpViewModel svm = new SpViewModel();
            svm.Description = spr.Description;
            svm.domain = spr.Domain;
            svm.sector = spr.sector;
            //foreach (var item in db.Domains.ToList()) domainList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            //foreach (var item in db.Sectors.ToList()) sectorList.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            var dom = db.Domains.ToList();
            var sec = db.Sectors.ToList();
            if(svm.domain==null || svm.sector==null)
            {
                
                Domain ain = db.Domains.Where(d => d.Name == "aucun").FirstOrDefault();
                if (ain == null)
                {
                    ain = new Domain { Name = "aucun", PhotoLink = "~/Content/Images/dm.jpg" }; db.Domains.Add(ain); db.SaveChanges();
                }
                Sector tor = db.Sectors.Where(s => s.Name == "aucun").FirstOrDefault();
                if (tor == null)
                {
                    tor = new Sector { Name = "aucun"};
                    tor.Domain = new List<Domain>();
                    tor.Domain.Add(ain) ;
                    db.Sectors.Add(tor);
                    db.SaveChanges();
                }
                svm.domain = ain;
                svm.sector = tor;
            }
            
            ViewBag.Domains = new SelectList(dom, "Name", "Name", svm.domain.Name);                                              //(IEnumerable<SelectListItem>)domainList;
            ViewBag.Sectors = new SelectList(sec, "Name", "Name", svm.sector.Name);                                             //(IEnumerable<SelectListItem>)sectorList;
            
            return View(svm);
           
        }
        [HttpPost]
        public async Task<ActionResult> DescUpdate(SpViewModel vm,FormCollection form)
        {
            string email = (string)Session["email"];
            var user = UserManager.Users.Where(u => u.Email == email).FirstOrDefault();
            ServiceProvider spr = new ServiceProvider();
            spr = db.ServiceProviders.Where(s => s.Email == email).FirstOrDefault();
            //string sector = form["Sectors"].ToString();
            //string domain = form["Domains"].ToString();
            Domain dd = db.Domains.Where(x => x.Name == vm.domain.Name).FirstOrDefault();
            Sector sr = db.Sectors.Single(x => x.Name == vm.sector.Name);
            spr.Domain = dd;
            spr.sector = sr;
            spr.Description = vm.Description;
            db.SaveChanges();
            string ss = await getPicLink();
            ViewBag.photoLink = ss;
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Print the cv Operation
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintCV()
        {
            string emailAddress = (string)Session["email"];
            ServiceProvider sp = new ServiceProvider();
            var cv = new ActionAsPdf("Findex",new {email=emailAddress });
            return cv;
        }
        /// <summary>
        /// get list of domains depending on the selected sector
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
            return Json(dvm,JsonRequestBehavior.AllowGet);
        }
    }
}