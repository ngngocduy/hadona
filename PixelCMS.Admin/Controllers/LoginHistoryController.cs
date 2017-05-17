using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class LoginHistoryController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Shipping/
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
                    if (roleaccess.Admin == false || roleaccess.History == false)
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

            var history = db.LoginHistories.Where(x => x.UserName != "tkmadmin").OrderByDescending(x=>x.TimeStart).ToList();

            if (User.IsInRole("tkmadmin"))
                history = db.LoginHistories.OrderByDescending(x => x.TimeStart).ToList();

            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(history.ToPagedList(pageNumber, pageSize));
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                var ship = db.LoginHistories.Find(id);
                db.LoginHistories.Remove(ship);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

    }
}
