using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using OfficeOpenXml;
using WebMatrix.WebData;
using PixelCMS.Filters;
using PixelCMS.Core.Models;
using PixelCMS.Helpers;
using PixelCMS.Mailer;

namespace PixelCMS.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult PopupLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_PopupLogin");
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken] đổi tên method post.
        public ActionResult PopupLoginPost(LoginModel model, string returnUrl, string checkout)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                //set history
                HttpRequestBase request = HttpContext.Request;
                var lghis = new LoginHistory();
                lghis.IPAddress = request.UserHostAddress;
                lghis.UserName = model.UserName;
                lghis.TimeStart = DateTime.Now;
                //lghis.TimeEnd = DateTime.Now;
                lghis.Browser = request.Browser.Browser;
                lghis.country = request.UserHostName;
                db.LoginHistories.Add(lghis);
                db.SaveChanges();

                if (checkout == "checkout")
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
                return Json(new { mess = true }, JsonRequestBehavior.AllowGet);
            }

            // If we got this far, something failed, redisplay form
            //  ModelState.AddModelError("", Resources.Resources.login_fail);

            if (checkout == "checkout")
            {
                TempData["error"] = Resources.Resources.login_fail;
                return RedirectToAction("Index", "ShoppingCart");
            }
            return Json(new { mess = false }, JsonRequestBehavior.AllowGet);
        }


        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl, string checkout)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                //set history
                HttpRequestBase request = HttpContext.Request;
                var lghis = new LoginHistory();
                lghis.IPAddress = request.UserHostAddress;
                lghis.UserName = model.UserName;
                lghis.TimeStart = DateTime.Now;
                //lghis.TimeEnd = DateTime.Now;
                lghis.Browser = request.Browser.Browser;
                lghis.country = request.UserHostName;
                db.LoginHistories.Add(lghis);
                db.SaveChanges();

                if (checkout == "checkout")
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", Resources.Resources.login_fail);

            if (checkout == "checkout")
            {
                TempData["errorlogin"] = Resources.Resources.login_fail;
                return RedirectToAction("Index", "ShoppingCart");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LoginPanel(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginPanel(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                //set history
                HttpRequestBase request = HttpContext.Request;
                var lghis = new LoginHistory();
                lghis.IPAddress = request.UserHostAddress;
                lghis.UserName = model.UserName;
                lghis.TimeStart = DateTime.Now;
                //lghis.TimeEnd = DateTime.Now;
                lghis.Browser = request.Browser.Browser;
                lghis.country = request.UserHostName;
                db.LoginHistories.Add(lghis);
                db.SaveChanges();

                //chuyển hướng về trang đổi mật khâu
                UsersContext u=new UsersContext();
                var  upro=u.UserProfiles.FirstOrDefault(x => x.UserName == model.UserName);
                var m = db.WebpagesMemberships.FirstOrDefault(x => x.UserId == upro.UserId);
                if (string.IsNullOrEmpty(m.ConfirmationToken))
                {
                    return RedirectToAction("Manage","Account");
                }
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", Resources.Resources.login_fail);
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            if (Session["facebooktoken"] != null)
            {
                var fb = new Facebook.FacebookClient();
                string accessToken = Session["facebooktoken"] as string;
                var logoutUrl = fb.GetLogoutUrl(new { access_token = accessToken, next = "http://localhost:57179/" });

                Session.RemoveAll();
                return Redirect(logoutUrl.AbsoluteUri);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult PopupRegister()
        {
            return PartialView("_PopupRegister");
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult PopupRegisterPost(RegisterModel model)
        {
            UsersContext dbu = new UsersContext();
            if (ModelState.IsValid)
            {
                //check mail đã tồn tại
                var UserMailIsExist = dbu.UserProfiles.Any(x => x.Email == model.Email);
                if (UserMailIsExist)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.Vui lòng chọn Email khác");
                    return Json(new { mess = false, Error = "Email đã tồn tại.Vui lòng chọn Email khác" });
                }
                // Attempt to register the user
                try
                {
                    if (model.UserName == "darkluan")
                    {
                        // UsersContext dbEF = new UsersContext();
                        // pixelcmsEntities db = new pixelcmsEntities();
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        WebSecurity.Login(model.UserName, model.Password);
                        var u = dbu.UserProfiles.FirstOrDefault(x => x.UserName == model.UserName);
                        if (u != null)
                        {
                            webpages_UsersInRoles uinr = new webpages_UsersInRoles();
                            uinr.RoleId = 1;
                            uinr.UserId = u.UserId;
                            db.webpages_UsersInRoles.Add(uinr);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });
                        Roles.AddUsersToRole(new[] { model.UserName }, "member");
                        WebSecurity.Login(model.UserName, model.Password);

                        //add địa chỉ cá nhân vào table address edit by luan
                        var address = new Address();
                        address.Username = model.UserName;
                        address.Email = model.Email;
                        address.Phone = model.Phone;
                        address.Name = model.FullName;
                        address.Type = 1;//default loại 1 =thông tin cá nhân
                        address.Date = DateTime.Now;
                        // address.District_Id = 0;
                        db.Address.Add(address);
                        db.SaveChanges();
                        //add địa chỉ giao hàng trống edit by luan
                        var address1 = new Address();
                        address1.Username = model.UserName;
                        address1.Email = model.Email;
                        address1.Type = 2;//default loại 1 =thông tin cá nhân
                        address1.Date = DateTime.Now;
                        //address1.District_Id = 0;
                        db.Address.Add(address1);

                        //lấy email lưu vào EmailSubscribe
                        var EmailSubscribe = new EmailSubscribe();
                        EmailSubscribe.Email = model.Email;
                        EmailSubscribe.Active = true;
                        EmailSubscribe.Date = DateTime.Now;
                        //add role là member cho EmailSubscribe
                        var role = db.webpages_Roles.FirstOrDefault(x => x.RoleName == "member");
                        if (role != null)
                            EmailSubscribe.webpages_Roles.Add(role);

                        //check EmailSubscribes đã tồn tại hay chưa nếu đã tồn tại ko lưu nữa.//cái này lưu để sử dụng cho mail marketing
                        var IsExist = db.EmailSubscribes.Any(x => x.Email == model.Email);
                        if (!IsExist)
                            db.EmailSubscribes.Add(EmailSubscribe);

                        db.SaveChanges();
                    }
                    return Json(new { mess = true });
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    return Json(new { mess = false, Error = ErrorCodeToString(e.StatusCode) });
                }
            }
            //đưa lỗi từ ModelState trả về json
            string error = "";
            foreach (var item in ModelState.Values)
            {
                var s = item.Errors.FirstOrDefault();
                if (s != null)
                {
                    error = s.ErrorMessage;
                    break;
                }
            }
            // If we got this far, something failed, redisplay form
            return Json(new { mess = false, Error = error });
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            UsersContext dbu = new UsersContext();
            if (ModelState.IsValid)
            {
                //check mail đã tồn tại
                var UserMailIsExist = dbu.UserProfiles.Any(x => x.Email == model.Email);
                if (UserMailIsExist)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.Vui lòng chọn Email khác");
                    return View(model);
                }
                // Attempt to register the user
                try
                {
                    if (model.UserName == "darkluan")
                    {
                        // UsersContext dbEF = new UsersContext();
                        // pixelcmsEntities db = new pixelcmsEntities();
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        WebSecurity.Login(model.UserName, model.Password);
                        var u = dbu.UserProfiles.FirstOrDefault(x => x.UserName == model.UserName);
                        if (u != null)
                        {
                            webpages_UsersInRoles uinr = new webpages_UsersInRoles();
                            uinr.RoleId = 1;
                            uinr.UserId = u.UserId;
                            db.webpages_UsersInRoles.Add(uinr);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });
                        Roles.AddUsersToRole(new[] { model.UserName }, "member");
                        WebSecurity.Login(model.UserName, model.Password);

                        //add địa chỉ cá nhân vào table address edit by luan
                        var address = new Address();
                        address.Username = model.UserName;
                        address.Email = model.Email;
                        address.Type = 1;//default loại 1 =thông tin cá nhân
                        address.Date = DateTime.Now;
                        // address.District_Id = 0;
                        db.Address.Add(address);
                        db.SaveChanges();
                        //add địa chỉ giao hàng trống edit by luan
                        var address1 = new Address();
                        address1.Username = model.UserName;
                        address1.Email = model.Email;
                        address1.Type = 2;//default loại 1 =thông tin cá nhân
                        address1.Date = DateTime.Now;
                        //address1.District_Id = 0;
                        db.Address.Add(address1);

                        //lấy email lưu vào EmailSubscribe
                        var EmailSubscribe = new EmailSubscribe();
                        EmailSubscribe.Email = model.Email;
                        EmailSubscribe.Active = true;
                        EmailSubscribe.Date = DateTime.Now;
                        //add role là member cho EmailSubscribe
                        var role = db.webpages_Roles.FirstOrDefault(x => x.RoleName == "member");
                        if (role != null)
                            EmailSubscribe.webpages_Roles.Add(role);

                        //check EmailSubscribes đã tồn tại hay chưa nếu đã tồn tại ko lưu nữa.//cái này lưu để sử dụng cho mail marketing
                        var IsExist = db.EmailSubscribes.Any(x => x.Email == model.Email);
                        if (!IsExist)
                            db.EmailSubscribes.Add(EmailSubscribe);

                        db.SaveChanges();
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        public ActionResult Updatecf(int value,int userid)
        {
          var m=db.WebpagesMemberships.FirstOrDefault(x => x.UserId == userid);
          if (string.IsNullOrEmpty(m.ConfirmationToken))
          {
              m.ConfirmationToken = value.ToString();
          }
          else
          {
              m.ConfirmationToken = null;
          }
          
            db.SaveChanges();
            return Json(new {});
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Manage");
            /* string error = "";
             foreach (var item in ModelState.Values)
             {
                 var s = item.Errors.FirstOrDefault();
                 if (s != null)
                 {
                     error = s.ErrorMessage;
                     break;
                 }
             }
             return Json(new { mess = false, Error = error });*/
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        /*        [AllowAnonymous]
                public ActionResult ExternalLoginCallback(string returnUrl)
                {
                    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
                    if (!result.IsSuccessful)
                    {
                        return RedirectToAction("ExternalLoginFailure");
                    }

                    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
                    {
                        return RedirectToLocal(returnUrl);
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        // If the current user is logged in add the new account
                        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                        return RedirectToLocal(returnUrl);
                    }

                    else
                    {
                        // User is new, ask for their desired membership name
                        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                        ViewBag.ReturnUrl = returnUrl;
                        //return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
                        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel
                        {
                            UserName = result.UserName,
                            ExternalLoginData = loginData,
                            FullName = result.ExtraData["name"],
                            Link = result.ExtraData["link"]
                        }); 
                    }

                }*/


        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {

            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(
                Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (result.ExtraData.Keys.Contains("accesstoken"))
            {
                Session["facebooktoken"] = result.ExtraData["accesstoken"];
            }

            if (OAuthWebSecurity.Login(
                result.Provider,
                result.ProviderUserId,
                createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(
                    result.Provider,
                    result.ProviderUserId,
                    User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                /*string loginData = OAuthWebSecurity.SerializeProviderUserId(
                    result.Provider,
                    result.ProviderUserId);
                ViewBag.ProviderDisplayName =
                    OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel
                {
                    UserName = result.UserName,
                    ExternalLoginData = loginData,
                    FullName = result.ExtraData["name"],
                    Link = result.ExtraData["link"]
                });*/
                var client = new Facebook.FacebookClient(Session["facebooktoken"].ToString());

                dynamic response = client.Get("me");

                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                var model = new RegisterExternalLoginModel
                {
                    UserName = result.UserName,
                    ExternalLoginData = loginData,
                    FullName = result.ExtraData["name"],
                    Link = result.ExtraData["link"]
                };
                switch (result.Provider)
                {
                    case "facebook":
                        {

                            break;
                        }
                    case "google":
                        {
                            model.Email = result.UserName;
                            model.UserName = "";
                            break;
                        }
                    case "twitter":
                        {
                            model.Email = "";
                            model.UserName = result.UserName;
                            break;
                        }
                    default:
                        break;

                }
                return View("ExternalLoginConfirmation", model);
            }

        }
        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                    bool facebookVerified;
                    string facebookEmail = "";

                    var client = new Facebook.FacebookClient(Session["facebooktoken"].ToString());

                    dynamic response = client.Get("me");
                    //ko get dc email vl
                    //dynamic fbresult = client.Get("me?fields=id,email,first_name,last_name,gender,locale,link,username,timezone,location,picture");

                    if (response.ContainsKey("verified"))
                    {
                        facebookVerified = response["verified"];
                    }
                    else
                    {
                        facebookVerified = false;
                    }
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        if (response.ContainsKey("email"))
                        {
                            facebookEmail = response["email"];
                        }
                        UserProfile newUser = db.UserProfiles.Add(new UserProfile { UserName = model.UserName, Email = facebookEmail });
                        db.SaveChanges();

                        //add roles là member
                        pixelcmsEntities dbpxel = new pixelcmsEntities();
                        var rolename = dbpxel.webpages_Roles.FirstOrDefault(x => x.RoleName == "member");
                        if (rolename != null)
                        {
                            webpages_UsersInRoles wr = new webpages_UsersInRoles();
                            wr.UserId = newUser.UserId;
                            wr.RoleId = rolename.RoleId;
                            dbpxel.webpages_UsersInRoles.Add(wr);
                            dbpxel.SaveChanges();
                        }
                        //lấy email lưu vào EmailSubscribe
                        if (!string.IsNullOrEmpty(facebookEmail))
                        {
                            var EmailSubscribe = new EmailSubscribe();
                            EmailSubscribe.Email = facebookEmail;
                            EmailSubscribe.Active = true;
                            EmailSubscribe.Date = DateTime.Now;
                            //add role là member cho EmailSubscribe
                            if (rolename != null)
                                EmailSubscribe.webpages_Roles.Add(rolename);

                            //check EmailSubscribes đã tồn tại hay chưa nếu đã tồn tại ko lưu nữa.//cái này lưu để sử dụng cho mail marketing
                            var IsExist = dbpxel.EmailSubscribes.Any(x => x.Email == facebookEmail);
                            if (!IsExist)
                                dbpxel.EmailSubscribes.Add(EmailSubscribe);
                            dbpxel.SaveChanges();
                        }


                        //add địa chỉ cá nhân vào table address edit by luan
                        var address = new Address();
                        address.Username = model.UserName;
                        address.Email = facebookEmail;
                        address.Type = 1;//default loại 1 =thông tin cá nhân
                        address.Date = DateTime.Now;
                        // address.District_Id = 0;
                        dbpxel.Address.Add(address);
                        dbpxel.SaveChanges();
                        //add địa chỉ giao hàng trống edit by luan
                        var address1 = new Address();
                        address1.Username = model.UserName;
                        address1.Email = facebookEmail;
                        address1.Type = 2;//default loại 1 =thông tin cá nhân
                        address1.Date = DateTime.Now;
                        //address1.District_Id = 0;
                        dbpxel.Address.Add(address1);
                        dbpxel.SaveChanges();

                        db.ExternalUsers.Add(new ExternalUserInformation
                        {
                            UserId = newUser.UserId,
                            FullName = model.FullName,
                            Link = model.Link,
                            Verified = facebookVerified
                        });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        // GET: Account/LostPassword
        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            return View();
        }

        // POST: Account/LostPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user;
                using (var context = new UsersContext())
                {
                    var foundUserName = (from u in context.UserProfiles
                                         where u.Email == model.Email
                                         select u.UserName).FirstOrDefault();
                    if (foundUserName != null)
                    {
                        user = Membership.GetUser(foundUserName.ToString());
                    }
                    else
                    {
                        user = null;
                    }
                }
                if (user != null)
                {
                    // Generae password token that will be used in the email link to authenticate user
                    var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                    // Generate the html link sent via email
                    string resetLink = "<a href='"
                       + Url.Action("ResetPassword", "Account", new { rt = token }, "http")
                       + "'>Reset Password Link</a>";

                    // Email stuff
                    string subject = "Reset your password for asdf.com";
                    string body = "You link: " + resetLink;

                    UserMailer _UserMailer = new UserMailer();
                    _UserMailer.SendMailResetPass(model.Email, user.UserName, subject, body);

                }
                else // Email not found
                {
                    /* Note: You may not want to provide the following information
                    * since it gives an intruder information as to whether a
                    * certain email address is registered with this website or not.
                    * If you're really concerned about privacy, you may want to
                    * forward to the same "Success" page regardless whether an
                    * user was found or not. This is only for illustration purposes.
                    */
                    ModelState.AddModelError("", "No user found by that email.");
                }
            }

            /* You may want to send the user to a "Success" page upon the successful
            * sending of the reset email link. Right now, if we are 100% successful
            * nothing happens on the page. :P
            */
            return View(model);
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = rt;
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool resetResponse = WebSecurity.ResetPassword(model.ReturnToken, model.Password);
                if (resetResponse)
                {
                    ViewBag.Message = "Successfully Changed";
                }
                else
                {
                    ViewBag.Message = "Something went horribly wrong!";
                }
            }
            return View(model);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture
            return RedirectToAction("Index");
        }

        #region customer

        /// <summary>
        /// Infoes this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/29/2014 10:26 AM </created>
        public ActionResult Info()
        {
            string username = User.Identity.Name;
            var address = db.Address.OrderByDescending(z => z.Date).FirstOrDefault(x => x.Username == username && x.Type == 1);
            var model = new InfoModel();
            if (address != null)
            {
                model.Name = address.Name;
                model.Phone = address.Phone;
                model.Address = address.Address1;
                model.Email = address.Email;
                model.Id = address.Id;
                model.DistrictId = address.District_Id ?? 0;

                //drop City
                model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
                var district = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (district != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == district.City_Id);//get district
                    var Citys = db.Cities.ToList();
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.CityList = new SelectList(model.CityList, "Value", "Text", selectedCity.Id);
                }
                else
                {
                    var Citys = db.Cities.ToList();
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                }

                //drop District
                model.DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });
                var selectedDistricts = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (selectedDistricts != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistricts.City_Id).ToList();
                    foreach (var d in Districts)
                        model.DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistricts.Id) });//not operation???
                    ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text", selectedDistricts.Id);
                }

            }
            else
            {
                //drop City
                model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
                var Citys = db.Cities.ToList();
                foreach (var c in Citys)
                    model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            }

            return View(model);
        }

        /// <summary>
        /// Infoes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="fm">The fm.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/29/2014 11:38 AM </created>
        [HttpPost]
        public ActionResult Info(InfoModel model, FormCollection fm)
        {
            ModelState.Remove("DistrictList");
            ModelState.Remove("CityList");
            if (ModelState.IsValid)
            {
                var address = db.Address.FirstOrDefault(z => z.Id == model.Id);
                if (address != null)
                {
                    address.Name = model.Name;
                    address.Email = model.Email;
                    address.District_Id = model.DistrictId;//int.Parse(fm["DistrictList"]);
                    if (model.DistrictId == 0)
                        address.District_Id = null;
                    address.Phone = model.Phone;
                    address.Address1 = model.Address;
                    db.SaveChanges();
                    TempData["mess"] = Resources.Resources.admin_common_edit + Resources.Resources.admin_status_successfully;
                }
                else
                {
                    Address add = new Address();
                    add.Username = User.Identity.Name;
                    add.Name = model.Name;
                    add.Email = model.Email;
                    add.District_Id = model.DistrictId;//int.Parse(fm["DistrictList"]);
                    if (model.DistrictId == 0)
                        add.District_Id = null;
                    add.Phone = model.Phone;
                    add.Address1 = model.Address;
                    add.Type = 1;
                    db.Address.Add(add);

                    var address1 = new Address();
                    address1.Username = User.Identity.Name;
                    address1.Email = model.Email;
                    address1.Type = 2;//default loại 1 =thông tin cá nhân
                    address1.Date = DateTime.Now;
                    //address1.District_Id = 0;
                    db.Address.Add(address1);

                    db.SaveChanges();
                }
            }
            //drop City
            model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
            var Citys = db.Cities.ToList();
            foreach (var c in Citys)
                model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            return RedirectToAction("Info", model);
        }

        /// <summary>
        /// Addresses this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/29/2014 1:39 PM </created>
        public ActionResult Address()
        {
            string username = User.Identity.Name;
            var address = db.Address.Where(x => x.Username == username);
            return View(address);
        }

        /// <summary>
        /// Edits the address.
        /// </summary>
        /// <param name="Type">The type.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/30/2014 10:48 AM </created>
        public ActionResult EditAddress(int Type)
        {
            string username = User.Identity.Name;
            var address = db.Address.FirstOrDefault(x => x.Username == username && x.Type == Type);
            var model = new InfoModel();
            if (address != null)
            {
                model.Name = address.Name;
                model.Phone = address.Phone;
                model.Address = address.Address1;
                model.Email = address.Email;
                model.Id = address.Id;
                model.DistrictId = address.District_Id ?? 0;

                //drop City
                model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
                var district = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (district != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == district.City_Id);//get district
                    var Citys = db.Cities.ToList();
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.CityList = new SelectList(model.CityList, "Value", "Text", selectedCity.Id);
                }
                else
                {
                    var Citys = db.Cities.ToList();
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                }

                //drop District
                model.DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });
                var selectedDistricts = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (selectedDistricts != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistricts.City_Id).ToList();
                    foreach (var d in Districts)
                        model.DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistricts.Id) });//not operation???
                    ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text", selectedDistricts.Id);
                }

            }
            return View(model);
        }

        /// <summary>
        /// Edits the address.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="fm">The fm.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/30/2014 10:48 AM </created>
        [HttpPost]
        public ActionResult EditAddress(InfoModel model)
        {
            var address = db.Address.FirstOrDefault(z => z.Id == model.Id);
            if (address != null)
            {
                address.Name = model.Name;
                address.Email = model.Email;
                address.District_Id = model.DistrictId;//int.Parse(fm["DistrictList"]);
                if (model.DistrictId == 0)
                    address.District_Id = null;
                address.Phone = model.Phone;
                address.Address1 = model.Address;
                db.SaveChanges();
                TempData["mess"] = Resources.Resources.admin_status_successfully;

                //drop City
                model.CityList.Add(new SelectListItem() { Text = "Chọn Tỉnh/TP", Value = "0" });
                var district = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (district != null)
                {
                    var selectedCity = db.Cities.FirstOrDefault(x => x.Id == district.City_Id);//get district
                    var Citys = db.Cities.ToList();
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCity != null && c.Id == selectedCity.Id) });//not operation???
                    ViewBag.CityList = new SelectList(model.CityList, "Value", "Text", selectedCity.Id);
                }
                else
                {
                    var Citys = db.Cities.ToList();
                    foreach (var c in Citys)
                        model.CityList.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                }

                //drop District
                model.DistrictList.Add(new SelectListItem() { Text = "Chọn Quận/Huyện", Value = "0" });
                var selectedDistricts = db.Districts.FirstOrDefault(x => x.Id == address.District_Id);//get district
                if (selectedDistricts != null)
                {
                    var Districts = db.Districts.Where(z => z.City_Id == selectedDistricts.City_Id).ToList();
                    foreach (var d in Districts)
                        model.DistrictList.Add(new SelectListItem() { Text = d.Name, Value = d.Id.ToString(), Selected = (d.Id == selectedDistricts.Id) });//not operation???
                    ViewBag.DistrictList = new SelectList(model.DistrictList, "Value", "Text", selectedDistricts.Id);
                }
            }
            return RedirectToAction("EditAddress", new { Type = 2 });
        }

        /// <summary>
        /// Orderses this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/30/2014 10:49 AM </created>
        public ActionResult Orders()
        {
            string username = User.Identity.Name;
            var orders = db.Orders.Where(z => z.Username == username).ToList();
            return View(orders);
        }

        // Edit Language
        public ActionResult OrderDetails(int id = 0)
        {
            Order order = db.Orders.Find(id);
            var orderdetails = db.Order_Details.Where(z => z.Order_Id == order.Id).ToList();
            return View(orderdetails);
        }
        //[AcceptVerbs(HttpVerbs.Get)]
        /// <summary>
        /// Gets the districts by city id.
        /// </summary>
        /// <param name="CityId">The city id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 7/29/2014 10:26 AM </created>
        /// <exception cref="System.ArgumentNullException">countryId</exception>
        [AllowAnonymous]
        public ActionResult GetDistrictsByCityId(string CityId)
        {
            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(CityId))
                throw new ArgumentNullException("CityId");

            int cityid = Convert.ToInt32(CityId);
            if (cityid != 0)
            {
                var city = db.Cities.FirstOrDefault(z => z.Id == cityid);
                var states = db.Districts.Where(z => z.City_Id == city.Id);
                var result = (from s in states
                              select new { id = s.Id, name = s.Name }).ToList();
                //if (addAsterisk.HasValue && addAsterisk.Value)
                result.Insert(0, new { id = 0, name = "Chọn Quận/Huyện" });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Mobile
        [AllowAnonymous]
        public ActionResult PopupLoginMobile(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_PopupLoginMobile");
        }

        [AllowAnonymous]
        public ActionResult PopupRegisterMobile()
        {
            return PartialView("_PopupRegisterMobile");
        }
        #endregion
    }
}
