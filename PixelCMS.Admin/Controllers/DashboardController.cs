using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using PagedList;
using PixelCMS.Filters;
using System.Web.Security;
using WebMatrix.WebData;
using PixelCMS.Helpers;
// -----------------------------------------
// PIXEL CMS
// Admin Area / DashboardController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Admin/Dashboard/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return Redirect("/Account/LoginPanel?returnUrl=/quantriweb");
            }

            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            foreach (var item in roles)
            {
                var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                if (db.Roles_Access.Find(roleid).Admin == false)
                {
                    return RedirectToAction("Index", "Home",new {area=""});
                }
            }
            #endregion

            var langlist = db.Languages.Where(x => x.Admin == true).ToList();
            if (langlist.Count() == 1)
            {
                var culture = langlist.FirstOrDefault().Code;
                // Save culture in a cookie
                HttpCookie cookie = Request.Cookies["_culture"];
                if (cookie != null)
                    cookie.Value = culture;   // update cookie value
                else
                {
                    cookie = new HttpCookie("_culture");
                    cookie.Value = culture;
                    cookie.Expires = DateTime.Now.AddYears(1);
                }
                Response.Cookies.Add(cookie);
            }

            var posttypes = db.PostTypes.Where(x => x.DashboardStats == true && x.Active == true).ToList();
            return View(posttypes);
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult SetCulture(string culture, string url)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            //return RedirectToAction("Index");
            return Redirect(url);
        }
    }
 
}
