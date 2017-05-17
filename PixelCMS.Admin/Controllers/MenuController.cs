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
// Admin Area / MenuController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class MenuController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        List<SelectListItem> items = new List<SelectListItem>();
        List<SelectListItem> itemscat = new List<SelectListItem>();
        string currentculture = Helpers.CultureHelper.GetCurrentNeutralCulture();
        #region Menu
        //
        // GET: /Admin/Menu/
        public ActionResult Index(string culture, string status)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Menu == false)
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
            // Language
            var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Code, x.Name }).ToList();
            if (culture != null)
            {
                var currentlang = langlist.Where(x => x.Code == culture).FirstOrDefault();
                if (currentlang != null)
                {
                    ViewBag.CurrentLang = currentlang.Code;
                    ViewBag.CurrentLangName = currentlang.Name;
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                var currentlang = langlist.FirstOrDefault();
                culture = currentlang.Code;
                ViewBag.CurrentLang = currentlang.Code;
                ViewBag.CurrentLangName = currentlang.Name;
            }

            ViewBag.Status = status;
            var menus = db.Menus.Where(x => x.Lang == culture).ToList();
            return View(menus);
        }

        // Create Menu
        public ActionResult Create(string culture)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Menu == false)
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
            // Menu Position List
            var menupositions = db.MenuPositions.ToList();
            List<SelectListItem> menupositionlist = new List<SelectListItem>();
            foreach (var item in menupositions)
            {
                if (item.Active == false)
                {
                    menupositionlist.Add(new SelectListItem { Text = item.Name + " (Chưa kích hoạt)", Value = Convert.ToString(item.Id) });
                }
                else
                {
                    menupositionlist.Add(new SelectListItem { Text = item.Name, Value = Convert.ToString(item.Id) });
                }
            }
            ViewBag.MenuPosition = menupositionlist;
            // Language
            var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name, x.Flag }).OrderBy(x => x.Order).ToList();
            if (langlist.Count >= 1)
            {
                if (culture != null)
                {
                    var lang = langlist.Where(x => x.Code == culture).FirstOrDefault();
                    if (lang != null)
                    {
                        ViewBag.CurrentLangName = lang.Name;
                        ViewBag.CurrentLangFlag = lang.Flag;
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    var lang = langlist.FirstOrDefault();
                    if (langlist.Count > 1)
                    {
                        ViewBag.CurrentLangName = lang.Name;
                        ViewBag.CurrentLangFlag = lang.Flag;
                    }
                }
            }
            else
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Menu menu, string culture)
        {
            // Language
            var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name }).OrderBy(x => x.Order).ToList();
            if (culture != null)
            {
                var lang = langlist.Where(x => x.Code == culture).FirstOrDefault();
                menu.Lang = lang.Code;
            }
            else
            {
                var lang = langlist.FirstOrDefault();
                menu.Lang = lang.Code;
            }
            db.Menus.Add(menu);
            // Check Menu Position
            var menulist = db.Menus.Where(x => x.Lang == menu.Lang).ToList();
            foreach (var item in menulist)
            {
                if (item.MenuPosition == menu.MenuPosition)
                {
                    item.MenuPosition = null;

                }
            }
            db.SaveChanges();
            if (langlist.Count > 1)
            {
                return RedirectToAction("Index", new { culture = menu.Lang, status = "create-done" });
            }
            else
            {
                return RedirectToAction("Index", new { status = "create-done" });
            }
        }

        // EDIT Menu
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
                    if (roleaccess.Admin == false || roleaccess.Menu == false)
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

            Menu menu = db.Menus.Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }

            // Menu Position List
            var menupositions = db.MenuPositions.ToList();
            List<SelectListItem> menupositionlist = new List<SelectListItem>();
            foreach (var item in menupositions)
            {
                if (item.Active == false)
                {
                    if (item.Id == menu.MenuPosition)
                    {
                        menupositionlist.Add(new SelectListItem { Text = item.Name + " (Chưa kích hoạt)", Value = Convert.ToString(item.Id), Selected = true });
                    }
                    else
                    {
                        menupositionlist.Add(new SelectListItem { Text = item.Name + " (Chưa kích hoạt)", Value = Convert.ToString(item.Id), Selected = false });
                    }
                }
                else
                {
                    if (item.Id == menu.MenuPosition)
                    {
                        menupositionlist.Add(new SelectListItem { Text = item.Name, Value = Convert.ToString(item.Id), Selected = true });
                    }
                    else
                    {
                        menupositionlist.Add(new SelectListItem { Text = item.Name, Value = Convert.ToString(item.Id), Selected = false });
                    }
                }
            }
            ViewBag.MenuPosition = menupositionlist;

            // Language
            var langlist = db.Languages.Where(x => x.Code == menu.Lang).Select(x => new { x.Code, x.Name }).FirstOrDefault();
            ViewBag.CurrentLang = langlist.Code;
            ViewBag.CurrentLangName = langlist.Name;

            return View(menu);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Menu menu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menu).State = EntityState.Modified;
                db.SaveChanges();
                // Check Menu Position
                var menulist = db.Menus.Where(x => x.Lang == menu.Lang && x.Id != menu.Id).ToList();
                foreach (var item in menulist)
                {
                    if (item.MenuPosition == menu.MenuPosition)
                    {
                        item.MenuPosition = null;
                    }
                }
                db.SaveChanges();
                if (db.Languages.Where(x => x.Active == true).Count() > 1)
                {
                    return RedirectToAction("Index", new { culture = menu.Lang, status = "edit-done" });
                }
                else
                {
                    return RedirectToAction("Index", new { status = "edit-done" });
                }
            }
            return View(menu);
        }

        // DELETE Menu
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Menu menu = db.Menus.Find(id);

                var menuitem = db.MenuItems.Where(x => x.Menu == menu.Id).ToList();

                if (menuitem.FirstOrDefault() != null)
                {
                    foreach (var item in menuitem)
                    {
                        db.MenuItems.Remove(item);
                    }
                }

                db.Menus.Remove(menu);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region MenuPotision
        public ActionResult MenuPosition(string status)
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
            var menupositions = db.MenuPositions.ToList();
            return View(menupositions);
        }

        //Create Menu Position
        public ActionResult CreateMenuPosition()
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
        public ActionResult CreateMenuPosition(MenuPosition menuposition)
        {
            if (ModelState.IsValid)
            {
                db.MenuPositions.Add(menuposition);
                db.SaveChanges();
                return RedirectToAction("MenuPosition", new { status = "create-done" });
            }
            return View(menuposition);
        }

        //Edit Menu Position
        public ActionResult EditMenuPosition(int id = 0)
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
            MenuPosition menuposition = db.MenuPositions.Find(id);
            return View(menuposition);
        }

        [HttpPost]
        public ActionResult EditMenuPosition(MenuPosition menuposition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuposition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MenuPosition", new { status = "edit-done" });
            }
            return View(menuposition);
        }

        // DELETE Menu Position
        [HttpPost]
        public ActionResult DeleteMenuPosition(int id)
        {
            try
            {
                MenuPosition menuposition = db.MenuPositions.Find(id);
                db.MenuPositions.Remove(menuposition);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }
        #endregion

        #region MenuItem
        public ActionResult MenuItem(string status, int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Menu == false)
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
            var menu = db.Menus.Find(id);
            ViewBag.MenuName = menu.Name;
            ViewBag.MenuLang = db.Languages.Where(x => x.Code == menu.Lang).FirstOrDefault().Name;
            ViewBag.MenuLangCode = menu.Lang;
            ViewBag.MenuId = id;

            ViewBag.Status = status;

            // Owner list
            /*var owners = db.MenuItems.Where(c => c.Menu == menu.Id && c.Level < 3).OrderBy(o => o.Order).ToList();
            int a = owners.Count();
            for (int i = 0; i < a; i++)
            {
                if (owners[i].Level == 1)
                {
                    items.Add(new SelectListItem { Text = owners[i].Name, Value = owners[i].Id.ToString() });
                    for (int j = 0; j < a; j++)
                    {
                        if (owners[j].Level == 2 && owners[j].Owner_Id == owners[i].Id)
                        {
                            items.Add(new SelectListItem { Text = "-- " + owners[j].Name, Value = owners[j].Id.ToString() });
                            for (int k = 0; k < a; k++)
                            {
                                if (owners[k].Level == 3 && owners[k].Owner_Id == owners[j].Id)
                                {
                                    items.Add(new SelectListItem { Text = "--- " + owners[k].Name, Value = owners[k].Id.ToString() });
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.Owner_Id = items;*/
            //new menu
            var list = db.MenuItems.Where(c => c.Owner_Id == null && c.Menu == menu.Id).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list)
            {
                string bullet = "---";

                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                bullet += "-----";
                Drrdown(item.Id, bullet,null);
            }

            ViewBag.Owner_Id = new SelectList(items, "Value", "Text");

            //cat
            var list1 = db.Cats.Where(c => c.Owner_Id == null && c.Lang == menu.Lang).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list1)
            {
                string bullet = "---";

                itemscat.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                bullet += "---";
                Drrdowncat(item.Id, bullet);
            }

            ViewBag.cat = new SelectList(itemscat, "Value", "Text");
            // Edit Type List
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "URL", Value = "1" });
            types.Add(new SelectListItem { Text = Resources.Resources.admin_common_posttype, Value = "100" });
            types.Add(new SelectListItem { Text = Resources.Resources.admin_common_page, Value = "2" });
            types.Add(new SelectListItem { Text = Resources.Resources.admin_content_category, Value = "3" });
            types.Add(new SelectListItem { Text = Resources.Resources.common_home, Value = "4" });
            types.Add(new SelectListItem { Text = Resources.Resources.contact, Value = "5" });
            types.Add(new SelectListItem { Text = Resources.Resources.common_search, Value = "6" });
            ViewBag.Type = types;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MenuItem(MenuItem menuitem, int id = 0)
        {
            if (ModelState.IsValid)
            {
                var currentculture = PixelCMS.Helpers.CultureHelper.GetCurrentNeutralCulture();
                // Type
                menuitem.Menu = id;
                if (menuitem.Type == 1)
                {
                    menuitem.Post = null;
                    menuitem.Cat = null;
                    menuitem.PostType = null;
                }
                else if (menuitem.Type == 100)
                {
                    menuitem.Post = null;
                    menuitem.Cat = null;
                    if (menuitem.PostType == null)
                    {
                        menuitem.Type = 1;
                        menuitem.Url = "#";
                        menuitem.Name = "Error";
                        menuitem.Active = false;
                    }
                    else
                    {
                        menuitem.Url = null;
                        menuitem.Name = db.PostType_Names.FirstOrDefault(x => x.Posttype_id == menuitem.PostType && x.Lang == currentculture).Name;
                    }
                }
                else if (menuitem.Type == 2)
                {
                    menuitem.PostType = null;
                    menuitem.Cat = null;
                    if (menuitem.Post == null)
                    {
                        menuitem.Type = 1;
                        menuitem.Url = "#";
                        menuitem.Name = "Error";
                        menuitem.Active = false;
                    }
                    else
                    {
                        menuitem.Url = null;
                        menuitem.Name = db.Posts.Find(menuitem.Post).Title;
                    }
                }
                else if (menuitem.Type == 3)
                {
                    menuitem.Post = null;
                    menuitem.PostType = null;
                    if (menuitem.Cat == null)
                    {
                        menuitem.Type = 1;
                        menuitem.Url = "#";
                        menuitem.Name = "Error";
                        menuitem.Active = false;
                    }
                    else
                    {
                        menuitem.Url = null;
                        menuitem.Name = db.Cats.Find(menuitem.Cat).Name;
                    }
                }
                else if (menuitem.Type == 4 || menuitem.Type == 5 || menuitem.Type == 6)
                {
                    menuitem.Url = null;
                    menuitem.Post = null;
                    menuitem.Cat = null;
                    menuitem.PostType = null;
                    if (menuitem.Type == 4)
                    {
                        menuitem.Name = Resources.Resources.common_home;
                    }
                    else if (menuitem.Type == 5)
                    {
                        menuitem.Name = Resources.Resources.contact;
                    }
                    else if (menuitem.Type == 6)
                    {
                        menuitem.Name = Resources.Resources.common_search;
                    }
                }
                // Owner - Level
                if (menuitem.Owner_Id != null)
                {
                    menuitem.Level = ((int)db.MenuItems.Single(c => c.Id == menuitem.Owner_Id).Level) + 1;
                }
                else
                {
                    menuitem.Level = 1;
                }
                db.MenuItems.Add(menuitem);
                db.SaveChanges();
                return RedirectToAction("MenuItem", new { id = menuitem.Menu, status = "create-done" });
            }
            return View(menuitem);
        }
        public ActionResult EditMenuItem(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Menu == false)
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
            MenuItem menuitem = db.MenuItems.Find(id);
            if (menuitem == null)
            {
                return HttpNotFound();
            }
            ViewBag.MenuName = menuitem.Menu1.Name;
            ViewBag.MenuLang = db.Languages.Where(x => x.Code == menuitem.Menu1.Lang).FirstOrDefault().Name;
            ViewBag.MenuLangCode = menuitem.Menu1.Lang;
            ViewBag.MenuItemName = menuitem.Name;
            ViewBag.MenuId = menuitem.Menu1.Id;
            ViewBag.MenuItemType = menuitem.Type;
            ViewBag.MenuItemPost = menuitem.Post;
            ViewBag.MenuItemCat = menuitem.Cat;
            ViewBag.MenuItemPostType = menuitem.PostType;

            // Owner list
            /*List<SelectListItem> items = new List<SelectListItem>();
            var owners = db.MenuItems.Where(c => c.Menu == menuitem.Menu1.Id && c.Level <= 3).OrderBy(o => o.Order).ToList();
            int a = owners.Count();
            for (int i = 0; i < a; i++)
            {
                if (owners[i].Level == 1)
                {
                    items.Add(new SelectListItem { Text = owners[i].Name, Value = owners[i].Id.ToString() });
                    for (int j = 0; j < a; j++)
                    {
                        if (owners[j].Level == 2 && owners[j].Owner_Id == owners[i].Id)
                        {
                            items.Add(new SelectListItem { Text = "-- " + owners[j].Name, Value = owners[j].Id.ToString() });
                            for (int k = 0; k < a; k++)
                            {
                                if (owners[k].Level == 3 && owners[k].Owner_Id == owners[j].Id)
                                {
                                    items.Add(new SelectListItem { Text = "--- " + owners[k].Name, Value = owners[k].Id.ToString() });
                                }
                            }
                        }
                    }
                }
            }
            foreach (var item in items) {
                if (item.Value == Convert.ToString(menuitem.Owner_Id)) {
                    item.Selected = true;
                }
            }
            ViewBag.Owner_Id = items;*/

            var list = db.MenuItems.Where(c => c.Owner_Id == null && c.Menu == menuitem.Menu1.Id).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list.Where(x=>x.Id!=id))
            {
                string bullet = "---";

                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                bullet += "-----";
                Drrdown(item.Id, bullet,id);
            }

            ViewBag.Owner_Id = new SelectList(items, "Value", "Text", menuitem.Owner_Id);

            //cat
            var list1 = db.Cats.Where(c => c.Owner_Id == null && c.Lang == menuitem.Menu1.Lang).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list1)
            {
                string bullet = "---";

                itemscat.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                bullet += "---";
                Drrdowncat(item.Id, bullet);
            }

            ViewBag.cat = new SelectList(itemscat, "Value", "Text", menuitem.Cat);

            // Edit Type List
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "URL", Value = "1" });
            types.Add(new SelectListItem { Text = Resources.Resources.admin_common_posttype, Value = "100" });
            types.Add(new SelectListItem { Text = Resources.Resources.admin_common_page, Value = "2" });
            types.Add(new SelectListItem { Text = Resources.Resources.admin_content_category, Value = "3" });
            types.Add(new SelectListItem { Text = Resources.Resources.common_home, Value = "4" });
            types.Add(new SelectListItem { Text = Resources.Resources.contact, Value = "5" });
            types.Add(new SelectListItem { Text = Resources.Resources.common_search, Value = "6" });
            foreach (var type in types)
            {
                if (type.Value == Convert.ToString(menuitem.Type))
                {
                    type.Selected = true;
                }
            }
            ViewBag.Type = types;

            return View(menuitem);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMenuItem(MenuItem menuitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuitem).State = EntityState.Modified;
                // Type
                if (menuitem.Type == 1)
                {
                    menuitem.Post = null;
                    menuitem.Cat = null;
                    menuitem.PostType = null;
                }
                else if (menuitem.Type == 100)
                {
                    menuitem.Post = null;
                    menuitem.Cat = null;
                    if (menuitem.PostType == null)
                    {
                        menuitem.Type = 1;
                        menuitem.Url = "#";
                        menuitem.Name = "Error";
                        menuitem.Active = false;
                    }
                    else
                    {
                        menuitem.Url = null;
                    }
                }
                else if (menuitem.Type == 2)
                {
                    menuitem.Cat = null;
                    menuitem.PostType = null;
                    if (menuitem.Post == null)
                    {
                        menuitem.Type = 1;
                        menuitem.Url = "#";
                        menuitem.Name = "Error";
                        menuitem.Active = false;
                    }
                    else
                    {
                        menuitem.Url = null;
                    }
                }
                else if (menuitem.Type == 3)
                {
                    menuitem.Post = null;
                    menuitem.PostType = null;
                    if (menuitem.Cat == null)
                    {
                        menuitem.Type = 1;
                        menuitem.Url = "#";
                        menuitem.Name = "Error";
                        menuitem.Active = false;
                    }
                    else
                    {
                        menuitem.Url = null;
                    }
                }
                else if (menuitem.Type == 4 || menuitem.Type == 5 || menuitem.Type == 6)
                {
                    menuitem.Url = null;
                    menuitem.Post = null;
                    menuitem.Cat = null;
                    menuitem.PostType = null;
                }
                // Owner - Level
                if (menuitem.Owner_Id != null)
                {
                    menuitem.Level = ((int)db.MenuItems.Single(c => c.Id == menuitem.Owner_Id).Level) + 1;
                }
                else
                {
                    menuitem.Level = 1;
                }
                foreach (var item in db.MenuItems.Where(x => x.Owner_Id == menuitem.Id).ToList())
                {
                    item.Level = menuitem.Level + 1;
                    foreach (var item2 in db.MenuItems.Where(x => x.Owner_Id == item.Id).ToList())
                    {
                        item2.Level = item.Level + 1;
                        /*if (item.Level > 3 || item2.Level > 3)
                        {
                            return RedirectToAction("Error", new { slug = "fail-level", menuid = menuitem.Menu });
                        }*/
                    }
                }

                db.SaveChanges();
                return RedirectToAction("MenuItem", new { id = menuitem.Menu, status = "edit-done" });
            }
            return View(menuitem);
        }

        // Delete Menu Item
        public ActionResult DeleteMenuItem(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Menu == false)
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
            MenuItem menuitem = db.MenuItems.Find(id);
            var menuitemcheck = db.MenuItems.Where(c => c.Owner_Id == menuitem.Id).Count();
            if (menuitemcheck != 0)
            {
                return RedirectToAction("Error", new { slug = "owner", menuid = menuitem.Menu });
            }
            if (menuitem == null)
            {
                return HttpNotFound();
            }
            ViewBag.MenuName = menuitem.Menu1.Name;
            ViewBag.MenuLang = db.Languages.Where(x => x.Code == menuitem.Menu1.Lang).FirstOrDefault().Name;
            ViewBag.MenuLangCode = menuitem.Menu1.Lang;
            ViewBag.MenuItemName = menuitem.Name;
            ViewBag.MenuId = menuitem.Menu1.Id;
            return View(menuitem);
        }

        [HttpPost, ActionName("DeleteMenuItem")]
        public ActionResult DeleteMenuItemConfirmed(int id)
        {
            MenuItem menuitem = db.MenuItems.Find(id);
            int menuid = (int)menuitem.Menu;
            db.MenuItems.Remove(menuitem);
            db.SaveChanges();
            return RedirectToAction("MenuItem", new { id = menuid, status = "delete-done" });
        }

        public ActionResult Error(string slug, int menuid)
        {
            var menu = db.Menus.Find(menuid);
            if (menu == null)
            {
                return RedirectToAction("MenuItem", new { id = db.Menus.FirstOrDefault().Id });
            }
            ViewBag.MenuName = menu.Name;
            ViewBag.MenuLang = db.Languages.Where(x => x.Code == menu.Lang).FirstOrDefault().Name;
            ViewBag.MenuLangCode = menu.Lang;
            ViewBag.MenuId = menu.Id;
            if (slug == "fail-level")
            {
                ViewBag.Error = Resources.Resources.admin_menu_deletelevel;
            }
            else if (slug == "owner")
            {
                ViewBag.Error = Resources.Resources.admin_cat_deleteowner;
            }
            return View();
        }

        // MenuItem List
        public ActionResult MenuItemList(int menuid = 0)
        {
            var menuitems = db.MenuItems.Where(x => x.Menu == menuid).OrderBy(x => x.Order).ToList();
            return PartialView("_MenuItemList", menuitems);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region Load menuitem
        public void Drrdown(Int32 catid, string bullet,int?id)
        {
            var listchild = db.MenuItems.Where(x => x.Owner_Id == catid).OrderByDescending(x => x.Order).ToList();
            if (listchild.Count > 0)
            {
                foreach (var item in listchild.Where(x => id != null ? (x.Id != id) : true))
                {
                    string bullet1 = "---";
                    items.Add(new SelectListItem { Text = bullet + item.Name, Value = item.Id.ToString() });
                    Drrdown(item.Id, bullet + bullet1,id);
                }

            }
        }
        #endregion

        #region Load category
        public void Drrdowncat(Int32 catid, string bullet)
        {
            // var cat = db.GetTable<ENEWS_CATEGORy>().Where(x => x.CAT_PARENT_ID == 0).OrderByDescending(x => x.CAT_ORDER);

            var listchild = db.Cats.Where(x => x.Owner_Id == catid).OrderByDescending(x => x.Order).ToList();
            if (listchild.Count > 0)
            {
                foreach (var item in listchild)
                {
                    string bullet1 = "---";

                    itemscat.Add(new SelectListItem { Text = bullet + item.Name, Value = item.Id.ToString() });

                    // var catchildren = db.GetTable<ENEWS_CATEGORy>().Where(x => x.CAT_PARENT_ID == item.CAT_ID);
                    //  bullet += "--------------";
                    Drrdowncat(item.Id, bullet + bullet1);
                }
            }
        }

        #endregion
    }
}
