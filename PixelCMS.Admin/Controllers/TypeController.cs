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
// Admin Area / TypeController.cs v2.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class TypeController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        #region Type
        //
        // GET: /Admin/PostType/

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
            return View(db.PostTypes.ToList());
        }

        //
        // GET: /Admin/PostType/Create

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
            // Group List
            List<SelectListItem> groups = new List<SelectListItem>();
            groups.Add(new SelectListItem { Text = Resources.Resources.admin_common_cms, Value = "1" });
            groups.Add(new SelectListItem { Text = Resources.Resources.admin_common_cms + "->" + Resources.Resources.admin_common_page, Value = "2" });
            groups.Add(new SelectListItem { Text = Resources.Resources.admin_common_cms + "->" + Resources.Resources.admin_common_content, Value = "3" });
            groups.Add(new SelectListItem { Text = Resources.Resources.admin_common_product, Value = "4" });
            groups.Add(new SelectListItem { Text = "Media", Value = "5" });
            ViewBag.Group = groups;

            return View();
        }

        //
        // POST: /Admin/PostType/Create

        [HttpPost]
        public ActionResult Create(PostType posttype, int[] attributes, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                // Save attribute
                if (attributes != null)
                {
                    int a = attributes.Count();
                    for (int i = 0; i < a; i++)
                    {
                        PostAttribute postattribute = db.PostAttributes.Find(attributes[i]);
                        posttype.PostAttributes.Add(postattribute);
                    }
                }

                var langs = db.Languages.OrderBy(x => x.Order).ToList();

                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        PostType_Name typename = new PostType_Name();
                        typename.Posttype_id = posttype.Id;
                        typename.Lang = item.Code;
                        typename.Name = fm["name-" + item.Code];
                        db.PostType_Names.Add(typename);
                    }
                    else {
                        PostType_Name typename = new PostType_Name();
                        typename.Posttype_id = posttype.Id;
                        typename.Lang = item.Code;
                        db.PostType_Names.Add(typename);
                    }
                }

                #region Code check (Create)
                foreach (var item in langs)
                {
                    if (fm["code-" + item.Code] != null)
                    {
                        Slug slug = new Slug();
                        slug.PostType = posttype.Id;
                        slug.Lang = item.Code;
                        slug.Slug1 = fm["code-" + item.Code];
                        //Check slug
                        var sluglist = db.Slugs.Where(x => x.Id != slug.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.PostType }).ToList();
                        if (sluglist.FirstOrDefault(x => x.Slug1 == slug.Slug1) != null)
                        {
                            slug.Slug_Count = sluglist.Where(x => x.Slug1 == slug.Slug1).Max(m => m.Slug_Count) + 1;
                            slug.Slug_Full = slug.Slug_Count + "-" + slug.Slug1;
                        }
                        else if (sluglist.FirstOrDefault(x => x.Slug_Full == slug.Slug1) != null)
                        {
                            slug.Slug1 = sluglist.FirstOrDefault(x => x.Slug_Full == slug.Slug1).Slug1;
                            slug.Slug_Count = sluglist.Where(x => x.Slug1 == slug.Slug1).Max(m => m.Slug_Count) + 1;
                            slug.Slug_Full = slug.Slug_Count + "-" + slug.Slug1;
                        }
                        else
                        {
                            slug.Slug_Count = 1;
                            slug.Slug_Full = slug.Slug1;
                        }
                        db.Slugs.Add(slug);
                    }
                }
                #endregion

                db.PostTypes.Add(posttype);

                /* Roles - PostType Permisstion */
                var rolelist = db.webpages_Roles.ToList();
                foreach (var item in rolelist)
                {
                    var posttyperoletb = new Roles_PostType();
                    posttyperoletb.PostType_Id = posttype.Id;
                    posttyperoletb.Role_Id = item.RoleId;
                    posttyperoletb.Create = true;
                    posttyperoletb.View = true;
                    posttyperoletb.Edit = true;
                    posttyperoletb.Delete = true;
                    posttyperoletb.CatCreate = true;
                    posttyperoletb.CatView = true;
                    posttyperoletb.CatEdit = true;
                    posttyperoletb.CatDelete = true;
                    db.Roles_PostType.Add(posttyperoletb);
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { status = "create-done" });
            }

            return View(posttype);
        }

        //
        // GET: /Admin/PostType/Edit/5

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
            PostType posttype = db.PostTypes.Find(id);
            ViewBag.TypeId = posttype.Id;
            if (posttype == null)
            {
                return HttpNotFound();
            }

            var langs = db.Languages.Where(z=>z.Active).OrderBy(x => x.Order).ToList();
            foreach (var item in langs)
            {
                var op = db.PostType_Names.FirstOrDefault(x => x.Posttype_id == posttype.Id && x.Lang == item.Code);
                if (op == null)
                {
                    PostType_Name opname = new PostType_Name();
                    opname.Posttype_id = posttype.Id;
                    opname.Lang = item.Code;
                    opname.Name = posttype.Name;
                    db.PostType_Names.Add(opname);
                    db.SaveChanges();
                }
            }

            // Group List
            List<SelectListItem> groups = new List<SelectListItem>();
            groups.Add(new SelectListItem { Text = "CMS", Value = "1" });
            groups.Add(new SelectListItem { Text = "CMS->Trang", Value = "2" });
            groups.Add(new SelectListItem { Text = "CMS->Nội dung", Value = "3" });
            groups.Add(new SelectListItem { Text = "Sản phẩm", Value = "4" });
            groups.Add(new SelectListItem { Text = "Media", Value = "5" });
            foreach (var item in groups)
            {
                if (item.Value == Convert.ToString(posttype.Group))
                {
                    item.Selected = true;
                }
            }
            ViewBag.Group = groups;

            return View(posttype);
        }

        //
        // POST: /Admin/PostType/Edit/5

        [HttpPost]
        public ActionResult Edit(PostType posttype, int[] attributes, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(posttype).State = EntityState.Modified;
                // Delete old attribute in post
                var test = db.PostTypes.Include(p => p.PostAttributes).FirstOrDefault(p => p.Id == posttype.Id);
                test.PostAttributes.Clear();

                var langs = db.Languages.OrderBy(x => x.Order).ToList();

                foreach (var item in langs)
                {
                    if (fm["name-" + item.Code] != null)
                    {
                        var oldname = db.PostType_Names.FirstOrDefault(x => x.Posttype_id == posttype.Id && x.Lang == item.Code);
                        if (oldname.Name != fm["name-" + item.Code])
                        {
                            oldname.Name = fm["name-" + item.Code];
                        }
                    }
                }

                #region Code check (Edit)
                foreach (var item in langs)
                {
                    if (fm["code-"+item.Code] != null)
                    {
                        var slug = db.Slugs.FirstOrDefault(x => x.PostType == posttype.Id && x.Lang == item.Code);
                        if (slug!=null)
                        {
                            if (slug.Slug_Full != fm["code-" + item.Code])
                            {
                                slug.Slug1 = fm["code-" + item.Code];
                                //Check slug
                                var sluglist = db.Slugs.Where(x => x.Id != slug.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.PostType }).ToList();
                                if (sluglist.FirstOrDefault(x => x.Slug1 == slug.Slug1) != null)
                                {
                                    slug.Slug_Count = sluglist.Where(x => x.Slug1 == slug.Slug1).Max(m => m.Slug_Count) + 1;
                                    slug.Slug_Full = slug.Slug_Count + "-" + slug.Slug1;
                                }
                                else if (sluglist.FirstOrDefault(x => x.Slug_Full == slug.Slug1) != null)
                                {
                                    slug.Slug1 = sluglist.FirstOrDefault(x => x.Slug_Full == slug.Slug1).Slug1;
                                    slug.Slug_Count = sluglist.Where(x => x.Slug1 == slug.Slug1).Max(m => m.Slug_Count) + 1;
                                    slug.Slug_Full = slug.Slug_Count + "-" + slug.Slug1;
                                }
                                else
                                {
                                    slug.Slug_Count = 1;
                                    slug.Slug_Full = slug.Slug1;
                                }
                            }  
                        }
                        else
                        {
                            Slug slug1 = new Slug();
                            slug1.PostType = posttype.Id;
                            slug1.Lang = item.Code;
                            slug1.Slug1 = fm["code-" + item.Code];
                            slug1.Slug_Full = fm["code-" + item.Code];
                            slug1.Slug_Count = 1;
                            db.Slugs.Add(slug1);
                            db.SaveChanges();
                        }
                    }
                }
                #endregion

                if (attributes != null)
                {
                    int a = attributes.Count();
                    for (int i = 0; i < a; i++)
                    {
                        PostAttribute postattribute = db.PostAttributes.Find(attributes[i]);
                        posttype.PostAttributes.Add(postattribute);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { status = "edit-done" });
            }
            return View(posttype);
        }

        // DELETE Post type
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                PostType posttype = db.PostTypes.Find(id);

                //Delete all posts
                var posts = db.Posts.Where(x => x.PostType.Id == posttype.Id).ToList();
                if (posts.FirstOrDefault() != null)
                {
                    foreach (var item in posts)
                    {
                        db.Slugs.Remove(db.Slugs.FirstOrDefault(x => x.Post == item.Id));

                        var attributes = db.Post_PostAttributes.Where(x => x.Id_Post == item.Id).ToList();
                        if (attributes.FirstOrDefault() != null)
                        {
                            foreach (var attr in attributes)
                            {
                                db.Post_PostAttributes.Remove(attr);
                            }
                        }

                        var menulist = db.MenuItems.Where(x => x.Post == item.Id).ToList();
                        if (menulist.FirstOrDefault() != null)
                        {
                            foreach (var menu in menulist)
                            {
                                menu.Type = 1;
                                menu.Url = "#";
                                menu.Post = null;
                            }
                        }

                        #region Parallel Language
                        var postslang = db.Posts.Where(x => x.MainLang_Id == item.Id).ToList();
                        if (postslang.FirstOrDefault() != null)
                        {
                            foreach (var postlang in postslang)
                            {
                                db.Slugs.Remove(db.Slugs.FirstOrDefault(x => x.Post == postlang.Id));
                                var menulanglist = db.MenuItems.Where(x => x.Post == postlang.Id).ToList();
                                if (menulanglist.FirstOrDefault() != null)
                                {
                                    foreach (var langitem in menulanglist)
                                    {
                                        langitem.Type = 1;
                                        langitem.Url = "#";
                                        langitem.Post = null;
                                    }
                                }
                                db.Posts.Remove(postlang);
                            }
                        }
                        #endregion

                        db.Posts.Remove(item);
                    }
                }

                //Delete all cats
                var cats = db.Cats.Where(x => x.PostType.Id == posttype.Id).ToList();
                if (cats.FirstOrDefault() != null)
                {
                    foreach (var item in cats)
                    {
                        db.Slugs.Remove(db.Slugs.FirstOrDefault(x => x.Cat == item.Id));

                        var catmenulist = db.MenuItems.Where(x => x.Cat == item.Id).ToList();
                        if (catmenulist.FirstOrDefault(x => x.Cat == item.Id) != null)
                        {
                            foreach (var catmenu in catmenulist)
                            {
                                catmenu.Type = 1;
                                catmenu.Url = "#";
                                catmenu.Cat = null;
                            }
                        }

                        #region Parallel Language
                        var catslang = db.Cats.Where(x => x.MainLang_Id == item.Id).ToList();
                        if (catslang.FirstOrDefault() != null)
                        {
                            foreach (var catlang in catslang)
                            {
                                db.Slugs.Remove(db.Slugs.FirstOrDefault(x => x.Cat == catlang.Id));

                                var catlangmenulist = db.MenuItems.Where(x => x.Cat == catlang.Id).ToList();
                                if (catlangmenulist.FirstOrDefault() != null)
                                {
                                    foreach (var langitem in catlangmenulist)
                                    {
                                        langitem.Type = 1;
                                        langitem.Url = "#";
                                        langitem.Cat = null;
                                    }
                                }

                                db.Cats.Remove(catlang);
                            }
                        }
                        #endregion

                        db.Cats.Remove(item);
                    }
                }

                var postrole = db.Roles_PostType.Where(x => x.PostType_Id == id).ToList();
                if (postrole.FirstOrDefault() != null)
                {
                    foreach (var item in postrole)
                    {
                        db.Roles_PostType.Remove(item);
                    }
                }

                var slugs = db.Slugs.Where(x => x.PostType == id).ToList();
                foreach (var item in slugs)
                {
                    db.Slugs.Remove(item);
                }

                var typenames = db.PostType_Names.Where(x => x.Posttype_id == id).ToList();
                foreach (var item in typenames) {
                    db.PostType_Names.Remove(item);
                }

                var menuitem = db.MenuItems.Where(x => x.PostType == id).ToList();
                foreach (var item in menuitem) {
                    item.PostType = null;
                    item.Type = 1;
                    item.Url = "#";
                }

                db.PostTypes.Remove(posttype);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Posttype Roles Access
        public ActionResult PostTypeRoles(string status, int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            foreach (var item in roles)
            {
                var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                var roleaccess = db.Roles_Access.Find(roleid);
                if (roleaccess.Admin == false || roleaccess.PixelAdmin == false)
                {
                    return RedirectToAction("Error", "Dashboard");
                }
            }
            #endregion
            PostType posttype = db.PostTypes.Find(id);
            if (posttype == null)
            {
                return HttpNotFound();
            }

            /* Roles - PostType Permisstion */
            var rolelist = db.webpages_Roles.ToList();
            var posttyperole = db.Roles_PostType.Where(x => x.PostType_Id == id).ToList();
            if (posttyperole.FirstOrDefault() != null)
            {
                foreach (var item in rolelist)
                {
                    if (posttyperole.FirstOrDefault(x => x.Role_Id == item.RoleId) == null)
                    {
                        var posttyperoletb = new Roles_PostType();
                        posttyperoletb.PostType_Id = id;
                        posttyperoletb.Role_Id = item.RoleId;
                        posttyperoletb.Create = true;
                        posttyperoletb.View = true;
                        posttyperoletb.Edit = true;
                        posttyperoletb.Delete = true;
                        posttyperoletb.CatCreate = true;
                        posttyperoletb.CatView = true;
                        posttyperoletb.CatEdit = true;
                        posttyperoletb.CatDelete = true;
                        db.Roles_PostType.Add(posttyperoletb);
                    }
                }
            }
            else
            {
                foreach (var item in rolelist)
                {
                    var posttyperoletb = new Roles_PostType();
                    posttyperoletb.PostType_Id = id;
                    posttyperoletb.Role_Id = item.RoleId;
                    posttyperoletb.Create = true;
                    posttyperoletb.View = true;
                    posttyperoletb.Edit = true;
                    posttyperoletb.Delete = true;
                    posttyperoletb.CatCreate = true;
                    posttyperoletb.CatView = true;
                    posttyperoletb.CatEdit = true;
                    posttyperoletb.CatDelete = true;
                    db.Roles_PostType.Add(posttyperoletb);
                }
            }
            db.SaveChanges();

            ViewBag.Status = status;

            return View(posttype);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostTypeRoles(FormCollection fm, int id = 0)
        {
            var postroles = db.Roles_PostType.Where(x => x.PostType_Id == id).OrderBy(o => o.Role_Id).ToList();

            foreach (var item in postroles)
            {
                var value = fm["View-" + item.Role_Id];
                if (fm["View-" + item.Role_Id] == "true")
                {
                    item.View = true;
                }
                else
                {
                    item.View = false;
                }

                if (fm["Create-" + item.Role_Id] == "true")
                {
                    item.Create = true;
                }
                else
                {
                    item.Create = false;
                }

                if (fm["Delete-" + item.Role_Id] == "true")
                {
                    item.Delete = true;
                }
                else
                {
                    item.Delete = false;
                }

                if (fm["Edit-" + item.Role_Id] == "true")
                {
                    item.Edit = true;
                }
                else
                {
                    item.Edit = false;
                }
                if (fm["CatView-" + item.Role_Id] == "true")
                {
                    item.CatView = true;
                }
                else
                {
                    item.CatView = false;
                }

                if (fm["CatCreate-" + item.Role_Id] == "true")
                {
                    item.CatCreate = true;
                }
                else
                {
                    item.CatCreate = false;
                }

                if (fm["CatDelete-" + item.Role_Id] == "true")
                {
                    item.CatDelete = true;
                }
                else
                {
                    item.CatDelete = false;
                }

                if (fm["CatEdit-" + item.Role_Id] == "true")
                {
                    item.CatEdit = true;
                }
                else
                {
                    item.CatEdit = false;
                }
            }

            db.SaveChanges();

            return RedirectToAction("PostTypeRoles", new { id = id, status = "edit-done" });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}