using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using PixelCMS.Filters;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / OptionController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class OptionController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Admin/Option/
        public ActionResult Index(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if(roles.FirstOrDefault() != null){
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Option == false)
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
            var options = db.Options.Where(x => x.Active == true && x.Public == true).OrderBy(x => x.Order).ToList();
            return View(options);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                //Save Option
                var option = db.Options.Where(x => x.Active == true && x.Public == true).ToList();
                for (int i = 0; i < fm.Count; i++)
                {
                    if (fm["optionId-" + i] != null)
                    {
                        if (fm["option-" + i] != null && fm["option-" + i] != string.Empty)
                        {
                            // Add new content
                            foreach (var item in option)
                            {
                                if (item.Id == int.Parse(fm["optionId-" + i]))
                                {
                                    item.Value = fm["option-" + i];
                                }
                            }
                        }
                        else {
                            foreach (var item in option)
                            {
                                if (item.Id == int.Parse(fm["optionId-" + i]))
                                {
                                    item.Value = null;
                                }
                            }
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { status = "update-done" });
            }
            return View();
        }

        public ActionResult Language(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Option == false)
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
            var languages = db.Languages.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
            return View(languages);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Language(FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                //Save Option
                var languages = db.Languages.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                foreach (var lang in languages) {
                    if (fm["Construction-" + lang.Code] == "true")
                    {
                        lang.Construction = true;
                    }
                    else {
                        lang.Construction = false;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Language", new { status = "update-done" });
            }
            return View();
        }


    }
}
