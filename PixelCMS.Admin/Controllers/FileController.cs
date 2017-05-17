using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using PixelCMS.Areas.Admin.Controllers;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    //  [Auth(View = (int)ViewEnum.InputDataManagement, Role = (int)RoleEnum.Edit)]
    public class FileController : BaseController
    {
        //
        // GET: /File/
        private pixelcmsEntities db = new pixelcmsEntities();
        public virtual ActionResult Files(string subFolder)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Email == false)
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

            // FileViewModel contains the root MyFolder and the selected subfolder if any
            FileViewModel model = new FileViewModel() {Folder = "", SubFolder = subFolder};

            return View(model);
        }
    }
}
