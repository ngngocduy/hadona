using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / LanguageController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class LanguageController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Admin/Language/

        public ActionResult Index(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            foreach (var item in roles)
            if(roles.FirstOrDefault() != null){
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.PixelAdmin == false)
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
            return View(db.Languages.ToList());
        }

        // Add new Language
        public ActionResult Create()
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if(roles.FirstOrDefault() != null){
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.PixelAdmin == false)
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
            return View();
        }

        [HttpPost]
        public ActionResult Create(Language lang)
        {
            if (ModelState.IsValid)
            {
                db.Languages.Add(lang);
                db.SaveChanges();
                return RedirectToAction("Index", new { status = "create-done"});
            }
            return View(lang);
        }

        // Edit Language
        public ActionResult Edit(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if(roles.FirstOrDefault() != null){
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.PixelAdmin == false)
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
            Language lang = db.Languages.Find(id);
            if (lang == null)
            {
                return HttpNotFound();
            }
            return View(lang);
        }

        [HttpPost]
        public ActionResult Edit(Language lang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { status = "edit-done" });
            }
            return View(lang);
        }

        // DELETE Language
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Language lang = db.Languages.Find(id);
                db.Languages.Remove(lang);
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
