using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Contactini.Models.Entities;
using Contactini.DAL;
using Contactini.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace IdentitySample.Controllers
{
    // Controller responsible for operations on users profiles exactly on the ApplicationUserModel
   // [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
        }
        ContactiniContext ctx = new ContactiniContext();
        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }
        // Our way to access the AplicationUser Model and apply modifications ( delete , create , edit ...)
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
        /// Show all the users of our application
        /// </summary>
        /// <returns></returns>
        // GET: /Users/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.photoLink = await getPicLink();
            return View(await UserManager.Users.ToListAsync());
        }

        /// <summary>
        /// Afficher les détails d'un utilisateur spécifique
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            ViewBag.photoLink = await getPicLink();
            return View(user);
        }

        /// <summary>
        /// The next method provide the process to create a user  manually ! but in this app we're not using it , since evry user should go through the registration process 
        /// specified in the account controller in order to have an account
        /// </summary>
        /// <returns></returns>
        // GET: /Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            if(User.IsInRole("Admin"))
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.photoLink = await getPicLink();
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = userViewModel.Email,
                    FullName = userViewModel.FullName,
                    Email = userViewModel.Email,
                    // Add the Address Info:
                    StreetAddress = userViewModel.StreetAddress,
                    City = userViewModel.City,
                    State = userViewModel.State,
                    PostalCode = userViewModel.PostalCode
                };
                // Add the Address Info:
                //user.StreetAddress = userViewModel.StreetAddress;
                //user.City = userViewModel.City;
                //user.State = userViewModel.State;
                //user.PostalCode = userViewModel.PostalCode;
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        /// <summary>
        /// Edit is the method the admin use in order to modify a user access level , so to give him new access or remove his access to some
        /// space of the app ( for example allow admin access and remove Service provider Access)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            ViewBag.photoLink = await getPicLink();
            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                // Include the Addresss info:
                UserName = user.UserName,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                PhotoLink = user.PhotoLink,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,FullName,UserName,Id,StreetAddress,City,State,PostalCode,photoLink")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.FullName = editUser.FullName;
                user.UserName = editUser.UserName;
                user.Email = editUser.Email;
                user.StreetAddress = editUser.StreetAddress;
                user.City = editUser.City;
                user.State = editUser.State;
                user.PostalCode = editUser.PostalCode;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }
        /// <summary>
        /// This is the metod used by any user tp update his profile data ( fullsName , username ,address , profile pic....)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> _Edit (string id,string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl)
                 && Request.UrlReferrer != null
                 && Request.UrlReferrer.ToString().Length > 0)
            {
                return RedirectToAction("_Edit", new { returnUrl = Request.UrlReferrer.ToString() });
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
          
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            ViewBag.photoLink = await getPicLink();
            Session["retour"] = Request.UrlReferrer;
            EditUserViewModel ed = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                // Include the Addresss info:
                UserName = user.UserName,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                PhotoLink = user.PhotoLink,
                PhoneNumber = user.PhoneNumber,
                Country =user.Country,
                
            };
            if (userRoles.Contains("ServiceProvider")) {
                ed.isServiceProvider = true; TempData["sp"] = "SP";
            }
            
            else
            {
                ed.isServiceProvider = false; TempData["sp"] = "nonSP";
            }
             return View(ed);
           // return RedirectToAction("_Edit", new { returnUrl = Request.UrlReferrer.ToString() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Edit([Bind(Include = "Email,FullName,UserName,Id,StreetAddress,City,State,Country,PostalCode,PhoneNumber,photoLink,isServiceProvider")] EditUserViewModel editUser,string returnUrl, params string[] Spr)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                ServiceProvider ss = ctx.ServiceProviders.Where(s => s.Email == user.Email).FirstOrDefault();
                Client cc = ctx.Clients.Where(c => c.Email == user.Email).FirstOrDefault();
                if(ss!=null) {
                    ss.Email = editUser.Email;
                    ss.PhoneNum = editUser.PhoneNumber;
                    ss.UserName = editUser.UserName;
                    ss.FullName = editUser.FullName;
                    ss.photoLink = editUser.PhotoLink;
                    ss.Address.StreetAddress = editUser.StreetAddress;
                    ss.Address.State = editUser.State;
                    ss.Address.Country = editUser.Country;
                    ss.Address.City = editUser.City;
                    ss.Address.PostalCode = editUser.PostalCode;
                    ss.photoLink = editUser.PhotoLink;
                    ctx.SaveChanges();
                }
                if(cc!=null) {
                    cc.Email = editUser.Email;
                    cc.UserName = editUser.UserName;
                    cc.PhoneNum = editUser.PhoneNumber;
                    cc.Address.StreetAddress = editUser.StreetAddress;
                    cc.Address.City = editUser.City;
                    cc.Address.State = editUser.State;
                    cc.Address.PostalCode = editUser.PostalCode;
                    cc.Address.Country = editUser.Country;
                    cc.FullName = editUser.FullName;
                    
                    ctx.SaveChanges();
                }
                if (cc == null)
                {
                    cc = new Client();
                    cc.Email = editUser.Email;
                    cc.UserName = editUser.UserName;
                    cc.PhoneNum = editUser.PhoneNumber;
                    cc.Address = new Address();
                    cc.Address.StreetAddress = editUser.StreetAddress;
                    cc.Address.City = editUser.City;
                    cc.Address.State = editUser.State;
                    cc.Address.PostalCode = editUser.PostalCode;
                    cc.Address.Country = editUser.Country;
                    cc.FullName = editUser.FullName;
                    ctx.Clients.Add(cc);
                    ctx.SaveChanges();
                }
                user.FullName = editUser.FullName;
                user.UserName = editUser.UserName;
                user.Email = editUser.Email;
                user.StreetAddress = editUser.StreetAddress;
                user.City = editUser.City;
                user.State = editUser.State;
                user.Country = editUser.Country;
                user.PostalCode = editUser.PostalCode;
                user.PhoneNumber = editUser.PhoneNumber;
                user.PhotoLink = editUser.PhotoLink;
                string retour = Session["retour"].ToString(); ;
                if (editUser.isServiceProvider && ((string)TempData["sp"] == "nonSP"))
                {
                    var result = await UserManager.AddToRoleAsync(user.Id, "ServiceProvider");
                   
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    else
                    {
                        ServiceProvider sp = new ServiceProvider
                        {
                            Email = user.Email,
                            FullName = user.FullName,
                            PhoneNum = user.PhoneNumber,
                            UserName = user.UserName,
                            Stars = 0,
                            photoLink = user.PhotoLink,
                            Titre = "Titre pas encore déterminé",
                            Description="New Service Provider"
                        };
                        Address ad = new Address
                        {
                            StreetAddress = user.StreetAddress,
                            City = user.City,
                            State = user.State,
                            Country = user.Country,
                            PostalCode = user.PostalCode
                        };

                        // On voulais affecté une domaine et une secteur par défaut a chaque nouveau prestatatir ( c obligatoire pour
                        //évité des exception non traité au futur
                        Domain ain = ctx.Domains.Where(d => d.Name == "aucun").FirstOrDefault();
                        if (ain == null)
                        {
                            ain = new Domain { Name = "aucun", PhotoLink = "~/Content/Images/dm.jpg" }; ctx.Domains.Add(ain); ctx.SaveChanges();
                        }
                        Sector tor = ctx.Sectors.Where(s => s.Name == "aucun").FirstOrDefault();
                        if (tor == null)
                        {
                            tor = new Sector { Name = "aucun" }; tor.Domain.Add(ain); ctx.Sectors.Add(tor); ctx.SaveChanges();
                        }
                        sp.Domain = ain;
                        sp.sector = tor;



                        sp.Address = ad;
                        ctx.ServiceProviders.Add(sp);
                        ctx.Addresses.Add(ad);
                        ctx.SaveChanges();
                    }
                }
                if(!editUser.isServiceProvider && ((string)TempData["sp"] == "SP"))
                {
                    var result = await UserManager.RemoveFromRoleAsync(user.Id, "ServiceProvider");
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    // if he removed himself from the serviceProvider role , we have to return him to client reception not beck from he came
                    // because guess he's coming from monCV , back to previous page will still direct him to SP space where he doesn't belong anymore
                    /* The problem now is when he trick the app by typing the link Business/mycv wils till work since we didn't delete him from SP list , 
                     anyway we will get back to that !*/

                    //else
                    //{
                    //    ServiceProvider sp = ctx.ServiceProviders.Where(x => x.Email == editUser.Email).FirstOrDefault();
                    //    ctx.ServiceProviders.Remove(sp);
                    //    ctx.SaveChanges();
                    //}
                }
                var rs = await UserManager.UpdateAsync(user);
                if (!rs.Succeeded)
                {
                    ModelState.AddModelError("", rs.Errors.First());
                    return View();
                }
                if (!String.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index","Missions");


                //HttpContext.GetOwinContext().Get<ApplicationDbContext>().SaveChanges();
                //return RedirectToAction("Index","Missions");
               // return Redirect(Request.UrlReferrer.ToString());
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }
        /// <summary>
        /// Method used for deleting users from the plateforme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.photoLink = await getPicLink();
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        /// <summary>
        /// Used to show the statistics from our db , in the nex method we are showing number of applications(candidature) by service provider
        /// Each service provider how many applicatioàns he had 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Statistics()
        {
          
            List<PrestabyMission> lis = new List<PrestabyMission>();
           foreach (var item in ctx.ServiceProviders.ToList())
            {
                PrestabyMission pbm = new PrestabyMission
                {
                    prestaName = item.FullName,
                    MissionsCount = item.Candidatures.Count()
                };
                lis.Add(pbm);
            }


            ViewBag.photoLink = await getPicLink();
            
            ViewBag.list = JsonConvert.SerializeObject(lis);
            return View();
        }
        /// <summary>
        /// Used to show the statistics from our db , in the next method we are showing number of missions by Domain
        /// Each domain, how many missions are there to be done 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Sta2()
        {
            List<missionByDomain> lisMBD = new List<missionByDomain>();
            foreach (var item in ctx.Domains.ToList())
            {
                missionByDomain mbd = new missionByDomain
                {
                    domainName = item.Name,
                    MissionsCount = item.Missions.Count()
                };
                lisMBD.Add(mbd);
            }


            ViewBag.photoLink = await getPicLink();

            ViewBag.list = JsonConvert.SerializeObject(lisMBD);
            return View();
        }
        /// <summary>
        /// Used to show the statistics from our db , in the next method we are showing number of missions by state
        /// Each state/governerate, how many missions are there to be done 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Sta1()
        {
            ViewBag.photoLink = await getPicLink();
            var data = (from Missions in ctx.Missions
                          group Missions by Missions.Address.State.Trim().ToLower() into stateMission
                          select new MissionsbyState()
                          {
                              stateName = stateMission.Key.ToString(),
                              MissionsCount = stateMission.Count()
                          }).ToList();
            ViewBag.lista = JsonConvert.SerializeObject(data);
            return View();
        }
        /// <summary>
        /// next method is called almost by every method in the app and implemented by all the controllers because it provides us with the
        /// profile picture of the current user that we need in order to show it on the menu next to his username
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

    }
}
