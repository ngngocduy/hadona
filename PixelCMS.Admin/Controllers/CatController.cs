using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using PagedList;
using PixelCMS.Filters;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / CatController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class CatController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        List<SelectListItem> items = new List<SelectListItem>();

        //
        // GET: /Admin/Cat/

        public ActionResult Index(string status, string culture, int? page, int type = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }

                    var postper = db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == type && x.Role_Id == roleid);
                    if (postper.CatView == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                    if (postper.CatCreate == false)
                    {
                        ViewBag.CatCreatePermission = "false";
                    }
                    if (postper.CatEdit == false)
                    {
                        ViewBag.CatEditPermission = "false";
                    }
                    if (postper.CatDelete == false)
                    {
                        ViewBag.CatDeletePermission = "false";
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion

            var posttype = db.PostTypes.Find(type);
            if (posttype == null)
            {
                return RedirectToAction("Index", new { type = db.PostTypes.FirstOrDefault(x => x.Active == true && x.Has_Cat == true).Id });
            }
            ViewBag.Status = status;
            // Language
            var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Code, x.Name }).ToList();
            if (culture != null)
            {
                var currentlang = langlist.FirstOrDefault(x => x.Code == culture);
                if (currentlang != null)
                {
                    ViewBag.CurrentLang = currentlang.Code;
                    ViewBag.CurrentLangName = currentlang.Name;
                }
                else
                {
                    return RedirectToAction("Error", "Dashboard");
                }
            }
            else
            {
                var currentlang = langlist.FirstOrDefault();
                culture = currentlang.Code;
                ViewBag.CurrentLang = currentlang.Code;
            }
            ViewBag.Name = posttype.Name;
            ViewBag.Description = posttype.Description;
            ViewBag.Type = posttype.Id;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(db.Cats.Where(x => x.Type == type && x.Lang == culture && x.Level == 1).OrderBy(x => x.Order).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SubCatList(int id, string cateditpermission, string catdeletepermission)
        {
            var cats = db.Cats.Where(x => x.Owner_Id == id).OrderBy(o => o.Order).ToList();
            if (catdeletepermission == "false")
            {
                ViewBag.CatDeletePermission = catdeletepermission;
            }
            if (cateditpermission == "false")
            {
                ViewBag.CatEditPermission = cateditpermission;
            }

            return PartialView("_SubCatList", cats);
        }

        //
        // GET: /Admin/Cat/Create

        public ActionResult Create(string culture, int type = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == type && x.Role_Id == roleid).CatCreate == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }

                    if (db.Roles_Access.Find(roleid).Header_Html == false)
                    {
                        ViewBag.HeaderHtml_Per = "false";
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion

            var posttype = db.PostTypes.Find(type);
            if (posttype == null)
            {
                return RedirectToAction("Index", new { type = db.PostTypes.FirstOrDefault(x => x.Active == true && x.Has_Cat == true).Id });
            }
            ViewBag.Type = posttype.Id;

            /*List<SelectListItem> items = new List<SelectListItem>();*/
            // Language
            var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name, x.Flag }).OrderBy(x => x.Order).ToList();
            if (langlist.Count >= 1)
            {
                if (culture != null)
                {
                    var lang = langlist.FirstOrDefault(x => x.Code == culture);
                    if (lang != null)
                    {
                        ViewBag.CurrentLangName = lang.Name;
                        ViewBag.CurrentLangFlag = lang.Flag;
                        ViewBag.CurrentLangCode = lang.Code;
                        /*var owners = db.Cats.Where(c => c.Type == type && c.Level < 3 && c.Lang == lang.Code).OrderBy(o => o.Order).ToList();
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
                        }*/
                        //ViewBag.Owner_Id = items;

                        //new
                        var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == type && c.Lang == lang.Code).OrderByDescending(x => x.Order).ToList();
                        foreach (var item in list)
                        {
                            string bullet = "---";

                            items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                            bullet += "---";
                            Drrdown(item.Id, bullet, null);
                        }

                        ViewBag.Owner_Id = new SelectList(items, "Value", "Text");


                    }
                    else
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
                else
                {
                    var lang = langlist.FirstOrDefault();
                    if (langlist.Count > 1)
                    {
                        ViewBag.CurrentLangName = lang.Name;
                        ViewBag.CurrentLangFlag = lang.Flag;
                        ViewBag.CurrentLangCode = lang.Code;
                    }
                    //old
                    /* var owners = db.Cats.Where(c => c.Type == type && c.Level < 3 && c.Lang == lang.Code).OrderBy(o => o.Order).ToList();
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
                    //new
                    var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == type && c.Lang == lang.Code).OrderByDescending(x => x.Order).ToList();
                    foreach (var item in list)
                    {
                        string bullet = "---";

                        items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                        bullet += "---";
                        Drrdown(item.Id, bullet, null);
                    }

                    ViewBag.Owner_Id = new SelectList(items, "Value", "Text");


                }
            }
            else
            {
                return RedirectToAction("Error", "Dashboard");
            }

            return View();
        }

        //
        // POST: /Admin/Cat/Create

        [HttpPost]
        public ActionResult Create(Cat cat, string culture, FormCollection fm, int type = 0)
        {
            if (ModelState.IsValid)
            {
                #region Code check (Create)
                if (fm["Slug"] != null)
                {
                    Slug slug = new Slug();
                    slug.Cat = cat.Id;
                    slug.Slug1 = fm["Slug"];
                    //Check slug
                    var sluglist = db.Slugs.Where(x => x.Id != slug.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.Post }).ToList();
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
                #endregion

                var posttype = db.PostTypes.Find(type);
                // Language
                var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name }).OrderBy(x => x.Order).ToList();
                if (culture != null)
                {
                    var lang = langlist.FirstOrDefault(x => x.Code == culture);
                    cat.Lang = lang.Code;
                }
                else
                {
                    var lang = langlist.FirstOrDefault();
                    cat.Lang = lang.Code;
                }

                // Type
                cat.Type = type;

                if (posttype.Has_Active == false)
                {
                    cat.Active = true;
                }

                // Owner
                if (cat.Owner_Id != null)
                {
                    cat.Level = ((int)db.Cats.Single(c => c.Id == cat.Owner_Id).Level) + 1;
                }
                else
                {
                    cat.Level = 1;
                }
                if (cat.Owner_Id != null)
                {
                    cat.Parent_Path = cat.Owner_Id + "," + db.Cats.FirstOrDefault(c => c.Id == cat.Owner_Id).Parent_Path;
                }

                db.Cats.Add(cat);
                db.SaveChanges();

                #region Parallel Language
                if (db.PostTypes.Find(type).Has_Parallel_Language == true)
                {
                    Cat cat2 = new Cat();
                    Slug slug2 = new Slug();
                    foreach (var langcat in langlist.Where(x => x.Code != cat.Lang))
                    {
                        cat2.Lang = langcat.Code;
                        cat2.Name = fm["Name-" + langcat.Code];
                        cat2.MainLang_Id = cat.Id;
                        cat2.Type = cat.Type;
                        cat2.Level = cat.Level;
                        cat2.Description = fm["Description-" + langcat.Code];

                        #region Code check (Create)
                        if (fm["Slug-" + langcat.Code] != null)
                        {
                            slug2.Cat = cat2.Id;
                            slug2.Slug1 = fm["Slug-" + langcat.Code];
                            //Check slug
                            var sluglist2 = db.Slugs.Where(x => x.Id != slug2.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.Post }).ToList();
                            if (sluglist2.FirstOrDefault(x => x.Slug1 == slug2.Slug1) != null)
                            {
                                slug2.Slug_Count = sluglist2.Where(x => x.Slug1 == slug2.Slug1).Max(m => m.Slug_Count) + 1;
                                slug2.Slug_Full = slug2.Slug_Count + "-" + slug2.Slug1;
                            }
                            else if (sluglist2.FirstOrDefault(x => x.Slug_Full == slug2.Slug1) != null)
                            {
                                slug2.Slug1 = sluglist2.FirstOrDefault(x => x.Slug_Full == slug2.Slug1).Slug1;
                                slug2.Slug_Count = sluglist2.Where(x => x.Slug1 == slug2.Slug1).Max(m => m.Slug_Count) + 1;
                                slug2.Slug_Full = slug2.Slug_Count + "-" + slug2.Slug1;
                            }
                            else
                            {
                                slug2.Slug_Count = 1;
                                slug2.Slug_Full = slug2.Slug1;
                            }
                            db.Slugs.Add(slug2);
                        }
                        #endregion

                        db.Cats.Add(cat2);
                        db.SaveChanges();
                    }
                }
                #endregion

                return RedirectToAction("Index", new { type = type, status = "create-done" });
            }

            return View(cat);
        }

        //
        // GET: /Admin/Cat/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Cat cat = db.Cats.Find(id);
            if (cat == null)
            {
                return RedirectToAction("Error", "Dashboard");
            }

            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == cat.PostType.Id && x.Role_Id == roleid).CatEdit == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }

                    if (db.Roles_Access.Find(roleid).Header_Html == false)
                    {
                        ViewBag.HeaderHtml_Per = "false";
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion

            var posttype = db.PostTypes.Find(cat.Type);
            ViewBag.Type = posttype.Id;
            /*List<SelectListItem> items = new List<SelectListItem>();
            var owners = db.Cats.Where(c => c.Type == cat.Type && c.Level < 3 && c.Id != cat.Id && c.Lang == cat.Lang).OrderBy(o => o.Order).ToList();
            int a = owners.Count();
            for (int i = 0; i < a; i++)
            {
                if (owners[i].Level == 1)
                {
                    if (owners[i].Id == cat.Owner_Id)
                    {
                        items.Add(new SelectListItem { Text = owners[i].Name, Value = owners[i].Id.ToString(), Selected = true });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = owners[i].Name, Value = owners[i].Id.ToString() });
                    }
                    for (int j = 0; j < a; j++)
                    {
                        if (owners[j].Level == 2 && owners[j].Owner_Id == owners[i].Id)
                        {
                            if (owners[j].Id == cat.Owner_Id)
                            {
                                items.Add(new SelectListItem { Text = "-- " + owners[j].Name, Value = owners[j].Id.ToString(), Selected = true });
                            }
                            else
                            {
                                items.Add(new SelectListItem { Text = "-- " + owners[j].Name, Value = owners[j].Id.ToString() });
                            }
                            for (int k = 0; k < a; k++)
                            {
                                if (owners[k].Level == 3 && owners[k].Owner_Id == owners[j].Id)
                                {
                                    if (owners[k].Id == cat.Owner_Id)
                                    {
                                        items.Add(new SelectListItem { Text = "--- " + owners[k].Name, Value = owners[k].Id.ToString(), Selected = true });
                                    }
                                    else
                                    {
                                        items.Add(new SelectListItem { Text = "--- " + owners[k].Name, Value = owners[k].Id.ToString() });
                                    }

                                }
                            }
                        }
                    }
                }
            }
            ViewBag.Owner_Id = items;*/

            //new
            var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == cat.Type && c.Lang == cat.Lang).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list.Where(x => x.Id != id))
            {
                string bullet = "---";

                items.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                bullet += "-----";
                Drrdown(item.Id, bullet, id);
            }

            ViewBag.Owner_Id = new SelectList(items, "Value", "Text", cat.Owner_Id);

            return View(cat);
        }

        //
        // POST: /Admin/Cat/Edit/5

        [HttpPost]
        public ActionResult Edit(Cat cat, FormCollection fm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cat).State = EntityState.Modified;
                int type = int.Parse(fm["Type"]);

                #region Code check (Edit)
                if (fm["Slug"] != null)
                {
                    var oldslug = db.Slugs.Where(x => x.Cat1.Id == cat.Id).FirstOrDefault();
                    if (oldslug.Slug_Full != fm["Slug"])
                    {
                        //Remove old slug
                        db.Slugs.Remove(oldslug);
                        db.SaveChanges();
                        //Add new slug
                        Slug slug = new Slug();
                        slug.Cat = cat.Id;
                        slug.Slug1 = fm["Slug"];
                        //Check slug
                        var sluglist = db.Slugs.Where(x => x.Id != slug.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.Post }).ToList();
                        if (sluglist.Where(x => x.Slug1 == slug.Slug1).FirstOrDefault() != null)
                        {
                            slug.Slug_Count = sluglist.Where(x => x.Slug1 == slug.Slug1).Max(m => m.Slug_Count) + 1;
                            slug.Slug_Full = slug.Slug_Count + "-" + slug.Slug1;
                        }
                        else if (sluglist.Where(x => x.Slug_Full == slug.Slug1).FirstOrDefault() != null)
                        {
                            slug.Slug1 = sluglist.Where(x => x.Slug_Full == slug.Slug1).FirstOrDefault().Slug1;
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

                // Owner
                if (cat.Owner_Id != null)
                {
                    cat.Level = ((int)db.Cats.Single(c => c.Id == cat.Owner_Id).Level) + 1;
                }
                else
                {
                    cat.Level = 1;
                }

                //Parent_Path Level
                if (cat.Owner_Id != null)
                {
                    cat.Parent_Path = cat.Owner_Id + "," + db.Cats.FirstOrDefault(c => c.Id == cat.Owner_Id).Parent_Path;
                }
                foreach (var item in db.Cats.Where(x => x.Owner_Id == cat.Id).ToList())
                {
                    var a = db.Cats.FirstOrDefault(c => c.Id == item.Owner_Id);

                    item.Parent_Path = item.Owner_Id + "," + (a != null ? a.Parent_Path : "");
                    item.Level = cat.Level + 1;
                    Owner(item.Id);
                }


                #region Parallel Language
                if (db.PostTypes.Find(type).Has_Parallel_Language == true)
                {
                    var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name }).OrderBy(x => x.Order).ToList();
                    var cated = db.Cats.Where(x => x.MainLang_Id == cat.Id).ToList();


                    foreach (var langcat in langlist.Where(x => x.Code != cat.Lang))
                    {
                        var cat1 = cated.Where(x => x.Lang == langcat.Code).FirstOrDefault();
                        if (cat1 != null)
                        {
                            cat1.Name = fm["Name-" + langcat.Code];
                            cat1.Level = cat.Level;
                            cat1.Description = fm["Description-" + langcat.Code];
                            #region Code check (Edit)
                            if (fm["Slug-" + langcat.Code] != null)
                            {
                                var oldslug = db.Slugs.Where(x => x.Cat1.Id == cat1.Id).FirstOrDefault();
                                if (oldslug.Slug_Full != fm["Slug-" + langcat.Code])
                                {
                                    //Remove old slug
                                    db.Slugs.Remove(oldslug);
                                    db.SaveChanges();
                                    //Add new slug
                                    Slug slug2 = new Slug();
                                    slug2.Cat = cat1.Id;
                                    slug2.Slug1 = fm["Slug-" + langcat.Code];
                                    //Check slug
                                    var sluglist2 = db.Slugs.Where(x => x.Id != slug2.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.Post }).ToList();
                                    if (sluglist2.Where(x => x.Slug1 == slug2.Slug1).FirstOrDefault() != null)
                                    {
                                        slug2.Slug_Count = sluglist2.Where(x => x.Slug1 == slug2.Slug1).Max(m => m.Slug_Count) + 1;
                                        slug2.Slug_Full = slug2.Slug_Count + "-" + slug2.Slug1;
                                    }
                                    else if (sluglist2.Where(x => x.Slug_Full == slug2.Slug1).FirstOrDefault() != null)
                                    {
                                        slug2.Slug1 = sluglist2.Where(x => x.Slug_Full == slug2.Slug1).FirstOrDefault().Slug1;
                                        slug2.Slug_Count = sluglist2.Where(x => x.Slug1 == slug2.Slug1).Max(m => m.Slug_Count) + 1;
                                        slug2.Slug_Full = slug2.Slug_Count + "-" + slug2.Slug1;
                                    }
                                    else
                                    {
                                        slug2.Slug_Count = 1;
                                        slug2.Slug_Full = slug2.Slug1;
                                    }
                                    db.Slugs.Add(slug2);
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            Cat cat2 = new Cat();
                            Slug slug2 = new Slug();
                                cat2.Lang = langcat.Code;
                                cat2.Name = fm["Name-" + langcat.Code];
                                cat2.MainLang_Id = cat.Id;
                                cat2.Type = cat.Type;
                                cat2.Level = cat.Level;
                                cat2.Description = fm["Description-" + langcat.Code];

                                #region Code check (Create)
                                if (fm["Slug-" + langcat.Code] != null)
                                {
                                    slug2.Cat = cat2.Id;
                                    slug2.Slug1 = fm["Slug-" + langcat.Code];
                                    //Check slug
                                    var sluglist2 = db.Slugs.Where(x => x.Id != slug2.Id).Select(x => new { x.Id, x.Slug1, x.Slug_Count, x.Slug_Full, x.Post }).ToList();
                                    if (sluglist2.Where(x => x.Slug1 == slug2.Slug1).FirstOrDefault() != null)
                                    {
                                        slug2.Slug_Count = sluglist2.Where(x => x.Slug1 == slug2.Slug1).Max(m => m.Slug_Count) + 1;
                                        slug2.Slug_Full = slug2.Slug_Count + "-" + slug2.Slug1;
                                    }
                                    else if (sluglist2.Where(x => x.Slug_Full == slug2.Slug1).FirstOrDefault() != null)
                                    {
                                        slug2.Slug1 = sluglist2.Where(x => x.Slug_Full == slug2.Slug1).FirstOrDefault().Slug1;
                                        slug2.Slug_Count = sluglist2.Where(x => x.Slug1 == slug2.Slug1).Max(m => m.Slug_Count) + 1;
                                        slug2.Slug_Full = slug2.Slug_Count + "-" + slug2.Slug1;
                                    }
                                    else
                                    {
                                        slug2.Slug_Count = 1;
                                        slug2.Slug_Full = slug2.Slug1;
                                    }
                                    db.Slugs.Add(slug2);
                                }
                                #endregion

                                db.Cats.Add(cat2);

                        }
                    }
                }
                #endregion

                db.SaveChanges();
                return RedirectToAction("Index", new { type = cat.Type, status = "edit-done" });
            }
            return View(cat);
        }

        //
        // GET: /Admin/Cat/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Cat cat = db.Cats.Find(id);
            var catcheck = db.Cats.Where(c => c.Owner_Id == cat.Id).Count();
            if (catcheck != 0)
            {
                return RedirectToAction("Error", new { slug = "owner", type = cat.Type });
            }
            if (cat == null)
            {
                return RedirectToAction("Error", "Dashboard");
            }

            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    if (db.Roles_Access.Find(roleid).Admin == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == cat.PostType.Id && x.Role_Id == roleid).CatDelete == false)
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

            var posttype = db.PostTypes.Find(cat.Type);
            ViewBag.Name = posttype.Name;
            ViewBag.Description = posttype.Description;
            ViewBag.Type = posttype.Id;
            return View(cat);
        }

        //
        // POST: /Admin/Cat/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Cat cat = db.Cats.Find(id);
            int type = (int)cat.Type;
            Slug slug = db.Slugs.FirstOrDefault(x => x.Cat == id);
            db.Slugs.Remove(slug);

            var catmenulist = db.MenuItems.Where(x => x.Cat == cat.Id).ToList();
            if (catmenulist.FirstOrDefault() != null)
            {
                foreach (var item in catmenulist)
                {
                    item.Type = 1;
                    item.Url = "#";
                    item.Cat = null;
                }
            }

            #region Parallel Language
            var catslang = db.Cats.Where(x => x.MainLang_Id == id).ToList();
            if (catslang.FirstOrDefault() != null)
            {
                foreach (var catlang in catslang)
                {
                    db.Slugs.Remove(db.Slugs.FirstOrDefault(x => x.Cat == catlang.Id));

                    var catlangmenulist = db.MenuItems.Where(x => x.Cat == catlang.Id).ToList();
                    if (catlangmenulist.FirstOrDefault() != null)
                    {
                        foreach (var item in catlangmenulist)
                        {
                            item.Type = 1;
                            item.Url = "#";
                            item.Cat = null;
                        }
                    }

                    db.Cats.Remove(catlang);
                }
            }
            #endregion

            db.Cats.Remove(cat);
            db.SaveChanges();
            return RedirectToAction("Index", new { type = type, status = "delete-done" });
        }

        public ActionResult Error(string slug, int type)
        {
            var posttype = db.PostTypes.Find(type);
            if (posttype == null)
            {
                return RedirectToAction("Index", new { type = db.PostTypes.Where(x => x.Active == true && x.Has_Cat == true).FirstOrDefault().Id });
            }
            ViewBag.Name = posttype.Name;
            ViewBag.Description = posttype.Description;
            ViewBag.Type = posttype.Id;
            if (slug == "fail-level")
            {
                ViewBag.Error = Resources.Resources.admin_cat_deletelevel;
            }
            else if (slug == "owner")
            {
                ViewBag.Error = Resources.Resources.admin_cat_deleteowner;
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region Load category
        public void Drrdown(Int32 catid, string bullet, int? id)
        {

            var listchild = db.Cats.Where(x => x.Owner_Id == catid).OrderByDescending(x => x.Order).ToList();
            if (listchild.Count > 0)
            {
                foreach (var item in listchild.Where(x => id != null ? (x.Id != id) : true))
                {
                    string bullet1 = "---";

                    items.Add(new SelectListItem { Text = bullet + item.Name, Value = item.Id.ToString() });

                    Drrdown(item.Id, bullet + bullet1, id);

                }

            }
        }
        /// <summary>
        /// Owners the specified catid.
        /// </summary>
        /// <param name="catid">The catid.</param>
        /// <author> Ba Luan </author>
        /// <created> 5/20/2015 9:44 AM </created>
        public void Owner(Int32 catid)
        {
            foreach (var item in db.Cats.Where(x => x.Owner_Id == catid).ToList())
            {
                var a = db.Cats.FirstOrDefault(c => c.Id == item.Owner_Id);

                item.Parent_Path = item.Owner_Id + "," + (a != null ? a.Parent_Path : "");
                item.Level = item.Level + 1;
                Owner(item.Id);
            }
        }
        #endregion
    }
}