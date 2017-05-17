using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using PagedList;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / AttributeController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class AttributeController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        #region Attribute
        //
        // GET: /Admin/Attribute/
        public ActionResult Index(string status, int? page)
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
            var PostAttributes = db.PostAttributes.OrderBy(x => x.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PostAttributes.ToPagedList(pageNumber, pageSize));
        }

        // Create Attribute
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
            
            // Edit Type List
            List<SelectListItem> edittypes = new List<SelectListItem>();
            edittypes.Add(new SelectListItem { Text = "Textbox", Value = "1" });
            edittypes.Add(new SelectListItem { Text = "TextArea", Value = "2" });
            edittypes.Add(new SelectListItem { Text = "DropdownList", Value = "3" });
            //edittypes.Add(new SelectListItem { Text = "Ratio button list", Value = "4" });
            edittypes.Add(new SelectListItem { Text = "ImageUpload", Value = "5" });
            edittypes.Add(new SelectListItem { Text = "Datepicker", Value = "6" });
            edittypes.Add(new SelectListItem { Text = "CKeditor", Value = "7" });
            edittypes.Add(new SelectListItem { Text = "FileUpload", Value = "8" });

            ViewBag.Edit_Type = edittypes;
            // Attribute Group
            var currentculture = PixelCMS.Helpers.CultureHelper.GetCurrentNeutralCulture();
            List<SelectListItem> items = new List<SelectListItem>();
            var groups = db.AttributeGroups.ToList();
            for (int i = 0; i < groups.Count(); i++)
            {
                items.Add(new SelectListItem { Text = groups[i].PostType_Name.FirstOrDefault(x => x.Lang == currentculture).Name, Value = groups[i].Id.ToString() });
            }
            ViewBag.Group = items;
            return View();
        }

        [HttpPost]
        public ActionResult Create(PostAttribute postattribute, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.PostAttributes.Add(postattribute);
                var langs = db.Languages.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.Attribute_id = postattribute.Id;
                        opname.Lang = item.Code;
                        opname.Name = fm["name-" + item.Code];
                        db.PostType_Names.Add(opname);
                    }
                    else
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.Attribute_id = postattribute.Id;
                        opname.Lang = item.Code;
                        db.PostType_Names.Add(opname);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { status="create-done" });
            }
            return View(postattribute);
        }

        //
        // GET: /Admin/Attribute/Edit/5

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
            
            PostAttribute attribute = db.PostAttributes.Find(id);
            if (attribute == null)
            {
                return HttpNotFound();
            }
            // Edit Type List
            List<SelectListItem> edittypes = new List<SelectListItem>();
            edittypes.Add(new SelectListItem { Selected=false, Text = "Textbox", Value = "1" });
            edittypes.Add(new SelectListItem { Selected=false, Text = "TextArea", Value = "2" });
            edittypes.Add(new SelectListItem { Selected = false, Text = "DropdownList", Value = "3" });
            //edittypes.Add(new SelectListItem { Selected = false, Text = "Ratio button list", Value = "4" });
            edittypes.Add(new SelectListItem { Selected = false, Text = "FileUpload", Value = "5" });
            edittypes.Add(new SelectListItem { Selected = false, Text = "Datepicker", Value = "6" });
            edittypes.Add(new SelectListItem { Selected = false, Text = "CKeditor", Value = "7" });
            edittypes.Add(new SelectListItem { Selected = false, Text = "FileUpload", Value = "8" });

            foreach (var item in edittypes) {
                if (item.Value == Convert.ToString(attribute.Edit_Type))
                {
                    item.Selected = true;
                }
            }
            ViewBag.Edit_Type = edittypes;

            // Attribute Group
            var currentculture = PixelCMS.Helpers.CultureHelper.GetCurrentNeutralCulture();
            List<SelectListItem> items = new List<SelectListItem>();
            var groups = db.AttributeGroups.ToList();
            for (int i = 0; i < groups.Count(); i++)
            {
                if (groups[i].Id == attribute.Group)
                {
                    items.Add(new SelectListItem { Selected = true, Text = groups[i].PostType_Name.FirstOrDefault(x => x.Lang == currentculture).Name, Value = groups[i].Id.ToString() });
                }
                else
                {
                    items.Add(new SelectListItem { Text = groups[i].PostType_Name.FirstOrDefault(x => x.Lang == currentculture).Name, Value = groups[i].Id.ToString() });
                }
            }
            ViewBag.Group = items;

            var langs = db.Languages.OrderBy(x => x.Order).ToList();
            foreach (var item in langs)
            {
                var op = db.PostType_Names.FirstOrDefault(x => x.Attribute_id == attribute.Id && x.Lang == item.Code);
                if (op == null)
                {
                    PostType_Name opname = new PostType_Name();
                    opname.Attribute_id = attribute.Id;
                    opname.Lang = item.Code;
                    opname.Name = attribute.Name;
                    db.PostType_Names.Add(opname);
                    db.SaveChanges();
                }
            }

            return View(attribute);
        }

        //
        // POST: /Admin/Attribute/Edit/5

        [HttpPost]
        public ActionResult Edit(PostAttribute attribute, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attribute).State = EntityState.Modified;
                var langs = db.Languages.OrderBy(x => x.Order).ToList();

                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        var oldname = db.PostType_Names.FirstOrDefault(x => x.Attribute_id == attribute.Id && x.Lang == item.Code);
                        if (oldname.Name != fm["name-" + item.Code])
                        {
                            oldname.Name = fm["name-" + item.Code];
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { status = "edit-done" });
            }
            return View(attribute);
        }

        // DELETE ATTRIBUTE
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
                PostAttribute attribute = db.PostAttributes.Find(id);
                var value = db.AttributeValues.Where(x => x.Attribute == id).ToList();
                foreach (var item in value) {
                    db.AttributeValues.Remove(item);
                }
                db.PostAttributes.Remove(attribute);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Attribute Group
        public ActionResult AttributeGroup(string status)
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
            ViewBag.Status = status;
            return View(db.AttributeGroups.ToList());
        }

        // Create Attribute Group
        public ActionResult CreateGroup()
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
        public ActionResult CreateGroup(AttributeGroup group, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.AttributeGroups.Add(group);
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.AttributeGroup_id = group.Id;
                        opname.Lang = item.Code;
                        opname.Name = fm["name-" + item.Code];
                        db.PostType_Names.Add(opname);
                    }
                    else
                    {
                        PostType_Name opname = new PostType_Name();
                        opname.AttributeGroup_id = group.Id;
                        opname.Lang = item.Code;
                        db.PostType_Names.Add(opname);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("AttributeGroup", new { status = "create-done" });
            }
            return View(group);
        }

        //  EDIT Group ATTRIBUTE
        public ActionResult EditGroup(int id = 0)
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
            AttributeGroup attributegroup = db.AttributeGroups.Find(id);
            if (attributegroup == null)
            {
                return HttpNotFound();
            }
            var langs = db.Languages.OrderBy(x => x.Order).ToList();
            foreach (var item in langs)
            {
                var op = db.PostType_Names.FirstOrDefault(x => x.AttributeGroup_id == attributegroup.Id && x.Lang == item.Code);
                if (op == null)
                {
                    PostType_Name opname = new PostType_Name();
                    opname.AttributeGroup_id = attributegroup.Id;
                    opname.Lang = item.Code;
                    opname.Name = attributegroup.Name;
                    db.PostType_Names.Add(opname);
                    db.SaveChanges();
                }
            }
            return View(attributegroup);
        }

        [HttpPost]
        public ActionResult EditGroup(AttributeGroup attributegroup, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attributegroup).State = EntityState.Modified;
                var langs = db.Languages.OrderBy(x => x.Order).ToList();
                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        var oldname = db.PostType_Names.FirstOrDefault(x => x.AttributeGroup_id == attributegroup.Id && x.Lang == item.Code);
                        if (oldname.Name != fm["name-" + item.Code])
                        {
                            oldname.Name = fm["name-" + item.Code];
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("AttributeGroup", new { status = "edit-done" });
            }
            return View(attributegroup);
        }

        // DELETE Group ATTRIBUTE
        [HttpPost]
        public ActionResult DeleteGroup(int id)
        {
            try
            {
                AttributeGroup attributegroup = db.AttributeGroups.Find(id);
                db.AttributeGroups.Remove(attributegroup);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }


        #endregion

        #region Attribute Value
        //
        // Value list
        public ActionResult Value(int id)
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

            PostAttribute attribute = db.PostAttributes.Find(id);
            if (attribute == null)
            {
                return HttpNotFound();
            }
            ViewBag.AttributeId = attribute.Id;
            ViewBag.Name = attribute.Name;
            var values = db.AttributeValues.Where(x => x.Attribute == id).ToList();
            return View(values);
        }

        //
        // Value input
        public ActionResult Value_Upload(int id)
        {
            ViewBag.Id = id;
            return PartialView("_Value_Upload");
        }

        [HttpPost]
        public ActionResult Value_Upload(string Value, string Id)
        {
            if (ModelState.IsValid)
            {
                var value = new AttributeValue();
                value.Value = Value;
                value.Attribute = Convert.ToInt32(Id);
                db.AttributeValues.Add(value);
                db.SaveChanges();
            }
            int id = Convert.ToInt32(Id);
            var values = db.AttributeValues.Where(x => x.Attribute == id).ToList();
            return PartialView("_ValueList", values);
        }

        // Value Delete
        [HttpPost]
        public ActionResult ValueDelete(int id)
        {
            try
            {
                AttributeValue value = db.AttributeValues.Find(id);
                db.AttributeValues.Remove(value);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
