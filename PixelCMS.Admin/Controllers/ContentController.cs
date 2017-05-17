using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using System.Collections;
using PagedList;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / ContentController.cs v1.2
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        List<MultiSelectList> items = new List<MultiSelectList>();

        #region CONTENT

        #region Index
        public ActionResult Index(string status, string culture, int? page, int? cat, int? type = 0)
        {
            try
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
                        if (postper.View == false)
                        {
                            return RedirectToAction("Error", "Dashboard");
                        }
                        if (postper.Delete == false)
                        {
                            ViewBag.DeletePermission = "false";
                        }
                        if (postper.Edit == false)
                        {
                            ViewBag.EditPermission = "false";
                        }
                        if (postper.Create == false)
                        {
                            ViewBag.CreatePermission = "false";
                        }
                        if (postper.CatView == false)
                        {
                            ViewBag.CatViewPermission = "false";
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                #endregion

                #region Language
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
                    ViewBag.CurrentLangName = currentlang.Name;
                }
                #endregion

                var posttype = db.PostTypes.Find(type);
                if (posttype == null)
                {
                    return RedirectToAction("Index", new { type = db.PostTypes.FirstOrDefault(x => x.Active).Id });
                }

                ViewBag.Status = status;
                ViewBag.Name = posttype.Name;
                ViewBag.Description = posttype.Description;
                ViewBag.Type = posttype.Id;
                if (cat != null)
                {
                    var catfull = db.Cats.Find(cat);
                    if (catfull == null || catfull.PostType.Id != type)
                    {
                        return RedirectToAction("Index", new { type = db.PostTypes.FirstOrDefault(x => x.Active).Id });
                    }
                    ViewBag.Cat = catfull.Name;
                    ViewBag.CatId = catfull.Id;
                    var posts = catfull.Posts.Where(x => x.Active == true && x.Lang == culture && x.Level == 1).OrderByDescending(x => x.Date_Create).ToList();
                    if (posttype.Has_Order == true)
                    {
                        posts = posts.OrderByDescending(x => x.Order).ThenByDescending(x=>x.Date_Create).ToList();
                    }
                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(posts.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var posts = db.Posts.Include(p => p.PostType).Where(x => x.Type == type && x.Lang == culture && x.Level == 1).OrderByDescending(x => x.Date_Create);
                    if (posttype.Has_Order == true)
                    {
                        posts = posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create);
                    }
                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(posts.ToPagedList(pageNumber, pageSize));
                }
            }
            catch
            {
                return RedirectToAction("Error", "Dashboard");
            }
        }

        public ActionResult SubPostList(int id, string deletepermission, string editpermission, string createpermission)
        {
            var posts = db.Posts.Where(x => x.Owner_Id == id).OrderByDescending(o => o.Date_Create);
            if (posts.FirstOrDefault().PostType.Has_Order == true)
            {
                posts = posts.OrderBy(x => x.Order);
            }
            if (deletepermission == "false")
            {
                ViewBag.DeletePermission = deletepermission;
            }
            if (editpermission == "false")
            {
                ViewBag.EditPermission = editpermission;
            }
            if (createpermission == "false")
            {
                ViewBag.CreatePermission = createpermission;
            }
            return PartialView("_SubPostList", posts.ToList());
        }
        #endregion

        [HttpGet]
        public JsonResult getpostlist(int type, string query = "")
        {
            var list = db.Posts.Where(p => p.Title.Contains(query) && p.PostType.Id == type).Select(x => new
            {
                Title = x.Title,
                Id = x.Id,
            }).Take(10).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public enum ButtonType
        {
            Submit,
            SubmitandMedia,
            SubmitandEdit,
            SubmitandCreate
        }

        public class MultipleButtonsAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                Type tButtonType = typeof(ButtonType);
                foreach (var key in filterContext.HttpContext.Request.Form.AllKeys)
                {
                    if (Enum.IsDefined(tButtonType, key))
                    {
                        var pDesc = filterContext.ActionDescriptor.GetParameters()
                            .FirstOrDefault(x => x.ParameterType == tButtonType);
                        filterContext.ActionParameters[pDesc.ParameterName] = Enum.Parse(tButtonType, key);

                    }
                }
            }
        }

        #region Create
        public ActionResult Create(string status, string culture, int type = 0)
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
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == type && x.Role_Id == roleid).Create == false)
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

            #region posttype
            var posttype = db.PostTypes.Find(type);
            if (posttype == null)
            {
                return RedirectToAction("Index", new { type = db.PostTypes.FirstOrDefault(x => x.Active == true).Id });
            }
            ViewBag.Name = posttype.Name;
            ViewBag.Description = posttype.Description;
            ViewBag.Type = posttype.Id;
            #endregion

            ViewBag.Status = status;
            //Language
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
                        var cats = db.Cats.Include(c => c.Cat2).Where(t => t.Type == type && t.Lang == lang.Code).ToList();
                        ViewBag.MultiselectCats = new MultiSelectList(cats, "Id", "Name");
                        // mitiselect categories
                        var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == type && c.Lang == lang.Code).OrderByDescending(x => x.Order).ToList();
                        foreach (var item in list)
                        {
                            string bullet = "---";

                            items.Add(new MultiSelectList(items, item.Id.ToString(), item.Name));
                            bullet += "---";
                            Drrdown(item.Id, bullet);
                        }

                        ViewBag.catss = new MultiSelectList(items, "dataValueField", "dataTextField");

                        if (posttype.Has_Owner)
                        {
                            List<SelectListItem> itemsPost = new List<SelectListItem>();
                            var owners = db.Posts.Where(c => c.Lang == lang.Code && c.Level < 3 && c.Type == type).OrderBy(o => o.Order).ToList();
                            int a = owners.Count();
                            for (int i = 0; i < a; i++)
                            {
                                if (owners[i].Level == 1)
                                {
                                    itemsPost.Add(new SelectListItem { Text = owners[i].Title, Value = owners[i].Id.ToString() });
                                    for (int j = 0; j < a; j++)
                                    {
                                        if (owners[j].Level == 2 && owners[j].Owner_Id == owners[i].Id)
                                        {
                                            itemsPost.Add(new SelectListItem { Text = "-- " + owners[j].Title, Value = owners[j].Id.ToString() });
                                            for (int k = 0; k < a; k++)
                                            {
                                                if (owners[k].Level == 3 && owners[k].Owner_Id == owners[j].Id)
                                                {
                                                    itemsPost.Add(new SelectListItem { Text = "--- " + owners[k].Title, Value = owners[k].Id.ToString() });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ViewBag.Owner_Id = itemsPost;
                        }

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
                    var cats = db.Cats.Include(c => c.Cat2).Where(t => t.Type == type && t.Lang == lang.Code).ToList();
                    ViewBag.MultiselectCats = new MultiSelectList(cats, "Id", "Name");
                    // mitiselect categories
                    var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == type && c.Lang == lang.Code).OrderByDescending(x => x.Order).ToList();
                    foreach (var item in list)
                    {
                        string bullet = "---";

                        items.Add(new MultiSelectList(items, item.Id.ToString(), item.Name));
                        bullet += "---";
                        Drrdown(item.Id, bullet);
                    }

                    ViewBag.catss = new MultiSelectList(items, "dataValueField", "dataTextField");

                    if (posttype.Has_Owner)
                    {
                        List<SelectListItem> itemsPost = new List<SelectListItem>();
                        var owners = db.Posts.Where(c => c.Lang == lang.Code && c.Level < 3 && c.Type == type).OrderBy(o => o.Order).ToList();
                        int a = owners.Count();
                        for (int i = 0; i < a; i++)
                        {
                            if (owners[i].Level == 1)
                            {
                                itemsPost.Add(new SelectListItem { Text = owners[i].Title, Value = owners[i].Id.ToString() });
                                for (int j = 0; j < a; j++)
                                {
                                    if (owners[j].Level == 2 && owners[j].Owner_Id == owners[i].Id)
                                    {
                                        itemsPost.Add(new SelectListItem { Text = "-- " + owners[j].Title, Value = owners[j].Id.ToString() });
                                        for (int k = 0; k < a; k++)
                                        {
                                            if (owners[k].Level == 3 && owners[k].Owner_Id == owners[j].Id)
                                            {
                                                itemsPost.Add(new SelectListItem { Text = "--- " + owners[k].Title, Value = owners[k].Id.ToString() });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ViewBag.Owner_Id = itemsPost;
                    }

                }
            }
            else
            {
                return RedirectToAction("Error", "Dashboard");
            }

            Post model = new Post();
            model.Quantity = 10000;
            //drop manufacturer
            ViewBag.manu = new SelectList(db.Manufacturers, "Id", "Name");
            //model.Product.Inv_min_Quantity = 0; not working
            return View(model);
        }
        //
        // POST: /Admin/Content/Create

        [HttpPost]
        [MultipleButtons]
        [ValidateInput(false)]
        public ActionResult Create(Post post, FormCollection fm, ButtonType buttonPressed, int[] cats, string culture, string tags, int type = 0)
        {
            //drop manufacturer
            ViewBag.manu = new SelectList(db.Manufacturers, "Id", "Name");
            //clear product khi ko đc kick hoạt

            post.Type = type;
            post.Date_Create = DateTime.Now;
            post.User_Create = User.Identity.Name;

            if (db.PostTypes.Find(type).Active_Product == false)
            {
                post.Product = null;
            }
            if (db.PostTypes.Find(type).Has_Title == false)
            {
                post.Title = db.Slugs.FirstOrDefault(x => x.PostType == type).Slug_Full;
            }

            if (db.PostTypes.Find(type).Has_Active == false)
            {
                post.Active = true;
            }

            #region Code check (Create)
            if (fm["Slug"] != null)
            {
                Slug slug = new Slug();
                slug.Post = post.Id;
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
            else if (db.PostTypes.Find(type).Has_Title == false)
            {
                Slug slug = new Slug();
                slug.Post = post.Id;
                slug.Slug1 = post.Title;
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

            // Language
            var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name }).OrderBy(x => x.Order).ToList();
            if (culture != null)
            {
                var lang = langlist.FirstOrDefault(x => x.Code == culture);
                post.Lang = lang.Code;
            }
            else
            {
                var lang = langlist.FirstOrDefault();
                post.Lang = lang.Code;
            }
            // Cats
            if (cats != null)
            {
                int a = cats.Count();
                for (int i = 0; i < a; i++)
                {
                    Cat cat = db.Cats.Find(cats[i]);
                    post.Cats.Add(cat);
                }
            }

            db.Posts.Add(post);

            // Tags
            if (!string.IsNullOrEmpty(tags))
            {
                var ArrayTags = tags.Split(',');
                for (int i = 0; i < ArrayTags.Count(); i++)
                {
                    string tagname = ArrayTags[i];
                    //kiem tra tag đã tồn tại hay chưa
                    var IsEixst = db.PostTags.Any(x => x.Name == tagname);
                    if (IsEixst)//nếu đã có thì add vào post_tag
                    {
                        var tag = db.PostTags.FirstOrDefault(x => x.Name == tagname);
                        post.PostTags.Add(tag);
                    }
                    else//ngược lại thêm từ khóa vào bảng tag và add vào post_tag
                    {
                        //thêm new tag
                        var PostTag = new PostTag();
                        PostTag.Name = tagname;
                        db.PostTags.Add(PostTag);
                        db.SaveChanges();
                        //add vào post_tag
                        var tag = db.PostTags.FirstOrDefault(x => x.Name == tagname);
                        post.PostTags.Add(tag);
                    }


                }
            }

            // Save Attribute    
            for (int i = 0; i < fm.Count; i++)
            {
                if (fm["attribute-" + i] != null && fm["attribute-" + i] != string.Empty)
                {
                    Post_PostAttribute pa = new Post_PostAttribute();
                    pa.Id_Post = post.Id;
                    pa.Id_Attribute = int.Parse(fm["attributeId-" + i]);
                    pa.Value = fm["attribute-" + i];
                    db.Post_PostAttributes.Add(pa);
                }
            }

            if (post.Owner_Id != null)
            {
                post.Level = ((int)db.Posts.Single(c => c.Id == post.Owner_Id).Level) + 1;
            }
            else
            {
                post.Level = 1;
            }

            db.SaveChanges();

            #region Inventory insert by luan
            //nếu product được kick hoạt
            if (post.Product != null)
            {
                //nếu kho đc active
                // if (post.Product.Inv_Active)
                //{
                Inventory inven = new Inventory();
                inven.Agent_Id = 1; //tạm thời lấy id=1 của chi nhánh đầu tiên 
                inven.Product_Id = post.Product.Id;
                inven.Quantity = post.Quantity;
                db.Inventories.Add(inven);
                db.SaveChanges();
                // }
            }

            #endregion

            #region Parallel Language
            if (db.PostTypes.Find(type).Has_Parallel_Language == true)
            {
                Post post2 = new Post();
                Slug slug2 = new Slug();
                foreach (var langcat in langlist.Where(x => x.Code != post.Lang))
                {
                    post2.Date_Create = DateTime.Now;
                    post2.User_Create = User.Identity.Name;

                    post2.Lang = langcat.Code;
                    post2.Title = fm["Title-" + langcat.Code];
                    post2.Description = fm["Description-" + langcat.Code];
                    post2.Content = fm["Content-" + langcat.Code];
                    post2.MainLang_Id = post.Id;
                    post2.Type = post.Type;
                    post2.Active = true;
                    post2.Level = post.Level;

                    post2.Thumb = post.Thumb;

                    #region Code check (Create)
                    if (fm["Slug-" + langcat.Code] != null)
                    {
                        slug2.Post = post2.Id;
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
                    else if (db.PostTypes.Find(type).Has_Title == false)
                    {
                        slug2.Post = post2.Id;
                        slug2.Slug1 = post2.Title;
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

                    db.Posts.Add(post2);
                    db.SaveChanges();
                }
            }
            #endregion

            if (buttonPressed == ButtonType.SubmitandMedia)
            {
                return RedirectToAction("MediaFiles", new { id = post.Id });
            }
            if (buttonPressed == ButtonType.SubmitandEdit)
            {
                return RedirectToAction("Edit", new { id = post.Id });
            }
            if (buttonPressed == ButtonType.SubmitandCreate)
            {
                if (langlist.Count() > 1)
                {
                    return RedirectToAction("Create", new { status = "create-done", type = type, culture = culture });
                }
                else
                {
                    return RedirectToAction("Create", new { status = "create-done", type = type });
                }
            }
            if (langlist.Count() > 1)
            {
                return RedirectToAction("Index", new { status = "create-done", type = type, culture = culture });
            }
            else
            {
                return RedirectToAction("Index", new { status = "create-done", type = type });
            }
        }

        #endregion

        #region Edit
        public ActionResult Edit(string status, int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
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
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == post.PostType.Id && x.Role_Id == roleid).Edit == false)
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

            ViewBag.Status = status;
            // Language
            var langlist = db.Languages.FirstOrDefault(x => x.Code == post.Lang);
            ViewBag.CurrentLang = langlist.Code;
            ViewBag.CurrentLangName = langlist.Name;

            var posttype = db.PostTypes.Find(post.Type);
            ViewBag.Type = posttype.Id;
            ViewBag.PostId = id;
            ArrayList selectedCategories = new ArrayList();
            foreach (var cat in post.Cats)
            {
                selectedCategories.Add(cat.Id).ToString();
            }
            var catitems = db.Cats.Include(c => c.Cat2).Where(t => t.Type == post.Type && t.Lang == post.Lang).ToList();
            ViewBag.MultiselectCats = new MultiSelectList(catitems, "Id", "Name", selectedCategories);//not working

            // mitiselect categories
            var list = db.Cats.Where(c => c.Owner_Id == null && c.Type == post.Type && c.Lang == post.Lang).OrderByDescending(x => x.Order).ToList();
            foreach (var item in list)
            {
                string bullet = "---";

                items.Add(new MultiSelectList(items, item.Id.ToString(), item.Name));
                bullet += "---";
                Drrdown(item.Id, bullet);
            }

            ViewBag.catss = new MultiSelectList(items, "dataValueField", "dataTextField", selectedCategories);


            if (posttype.Has_Owner)
            {
                List<SelectListItem> itemsPost = new List<SelectListItem>();
                var owners = db.Posts.Where(c => c.Lang == post.Lang && c.Level < 3 && c.Type == post.Type).OrderBy(o => o.Order).ToList();
                int a = owners.Count();
                for (int i = 0; i < a; i++)
                {
                    if (owners[i].Level == 1)
                    {
                        itemsPost.Add(new SelectListItem { Text = owners[i].Title, Value = owners[i].Id.ToString() });
                        for (int j = 0; j < a; j++)
                        {
                            if (owners[j].Level == 2 && owners[j].Owner_Id == owners[i].Id)
                            {
                                itemsPost.Add(new SelectListItem { Text = "-- " + owners[j].Title, Value = owners[j].Id.ToString() });
                                for (int k = 0; k < a; k++)
                                {
                                    if (owners[k].Level == 3 && owners[k].Owner_Id == owners[j].Id)
                                    {
                                        itemsPost.Add(new SelectListItem { Text = "--- " + owners[k].Title, Value = owners[k].Id.ToString() });
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in itemsPost)
                {
                    if (item.Value == Convert.ToString(post.Owner_Id))
                    {
                        item.Selected = true;
                    }
                }
                ViewBag.Owner_Id = itemsPost;
            }


            if (post.Product != null)
            {
                var quantity = post.Product.Inventories.FirstOrDefault();
                if (quantity != null)
                    post.Quantity = quantity.Quantity;
                ViewBag.manu = new SelectList(db.Manufacturers, "Id", "Name", post.Product.Supplier_Id);
            }
            return View(post);
        }

        //
        // POST: /Admin/Content/Edit/5

        [HttpPost]
        [MultipleButtons]
        [ValidateInput(false)]
        public ActionResult Edit(Post post, FormCollection fm, ButtonType buttonPressed, string tags, int[] catitems)
        {
            if (ModelState.IsValid)
            {

                if (db.PostTypes.Find(post.Type).Active_Product == false)
                {
                    post.Product = null;
                }
                db.Entry(post).State = EntityState.Modified;
                if (post.Product != null)
                    db.Entry(post.Product).State = EntityState.Modified;

                int type = int.Parse(fm["Type"]);

                #region Code check (Edit)
                if (fm["Slug"] != null)
                {
                    var oldslug = db.Slugs.FirstOrDefault(x => x.Post1.Id == post.Id);
                    if (oldslug.Slug_Full != fm["Slug"])
                    {
                        //Remove old slug
                        db.Slugs.Remove(oldslug);
                        db.SaveChanges();
                        //Add new slug
                        Slug slug = new Slug();
                        slug.Post = post.Id;
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
                }
                #endregion

                // Owner
                if (post.Owner_Id != null)
                {
                    post.Level = ((int)db.Posts.Single(c => c.Id == post.Owner_Id).Level) + 1;
                }
                else
                {
                    post.Level = 1;
                }

                // Delete old cat in post
                var test = db.Posts.Include(p => p.Cats).FirstOrDefault(p => p.Id == post.Id);
                test.Cats.Clear();
                post.User_Edit = User.Identity.Name;
                post.Date_Edit = DateTime.Now;
                //Add new select cat
                if (catitems != null)
                {
                    int a = catitems.Count();
                    for (int i = 0; i < a; i++)
                    {
                        Cat cat = db.Cats.Find(catitems[i]);
                        post.Cats.Add(cat);
                    }
                    db.SaveChanges();
                }
                //delete tags old
                var cleartags = db.Posts.Include(p => p.PostTags).FirstOrDefault(p => p.Id == post.Id);
                cleartags.PostTags.Clear();

                // Tags
                if (!string.IsNullOrEmpty(tags))
                {
                    var ArrayTags = tags.Split(',');
                    for (int i = 0; i < ArrayTags.Count(); i++)
                    {
                        string tagname = ArrayTags[i];
                        //kiem tra tag đã tồn tại hay chưa
                        var IsEixst = db.PostTags.Any(x => x.Name == tagname);
                        if (IsEixst)//nếu đã có thì add vào post_tag
                        {
                            var tag = db.PostTags.FirstOrDefault(x => x.Name == tagname);
                            post.PostTags.Add(tag);
                        }
                        else//ngược lại thêm từ khóa vào bảng tag và add vào post_tag
                        {
                            //thêm new tag
                            var PostTag = new PostTag();
                            PostTag.Name = tagname;
                            db.PostTags.Add(PostTag);
                            db.SaveChanges();
                            //add vào post_tag
                            var tag = db.PostTags.FirstOrDefault(x => x.Name == tagname);
                            post.PostTags.Add(tag);
                        }


                    }
                }

                // Save Attribute
                var attributeinputed = db.Post_PostAttributes.Where(x => x.Id_Post == post.Id).ToList();
                for (int i = 0; i < fm.Count; i++)
                {
                    if (fm["attribute-" + i] != null && fm["attribute-" + i] != string.Empty)
                    {
                        // Delete Old content
                        foreach (var items in attributeinputed)
                        {
                            int attributeid = int.Parse(fm["attributeId-" + i]);
                            var attributevalue = db.Post_PostAttributes.FirstOrDefault(x => x.Id_Post == items.Id_Post && x.Id_Attribute == attributeid);
                            if (attributevalue != null)
                            {
                                db.Post_PostAttributes.Remove(attributevalue);
                                db.SaveChanges();
                            }
                        }
                        // Add new content
                        Post_PostAttribute pa = new Post_PostAttribute();
                        pa.Id_Post = post.Id;
                        pa.Id_Attribute = int.Parse(fm["attributeId-" + i]);
                        pa.Value = fm["attribute-" + i];
                        db.Post_PostAttributes.Add(pa);
                    }
                    else if (fm["attribute-" + i] != null)
                    {
                        foreach (var items in attributeinputed)
                        {
                            int attributeid = int.Parse(fm["attributeId-" + i]);
                            var attributevalue = db.Post_PostAttributes.FirstOrDefault(x => x.Id_Post == items.Id_Post && x.Id_Attribute == attributeid);
                            if (attributevalue != null)
                            {
                                db.Post_PostAttributes.Remove(attributevalue);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                db.SaveChanges();

                #region Parallel Language
                if (db.PostTypes.Find(type).Has_Parallel_Language == true)
                {
                    var langlist = db.Languages.Where(x => x.Active == true).Select(x => new { x.Id, x.Code, x.Order, x.Name }).OrderBy(x => x.Order).ToList();
                    var posted = db.Posts.Where(x => x.MainLang_Id == post.Id).ToList();


                    foreach (var langpost in langlist.Where(x => x.Code != post.Lang))
                    {
                        if (posted.FirstOrDefault(x => x.Lang == langpost.Code) == null)
                        {
                            Post post2 = new Post();
                            Slug slug2 = new Slug();
                            post2.Lang = langpost.Code;
                            post2.Title = fm["Title-" + langpost.Code];
                            post2.Description = fm["Description-" + langpost.Code];
                            post2.Content = fm["Content-" + langpost.Code];
                            post2.MainLang_Id = post.Id;
                            post2.Type = post.Type;
                            post2.Active = true;
                            post2.Level = post.Level;
                            post2.Thumb = post.Thumb;

                            #region Code check (Create)
                            if (fm["Slug-" + langpost.Code] != null)
                            {
                                slug2.Post = post2.Id;
                                slug2.Slug1 = fm["Slug-" + langpost.Code];
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
                            else if (db.PostTypes.Find(type).Has_Title == false)
                            {
                                slug2.Post = post2.Id;
                                slug2.Slug1 = post2.Title;
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

                            db.Posts.Add(post2);
                        }
                        else
                        {
                            var post2 = posted.FirstOrDefault(x => x.Lang == langpost.Code);

                            post2.Title = fm["Title-" + langpost.Code];
                            post2.Description = fm["Description-" + langpost.Code];
                            post2.Content = fm["Content-" + langpost.Code];
                            post2.Level = post.Level;
                            post2.Thumb = post.Thumb;

                            #region Code check (Edit)
                            if (fm["Slug-" + langpost.Code] != null)
                            {
                                var oldslug2 = db.Slugs.FirstOrDefault(x => x.Post1.Id == post2.Id);
                                if (oldslug2 != null)
                                {
                                    //Remove old slug
                                    db.Slugs.Remove(oldslug2);
                                    db.SaveChanges();
                                }
                                //Add new slug
                                Slug slug2 = new Slug();
                                slug2.Post = post2.Id;
                                slug2.Slug1 = fm["Slug-" + langpost.Code];
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
                        }
                        db.SaveChanges();
                    }

                }
                #endregion


                #region Inventory insert by luan
                //nếu kho được kick hoạt
                if (post.Product != null)
                {
                    //if (post.Product.Inv_Active)
                    //{
                    var inven = db.Inventories.FirstOrDefault(x => x.Agent_Id == 1 && x.Product_Id == post.Product_Id);
                    if (inven != null)
                        inven.Quantity = post.Quantity;
                    db.SaveChanges();
                    // }
                    ViewBag.manu = new SelectList(db.Manufacturers, "Id", "Name", post.Product.Supplier_Id);
                }
                #endregion

                if (buttonPressed == ButtonType.SubmitandMedia)
                {
                    return RedirectToAction("MediaFiles", new { id = post.Id });
                }

                if (buttonPressed == ButtonType.SubmitandEdit)
                {
                    return RedirectToAction("Edit", new { status = "edit-done", id = post.Id });
                }
                if (db.Languages.Where(x => x.Active).Count() > 1)
                {
                    return RedirectToAction("Index", new { status = "edit-done", type = post.Type, culture = post.Lang });
                }
                else
                {
                    return RedirectToAction("Index", new { status = "edit-done", type = post.Type });
                }
            }
            return View(post);
        }

        #endregion

        #region Copy
        // COPY
        public ActionResult Copy(string lang, int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
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
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == post.PostType.Id && x.Role_Id == roleid).Create == false)
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

            var posttype = db.PostTypes.Find(post.Type);
            ViewBag.Name = posttype.Name;
            ViewBag.Description = posttype.Description;
            ViewBag.Type = posttype.Id;
            if (lang != null && db.Languages.Where(x => x.Code == lang) != null)
            {
                ViewBag.Lang = db.Languages.FirstOrDefault(x => x.Code == lang).Name;
            }
            return View(post);
        }

        [HttpPost]
        public ActionResult Copy(Post postnew, string lang, int id = 0)
        {
            Post post = db.Posts.Find(id);
            //copy product
            if (post.PostType.Active_Product == true)
            {
                postnew.Product = post.Product;
            }

            //postnew = post;
            // postnew.Title = post.Title;
            //postnew.Description = post.Description;
            //postnew.Content = post.Content;

            postnew.Price = post.Price;
            postnew.Price_Old = post.Price_Old;
            postnew.onsale = post.onsale;
            postnew.feature = post.feature;
            postnew.Active = post.Active;
            postnew.Lang = post.Lang;
            postnew.Level = 1;
            //postnew.Meta_key = post.Meta_key;
            //postnew.Meta_Description = post.Meta_Description;
            //postnew.Html_Head = post.Html_Head;
            postnew.PostType = post.PostType;
            postnew.Thumb = post.Thumb;
            postnew.Date_Create = DateTime.Now;
            postnew.User_Create = post.User_Create;

            if (lang != null && db.Languages.Where(x => x.Code == lang) != null)
            {
                postnew.Lang = lang;
            }
            db.Posts.Add(postnew);
            db.SaveChanges();

            Slug slug = new Slug();
            slug.Post = postnew.Id;
            slug.Slug1 = post.Slugs.FirstOrDefault().Slug1;

            slug.Slug_Count = db.Slugs.Where(x => x.Id != slug.Id && x.Slug1 == slug.Slug1).Max(m => m.Slug_Count) + 1;
            slug.Slug_Full = slug.Slug_Count + "-" + slug.Slug1;
            db.Slugs.Add(slug);
            db.SaveChanges();


            return RedirectToAction("Index", new { status = "copy-done", type = postnew.Type, culture = postnew.Lang });
        }

        #endregion

        // DELETE Post
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Post post = db.Posts.Find(id);
                //Slug slug = db.Slugs.FirstOrDefault(x => x.Post == id);
                //db.Slugs.Remove(slug);

                var attributes = db.Post_PostAttributes.Where(x => x.Id_Post == post.Id).ToList();
                if (attributes.FirstOrDefault() != null)
                {
                    foreach (var item in attributes)
                    {
                        db.Post_PostAttributes.Remove(item);
                    }
                }

                var menulist = db.MenuItems.Where(x => x.Post == post.Id).ToList();
                if (menulist.FirstOrDefault() != null)
                {
                    foreach (var item in menulist)
                    {
                        item.Type = 1;
                        item.Url = "#";
                        item.Post = null;
                    }
                }

                #region Parallel Language
                var postslang = db.Posts.Where(x => x.MainLang_Id == id).ToList();
                if (postslang.FirstOrDefault() != null)
                {
                    foreach (var postlang in postslang)
                    {
                        var s = db.Slugs.FirstOrDefault(x => x.Post == postlang.Id);
                        if (s != null)
                            db.Slugs.Remove(s);

                        var menulanglist = db.MenuItems.Where(x => x.Post == postlang.Id).ToList();
                        if (menulanglist.FirstOrDefault() != null)
                        {
                            foreach (var item in menulanglist)
                            {
                                item.Type = 1;
                                item.Url = "#";
                                item.Post = null;
                            }
                        }
                        db.Posts.Remove(postlang);
                    }
                }
                #endregion

                var subpost = db.Posts.Where(x => x.Owner_Id == post.Id).ToList();
                if (subpost.FirstOrDefault() != null)
                {
                    foreach (var item in subpost)
                    {
                        item.Owner_Id = null;
                        item.Active = false;
                    }
                }
                //remove product
                if (post.Product_Id != null)
                {
                    var pro = db.Products.Find(post.Product_Id);
                    db.Products.Remove(pro);
                }
                db.Posts.Remove(post);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region MediaFile
        //
        // MediaFiles list
        public ActionResult MediaFiles(int id, string status)
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
                }
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
            #endregion
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            var posttype = db.PostTypes.Find(post.Type);
            ViewBag.Name = posttype.Name;
            ViewBag.Description = posttype.Description;
            ViewBag.Type = posttype.Id;
            ViewBag.PostName = post.Title;
            ViewBag.PostId = id;
            ViewBag.Status = status;
            var medias = db.MediaFiles.Where(x => x.Post_Id == id).OrderBy(o => o.Order).ToList();
            return View(medias);
        }

        [HttpPost]
        public ActionResult MediaFiles(int id, FormCollection fm)
        {
            var orderinputed = db.MediaFiles.Where(x => x.Post_Id == id).ToList();
            for (int i = 0; i < fm.Count; i++)
            {
                if (fm["Order-" + i] != null && fm["Order-" + i] != string.Empty)
                {
                    // Delete Old content
                    foreach (var items in orderinputed)
                    {
                        int orderid = int.Parse(fm["OrderId-" + i]);
                        var media = orderinputed.Where(x => x.Id == orderid).FirstOrDefault();
                        media.Order = int.Parse(fm["Order-" + i]);
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("MediaFiles", new { id = id, status = "order-done" });
        }

        public ActionResult MediaEdit(string status, int id = 0)
        {
            MediaFile media = db.MediaFiles.Find(id);
            if (media == null)
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
                    if (db.Roles_PostType.FirstOrDefault(x => x.PostType_Id == media.Post.PostType.Id && x.Role_Id == roleid).Edit == false)
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

            return View(media);
        }

        [HttpPost]
        public ActionResult MediaEdit(MediaFile media)
        {
            if (ModelState.IsValid)
            {
                db.Entry(media).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MediaFiles", new { id = media.Post_Id, status = "edit-done" });
            }
            return View(media);
        }

        //
        // MediaFiles input
        public ActionResult MediaFiles_Upload(int id)
        {
            ViewBag.Id = id;
            return PartialView("_MediaFiles_Upload");
        }

        [HttpPost]
        public ActionResult MediaFiles_Upload(string URL, string Description, string Id, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var media = new MediaFile();
                media.URL = URL;
                media.Description = Description;
                media.Post_Id = Convert.ToInt32(Id);
                db.MediaFiles.Add(media);
                db.SaveChanges();
            }

            return RedirectToAction("Mediafiles", "Content", new { id = Id, status = "create-done" });
        }

        [HttpPost]
        public ActionResult MediaDelete(int id)
        {
            try
            {
                MediaFile media = db.MediaFiles.Find(id);
                db.MediaFiles.Remove(media);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Attribute

        public ActionResult AttributeBox(int type, int? postid)
        {
            ViewBag.PostId = postid;
            var attributes = db.PostAttributes.Include(p => p.PostTypes).Where(p => p.PostTypes.Where(x => x.Id == type).FirstOrDefault() != null).ToList();
            return PartialView("_AttributeBox", attributes);
        }

        #endregion

        /// <summary>
        /// Updates the price.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="price">The price.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/29/2014 12:03 PM </created>
        public ActionResult UpdatePrice(int id, decimal? price, int? order)
        {
            var post = db.Posts.Find(id);
            if (price != null)
                post.Price = price;
            if (order != null)
                post.Order = order;
            db.SaveChanges();
            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region Load category
        public void Drrdown(Int32 catid, string bullet)
        {

            var listchild = db.Cats.Where(x => x.Owner_Id == catid).OrderByDescending(x => x.Order).ToList();
            if (listchild.Count > 0)
            {
                foreach (var item in listchild)
                {
                    string bullet1 = "---";

                    items.Add(new MultiSelectList(items, item.Id.ToString(), bullet + item.Name));
                    Drrdown(item.Id, bullet + bullet1);
                }

            }
        }

        #endregion
    }
}