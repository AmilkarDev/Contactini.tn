using Contactini.DAL;
using Contactini.Models.Entities;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    //Authorize : Proporiété qui assure que l'accés est garantie seulement au personnes identifiées !
    [Authorize]
    public class AccountController : Controller
    {
        private ContactiniContext db = new ContactiniContext();
        public AccountController()
        {
        }
        // Application User manager : one of the Identity Models that gives us control over the ApplicationUser already created by Identity Framework
        //ApplicationRole manager : same description as above just this one helps managing role model
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        //That is the method for uploading user profile picture
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddPhoto()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddPhoto(HttpPostedFileBase upload)
        {
            if (upload != null && upload.ContentLength > 0)
            {
                var photo = new Photo
                {
                    Name = System.IO.Path.GetFileName(upload.FileName),
                    Link = "~/Helpers/" + upload.FileName,

                };
                //   photo.Link = "~/Images/" + photo.Name;
                upload.SaveAs(Server.MapPath(photo.Link));
                db.Photos.Add(photo);

                db.SaveChanges();
                return PartialView("Up");
            }
            return View();
        }
        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult UploadFiles()
        //{
        //    // Checking no of files injected in Request object  
        //    if (Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            //  Get all files from Request object  
        //            HttpFileCollectionBase files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
        //                //string filename = Path.GetFileName(Request.Files[i].FileName);  

        //                HttpPostedFileBase file = files[i];
        //                string fname;

        //                // Checking for Internet Explorer  
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }

        //                // Get the complete folder path and store the file inside it.  
        //                /*fname = Path.Combine(Server.MapPath("~/Content/Images/"), fname);*/
        //                var photo = new Photo
        //                {
        //                    Name = System.IO.Path.GetFileName(file.FileName),
        //                    Link = "~/Content/Images/" + file.FileName,

        //                };
        //                file.SaveAs(Server.MapPath(photo.Link));
        //                return Json(photo.Link);
        //                //file.SaveAs(fname);
        //            }
        //            // Returns message that successfully uploaded  
        //            return Json("File Uploaded Successfully!");
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //}
        /// <summary>
        /// Shows the profile pic of the user after finding it 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string showPhoto(string id )
        {
            // get EF Database (maybe different way in your applicaiton)
            var db = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            // find the user. I am skipping validations and other checks.
            var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user.PhotoLink != null) return user.PhotoLink;
            else return "~/Content/Images/Uknown.png";
        }
        //
        // GET: /Account/Login

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            /* next three lines were added tonight ,fix for the login already issue*/
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Redirect");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        /// <summary>
        /// ApplicationSignInManager is what gives us control over the way the user Login to our application
        /// </summary>
        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    // return RedirectToLocal(returnUrl);
                    return RedirectToAction("Index", "Redirect");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        /// <summary>
        /// verifyCode method is used when the user have to receive a code via email or phone that will give him access to his account 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model  , params string[] Spr)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                // Add the Address properties:
                user.FullName = model.FullName;
                user.StreetAddress = model.StreetAddress;
                user.City = model.City;
                user.State = model.State;
                user.Country = model.Country;
                user.PostalCode = model.PostalCode;
                user.PhotoLink = model.PhotoLink;
                

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (Spr != null)
                    {
                        var resultt = await UserManager.AddToRoleAsync(user.Id, "ServiceProvider");
                        var result1 = await UserManager.AddToRoleAsync(user.Id, "Client");
                        if (resultt.Succeeded && result1.Succeeded)
                        {
                            Address ad = new Address { StreetAddress = user.StreetAddress, City = user.City, State = user.State, PostalCode = user.PostalCode , Country=user.Country  };
                            //Creating DbClient and DbServiceProvider Process
                            Client cl = new Client
                            {
                                FullName = user.FullName,
                                UserName =user.UserName,
                                Email = user.Email,
                                PhoneNum = user.PhoneNumber,
                                Address =ad
                                
                            };
                            db.Clients.Add(cl);
                            db.SaveChanges();
                            ServiceProvider sp = new ServiceProvider
                            {
                                FullName = user.FullName,
                                Email = user.Email,
                                UserName = user.UserName,
                                PhoneNum = user.PhoneNumber,
                                Address = ad,
                                photoLink = user.PhotoLink,
                                Titre = "Titre pas encore déterminé",
                                Description = "Nouveau Prestataire"
                            };
                            // On voulais affecté une domaine et une secteur par défaut a chaque nouveau prestatatir ( c obligatoire pour
                            //évité des exception non traité au futur
                            Domain ain = db.Domains.Where(d => d.Name == "aucun").FirstOrDefault();
                            if (ain == null)
                            {
                                ain = new Domain { Name = "aucun", PhotoLink = "~/Content/Images/dm.jpg" }; db.Domains.Add(ain); db.SaveChanges();
                            }
                            Sector tor = db.Sectors.Where(s => s.Name == "aucun").FirstOrDefault();
                            if (tor == null)
                            {
                                tor = new Sector { Name = "aucun" }; tor.Domain = new List<Domain>(); tor.Domain.Add(ain); db.Sectors.Add(tor); db.SaveChanges();
                            }
                            sp.Domain = ain;
                            sp.sector = tor;
                            // Affectation terminé

                            db.ServiceProviders.Add(sp);
                            db.SaveChanges();
                            //Email Confirmation Process
                            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                            ViewBag.Link = callbackUrl;
                            return View("DisplayEmail");
                        }
                    }
                    else
                    {
                        var resultt = await UserManager.AddToRoleAsync(user.Id, "Client");
                        Address ad = new Address { StreetAddress = user.StreetAddress, City = user.City, State = user.State, PostalCode = user.PostalCode, Country = user.Country };
                        //Creating DbClient and DbServiceProvider Process
                        Client cl = new Client
                        {
                            FullName = user.FullName,
                            UserName = user.UserName,
                            Email = user.Email,
                            PhoneNum = user.PhoneNumber,
                            Address = ad
                        };
                        db.Clients.Add(cl);
                        db.SaveChanges();
                        if (resultt.Succeeded)
                        {
                            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                            ViewBag.Link = callbackUrl;
                            return View("DisplayEmail");
                        }
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
       
        /// <summary>
        /// Confirmation d'email , dans notre situation ca sera appliqué par la génération d'un email de confirmation
        /// mais normalement on utilise une plateforme d'échange d'email telle que SendGrid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// Method of password recovery when it's lost and the user can't log In 
        /// </summary>
        /// <returns></returns>
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
           // return RedirectToAction("Index", "Redirect");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}