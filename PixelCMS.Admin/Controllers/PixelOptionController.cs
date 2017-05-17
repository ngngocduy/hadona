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
// Admin Area / PixelOptionController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class PixelOptionController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        #region Control Option
        //
        // GET: /Admin/PixelConfig/
        public ActionResult Index(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
            ViewBag.Status = status;
            var options = db.Options.ToList();
            return View(options);
        }

        // Add option
        public ActionResult Create()
        {
            //Type
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
            List<SelectListItem> typelist = new List<SelectListItem>();
            typelist.Add(new SelectListItem { Text = "Textbox", Value = "1" });
            typelist.Add(new SelectListItem { Text = "TextArea", Value = "2" });
            typelist.Add(new SelectListItem { Text = "CKeditor", Value = "3" });
            typelist.Add(new SelectListItem { Text = "Image Browse", Value = "4" });
            typelist.Add(new SelectListItem { Text = "Passwords", Value = "5" });
            typelist.Add(new SelectListItem { Text = "Upload File", Value = "6" });
            ViewBag.Edit_Type = typelist;

            //Group
            List<SelectListItem> grouplist = new List<SelectListItem>();
            var groups = db.OptionGroups.ToList();
            for (int i = 0; i < groups.Count(); i++)
            {
                grouplist.Add(new SelectListItem { Text = groups[i].Name, Value = groups[i].Id.ToString() });
            }
            ViewBag.Group = grouplist;

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Option option, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.Option_id = option.Id;
                        opname.Lang = item.Code;
                        opname.Name = fm["name-" + item.Code];
                        db.PostType_Names.Add(opname);
                    }
                    else
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.Option_id = option.Id;
                        opname.Lang = item.Code;
                        db.PostType_Names.Add(opname);
                    }
                }
                db.Options.Add(option);
                db.SaveChanges();
                return RedirectToAction("Index", new { status = "create-done" });
            }
            return View(option);
        }

        // Edit option
        public ActionResult Edit(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
            Option option = db.Options.Find(id);
            if (option == null)
            {
                return HttpNotFound();
            }
            //Type
            List<SelectListItem> typelist = new List<SelectListItem>();
            typelist.Add(new SelectListItem { Text = "Textbox", Value = "1" });
            typelist.Add(new SelectListItem { Text = "TextArea", Value = "2" });
            typelist.Add(new SelectListItem { Text = "CKeditor", Value = "3" });
            typelist.Add(new SelectListItem { Text = "Image Browse", Value = "4" });
            typelist.Add(new SelectListItem { Text = "Passwords", Value = "5" });
            foreach (var item in typelist)
            {
                if (item.Value == Convert.ToString(option.Edit_Type))
                {
                    item.Selected = true;
                }
            }
            ViewBag.Edit_Type = typelist;

            //Group
            List<SelectListItem> grouplist = new List<SelectListItem>();
            var groups = db.OptionGroups.ToList();
            for (int i = 0; i < groups.Count(); i++)
            {
                if (groups[i].Id == option.Group)
                {
                    grouplist.Add(new SelectListItem { Text = groups[i].Name, Value = groups[i].Id.ToString(), Selected = true });
                }
                else
                {
                    grouplist.Add(new SelectListItem { Text = groups[i].Name, Value = groups[i].Id.ToString() });
                }
            }

            var langs = db.Languages.OrderBy(x => x.Order).ToList();
            foreach (var item in langs)
            {
                var op = db.PostType_Names.FirstOrDefault(x => x.Option_id == option.Id && x.Lang == item.Code);
                if (op == null)
                {
                    PostType_Name opname = new PostType_Name();
                    opname.Option_id = option.Id;
                    opname.Lang = item.Code;
                    opname.Name = option.Name;
                    db.PostType_Names.Add(opname);
                    db.SaveChanges();
                }
            }

            ViewBag.Group = grouplist;
            return View(option);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Option option, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(option).State = EntityState.Modified;
                var langs = db.Languages.OrderBy(x => x.Order).ToList();

                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        var oldname = db.PostType_Names.FirstOrDefault(x => x.Option_id == option.Id && x.Lang == item.Code);
                        if (oldname.Name != fm["name-" + item.Code])
                        {
                            oldname.Name = fm["name-" + item.Code];
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { status = "edit-done" });
            }
            return View(option);
        }

        // Delete Option
        [HttpPost]
        public ActionResult Delete(int id)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
            try
            {
                Option option = db.Options.Find(id);
                db.Options.Remove(option);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Option Group
        public ActionResult OptionGroup(string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
            ViewBag.Status = status;
            var groups = db.OptionGroups.ToList();
            return View(groups);
        }

        //add group
        public ActionResult CreateGroup()
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
        public ActionResult CreateGroup(OptionGroup group, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.OptionGroup_id = group.Id;
                        opname.Lang = item.Code;
                        opname.Name = fm["name-" + item.Code];
                        db.PostType_Names.Add(opname);
                    }
                    else
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.OptionGroup_id = group.Id;
                        opname.Lang = item.Code;
                        db.PostType_Names.Add(opname);
                    }
                }
                group.Name = fm["name-vi"];
                db.OptionGroups.Add(group);
                db.SaveChanges();
                return RedirectToAction("OptionGroup", new { status = "create-done" });
            }
            return View(group);
        }

        //edit group
        public ActionResult EditGroup(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
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
            OptionGroup group = db.OptionGroups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            var langs = db.Languages.OrderBy(x => x.Order).ToList();
            foreach (var item in langs)
            {
                var op = db.PostType_Names.FirstOrDefault(x => x.OptionGroup_id == group.Id && x.Lang == item.Code);
                if (op == null)
                {
                    PostType_Name opname = new PostType_Name();
                    opname.OptionGroup_id = group.Id;
                    opname.Lang = item.Code;
                    opname.Name = group.Name;
                    db.PostType_Names.Add(opname);
                    db.SaveChanges();
                }
            }
            return View(group);
        }
        [HttpPost]
        public ActionResult EditGroup(OptionGroup group, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        var oldname = db.PostType_Names.FirstOrDefault(x => x.OptionGroup_id == group.Id && x.Lang == item.Code);
                        if (oldname.Name != fm["name-" + item.Code])
                        {
                            oldname.Name = fm["name-" + item.Code];
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("OptionGroup", new { status = "edit-done" });
            }
            return View(group);
        }

        // Delete Group
        [HttpPost]
        public ActionResult DeleteGroup(int id)
        {
            try
            {
                OptionGroup group = db.OptionGroups.Find(id);
                db.OptionGroups.Remove(group);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }
        #endregion

        /// <summary>
        /// Updates the active.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 1/16/2015 8:46 AM </created>
        public ActionResult UpdateActive(int id)
        {
            var option = db.Options.Find(id);
            if (option.Active == false)
            {
                option.Active = true;
            }
            else
            {
                option.Active = false;
            }
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
