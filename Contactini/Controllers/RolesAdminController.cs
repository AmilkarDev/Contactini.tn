using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace IdentitySample.Controllers
{
    /// <summary>
    /// roles managing and operations
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
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
            set
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
        public async Task<string> getPicLink()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return "Problème à identifier l'utilisateur connecté";
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return "Problème à identifier l'utilisateur connecté";
            }
            string ss = "";
            if (user.PhotoLink != null) ss = user.PhotoLink;
            else ss = "~/Content/Images/Unknown.png";
            return ss;
        }
        /// <summary>
        /// get list of roles
        /// </summary>
        /// <returns></returns>
        // GET: /Roles/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.photoLink = await getPicLink();
            return View(RoleManager.Roles);
        }

        /// <summary>
        /// get the details of a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Roles/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            ViewBag.photoLink = await getPicLink();
            return View(role);
        }

        /// <summary>
        /// add new role
        /// </summary>
        /// <returns></returns>
        // GET: /Roles/Create
        [HttpGet]
        public  async Task<ActionResult> Create()
        {
            ViewBag.photoLink = await getPicLink();
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name);
                role.Description = roleViewModel.Description;
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        /// <summary>
        /// Modify a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Roles/Edit/Admin
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            // Update the new Description property for the ViewModel:
            roleModel.Description = role.Description;
            ViewBag.photoLink = await getPicLink();
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id,Description")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                // Update the new Description property:
                role.Description = roleModel.Description;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        /// <summary>
        /// delete a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: /Roles/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            ViewBag.photoLink = await getPicLink();
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
