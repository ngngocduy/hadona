using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using PagedList;
using WebMatrix.WebData;
using PixelCMS.Filters;
using PixelCMS.Core.Models;
using System.Data;
using System.Data.Entity;
// -----------------------------------------
// PIXEL CMS
// Admin Area / MemberController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        List<SelectListItem> roleslist = new List<SelectListItem>();

        //
        // GET: /Admin/Member/
        #region User
        public ActionResult Index(int? page)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.User == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            using (var ctx = new UsersContext())
            {
                return View(ctx.UserProfiles.ToList().ToPagedList(pageNumber, pageSize));
            }
        }

        // Create
        public ActionResult Create()
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.User == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            var rolesl = db.webpages_Roles.OrderBy(x => x.RoleId).ToList();

            foreach (var item in roles)
            {
                if (item != "tkmadmin")
                {
                    rolesl = rolesl.Where(x => x.RoleName != "tkmadmin").ToList();
                }
            }

            foreach (var item in rolesl)
            {
                roleslist.Add(new SelectListItem { Text = item.RoleName, Value = item.RoleName });
            }
            ViewBag.RolesList = roleslist;
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterModel model, FormCollection fm)
        {
            UsersContext dbu = new UsersContext();

            var rolesl = db.webpages_Roles.OrderBy(x => x.RoleId).ToList();
            foreach (var item in rolesl)
            {
                roleslist.Add(new SelectListItem { Text = item.RoleName, Value = item.RoleName });
            }
            ViewBag.RolesList = roleslist;

            if (ModelState.IsValid)
            {
                var UserMailIsExist = dbu.UserProfiles.Any(x => x.Email == model.Email);
                if (UserMailIsExist)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.Vui lòng chọn Email khác");
                    return View(model);
                }
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    Roles.AddUsersToRole(new[] { model.UserName }, fm["RolesList"]);

                    var uprofile = dbu.UserProfiles.FirstOrDefault(x=>x.UserName==model.UserName);
                    uprofile.Email = model.Email;
                    dbu.SaveChanges();
                    return RedirectToAction("Index");

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        // Edit
        public ActionResult Edit(int id)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.User == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion

            var tmpuser = "";
            var ctx = new UsersContext();
            var firstOrDefault = ctx.UserProfiles.FirstOrDefault(us => us.UserId == id);
            if (firstOrDefault != null)
                tmpuser = firstOrDefault.UserName;
            else
            {
                return HttpNotFound();
            }
            ViewBag.UserName = tmpuser;
            //drop
            var r =
                db.webpages_Roles.FirstOrDefault(
                    x => x.webpages_UsersInRoles.FirstOrDefault(z => z.UserId == id) != null);
            ViewBag.RolesList = new SelectList(db.webpages_Roles.Where(x => x.RoleName != "tkmadmin"), "RoleId", "RoleName", r != null ? r.RoleId : 0);

            //ViewBag.RolesList = roleslist;

            return View(firstOrDefault);
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile model, FormCollection fm)
        {
            /* ModelState state = ModelState["OldPassword"];
             if (state != null)
             {
                 state.Errors.Clear();
             }
             if (ModelState.IsValid)
             {
                 var tmpuser = "";
                 var ctx = new UsersContext();
                 using (ctx)
                 {
                     var firstOrDefault = ctx.UserProfiles.FirstOrDefault(us => us.UserId == id);
                     if (firstOrDefault != null)
                         tmpuser = firstOrDefault.UserName;
                 }
                 try
                 {
                     var token = WebSecurity.GeneratePasswordResetToken(tmpuser);
                     WebSecurity.ResetPassword(token, model.NewPassword);
                     return RedirectToAction("Index");
                 }
                 catch (Exception e)
                 {
                     ViewBag.UserName = tmpuser;
                     ModelState.AddModelError("", e);
                 }
             }*/
            /*var r =
               db.webpages_Roles.FirstOrDefault(
                   x => x.webpages_UsersInRoles.FirstOrDefault(z => z.UserId == id) != null);
            if (r!=null)
            {
                r.RoleId = int.Parse(fm["RolesList"]);
                db.SaveChanges();
            }*/
            //drop
            var r =
                db.webpages_Roles.FirstOrDefault(
                    x => x.webpages_UsersInRoles.FirstOrDefault(z => z.UserId == model.UserId) != null);
            ViewBag.RolesList = new SelectList(db.webpages_Roles, "RoleId", "RoleName", r != null ? r.RoleId : 0);

                UsersContext dbu = new UsersContext();
                var UserMailIsExist = dbu.UserProfiles.Any(x => x.Email == model.Email);
                if (UserMailIsExist)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.Vui lòng chọn Email khác");
                    return View(model);
                }

                var uprofile = dbu.UserProfiles.Find(model.UserId);
                uprofile.Email = model.Email;
                dbu.SaveChanges();

            // check nếu chưa có nhóm thì add(1 user chỉ đc 1 nhóm mới add dc)
            //remove nhóm củ
            var check = db.webpages_UsersInRoles.FirstOrDefault(x => x.UserId == model.UserId);
            if (check != null)
                db.webpages_UsersInRoles.Remove(check);

            //add lại nhóm mới
            webpages_UsersInRoles addrole = new webpages_UsersInRoles();
            addrole.UserId = model.UserId;
            addrole.RoleId = int.Parse(fm["RolesList"]);
            db.webpages_UsersInRoles.Add(addrole);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var tmpuser = "";
                var ctx = new UsersContext();
                //using (ctx)
                //{
                var firstOrDefault = ctx.UserProfiles.FirstOrDefault(us => us.UserId == id);
                if (firstOrDefault != null)
                    tmpuser = firstOrDefault.UserName;
                //  }


                string[] allRoles = Roles.GetRolesForUser(tmpuser);
                Roles.RemoveUserFromRoles(tmpuser, allRoles);

                //xóa user xóa luôn địa chỉ của user
                var address = db.Address.Where(x => x.Username == tmpuser);
                foreach (var item in address)
                {
                    db.Address.Remove(item);
                }
                db.SaveChanges();
                //Roles.RemoveUserFromRole(tmpuser, "RoleName");

                ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(tmpuser);
                Membership.Provider.DeleteUser(tmpuser, true);
                Membership.DeleteUser(tmpuser, true);

                //delete table ExtraUserInfomation 
                var u = ctx.ExternalUsers.FirstOrDefault(x => x.UserId == id);
                if (u != null)
                {
                    ctx.ExternalUsers.Remove(u);
                    ctx.SaveChanges();
                }
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Roles
        public ActionResult UserRoles(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.User == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            ViewBag.Status = status;
            var roleslist = db.webpages_Roles.ToList().OrderBy(x => x.RoleId);
            return View(roleslist);
        }

        public ActionResult CreateUserRoles()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUserRoles(webpages_Roles role)
        {
            if (ModelState.IsValid)
            {
                db.webpages_Roles.Add(role);
                db.SaveChanges();

                var access = new Roles_Access();
                access.Role_Id = role.RoleId;
                db.Roles_Access.Add(access);

                var posttype = db.PostTypes.ToList();
                var posttyperolelist = db.Roles_PostType.ToList();
                foreach (var item in posttype)
                {
                    if (posttyperolelist.FirstOrDefault(x => x.Role_Id == role.RoleId && x.PostType_Id == item.Id) == null)
                    {
                        var posttyperole = new Roles_PostType();
                        posttyperole.PostType_Id = item.Id;
                        posttyperole.Role_Id = role.RoleId;
                        db.Roles_PostType.Add(posttyperole);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("UserRoles", new { status = "create-done" });
            }
            return View(role);
        }


        public ActionResult EditUserRoles(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.User == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            webpages_Roles role = db.webpages_Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditUserRoles(webpages_Roles role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserRoles", new { status = "edit-done" });
            }
            return View(role);
        }

        [HttpPost]
        public ActionResult DeleteUserRoles(int id)
        {
            try
            {
                var userinrole = db.webpages_UsersInRoles.Where(x => x.RoleId == id).ToList();
                if (userinrole != null)
                {
                    foreach (var item in userinrole)
                    {
                        db.webpages_UsersInRoles.Remove(item);
                    }
                }

                Roles_Access access = db.Roles_Access.Find(id);
                db.Roles_Access.Remove(access);

                var postrole = db.Roles_PostType.Where(x => x.Role_Id == id).ToList();
                if (postrole.FirstOrDefault() != null)
                {
                    foreach (var item in postrole)
                    {
                        db.Roles_PostType.Remove(item);
                    }
                }

                webpages_Roles role = db.webpages_Roles.Find(id);
                db.webpages_Roles.Remove(role);

                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }
        #endregion

        #region RolesAccess
        public ActionResult RolesAccess(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.User == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            Roles_Access access = db.Roles_Access.Find(id);
            if (access == null)
            {
                var role = db.webpages_Roles.Find(id);
                access = new Roles_Access();
                access.Role_Id = role.RoleId;
                db.Roles_Access.Add(access);
            }
            return View(access);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RolesAccess(Roles_Access access)
        {
            if (ModelState.IsValid)
            {
                db.Entry(access).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserRoles", new { status = "edit-done" });
            }
            return View(access);
        }
        #endregion

        #region Helper
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
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

    }
}
