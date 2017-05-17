using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using PixelCMS.Helpers;
using System.Collections;
using PagedList;
// -----------------------------------------
// PIXEL CMS
// ContentController.cs v4.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Controllers
{
    public class ContentController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Content/

        public ActionResult Slug(string slug1, string slug2, string slug3, string slug4, string culture, int? page)
        {
            #region BREADCRUMB
            if (slug4 != null)
            {
                ViewBag.BreadCrumb = slug4;
            }
            else if (slug3 != null)
            {
                ViewBag.BreadCrumb = slug3;
            }
            else if (slug2 != null)
            {
                ViewBag.BreadCrumb = slug2;
            }
            else if (slug1 != null)
            {
                ViewBag.BreadCrumb = slug1;
            }
            #endregion

            #region LANGUAGE (CONSTRUCTION)
            var currentlang = db.Languages.FirstOrDefault(x => x.Code == culture);
            if (currentlang.Construction == true)
            {
                return RedirectToAction("UnderConstruction", "Errors");
            }
            else if (currentlang.Active == false)
            {
                return RedirectToRoute("Errors", new { culture = culture });
            }
            #endregion

            #region CONTENT (Posttype, Cat, Post)
            var sluglist1 = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug1 && x.Lang == culture);
            var langposttype = true;
            if (sluglist1 == null)
            {
                sluglist1 = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug1);
                langposttype = false;
            }

            if (sluglist1 != null)
            {
                if (slug2 == null)
                {
                    #region Slug 1
                    if (sluglist1.PostType != null)
                    {
                        #region /news/
                        var posttype = db.PostTypes.Find(sluglist1.PostType);
                        if (posttype.AsPost == false || posttype.Active == false  || langposttype == false)
                        {
                            return RedirectToRoute("Errors", new { culture = culture });
                        }


                        var postculture = "";
                        if (posttype.Has_Parallel_Language == true)
                        {
                            postculture = db.Languages.OrderBy(o => o.Order).FirstOrDefault().Code;
                        }
                        else
                        {
                            postculture = culture;
                        }
                        var posts = posttype.Posts.Where(x => x.Active == true && x.Lang == postculture);
                        if (posttype.Has_Order == true)
                        {
                            posts = posts.OrderByDescending(x => x.Order).ToList();
                        }
                        else
                        {
                            posts = posts.OrderByDescending(o => o.Date_Create).ToList();
                        }
                        int pageSize = 6;
                        int pageNumber = (page ?? 1);
                        ViewBag.TypeName = posttype.PostType_Name.FirstOrDefault(x => x.Lang == culture).Name;

                        ViewBag.PosttypeId = posttype.Id;
                        posts = posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create);
                        if (posttype.Code == "product")
                        {
                           pageSize = 6;
                           return View("ProductList", posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create).ToPagedList(pageNumber, pageSize));
                           // var cat1 = db.Cats.Where(x => x.Position == 1 && x.Active).OrderByDescending(x => x.Order).ToList();
                           // return View("ProductList1", cat1);
                        }
                        else if (posttype.Code == "project")
                        {
                            return View("ProjectList", posts.ToPagedList(pageNumber, pageSize));
                        }
                        else if (posttype.Code == "slide-doi-tac")
                        {
                            pageSize = 1000;
                            return View("gallery", posts.ToPagedList(pageNumber, pageSize));
                        }
                        else if (posttype.Code == "customer")
                        {
                            return View("CustomerList", posts.ToPagedList(pageNumber, pageSize));
                        }
                        else if (posttype.Code == "photo")
                        {
                            pageSize = 24;
                           // var cat = db.Cats.Where(x => x.PostType.Code=="photo" && x.Active).OrderByDescending(x => x.Order).ToList();
                            return View("PhotoList", posts.ToPagedList(pageNumber, pageSize));
                        }
                        return View("PostList", posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create).ToPagedList(pageNumber, pageSize));
                        #endregion
                    }
                    else if (sluglist1.Cat != null)
                    {
                        #region /cat/
                        var cat = db.Cats.Find(sluglist1.Cat);
                        string catid = cat.Id.ToString();
                        cat.RealId = cat.Id;
                        if (cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
                        {
                            var cattemp = cat;
                            cat = db.Cats.Find(cattemp.MainLang_Id);
                            cat.Name = cattemp.Name;
                            cat.Lang = cattemp.Lang;
                            cat.RealId = cattemp.Id;
                            catid = cat.Id.ToString();
                        }

                        if (cat == null || cat.Lang != culture || cat.PostType.AsPost == false || cat.Active == false || cat.PostType.Active == false )
                        {
                            return RedirectToRoute("Errors", new { culture = culture });
                        }

                        var posts = db.Posts.Where(x => x.Cats.FirstOrDefault((c => c.Id == cat.Id | c.Parent_Path.Contains(catid))) != null && x.Active == true).ToList();
                        if (cat.PostType.Has_Order == true)
                        {
                            posts = posts.OrderBy(x => x.Order).ToList();
                        }

                        int pageSize = 6;
                        int pageNumber = (page ?? 1);
                        ViewBag.TypeName = cat.Name;
                        ViewBag.CatKey = cat.Meta_Key;
                        ViewBag.CatDescription = cat.Meta_Description;
                        ViewBag.CatHead = cat.Html_Head;
                        ViewBag.CatTitle = cat.Meta_Title;
                        ViewBag.PosttypeId = cat.PostType.Id;

                        if (cat.PostType.Code == "product")
                        {
                            return View("ProductList", posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create).ToPagedList(pageNumber, pageSize));
                           // var cat1 = db.Cats.Where(x => x.Position == 1 && x.Active).OrderByDescending(x => x.Order).ToList();
                            //return View("ProductList", cat1);
                        }
                        if (cat.PostType.Code == "project")
                        {
                            pageSize = 500;
                            return View("ProjectList", posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create).ToPagedList(pageNumber, pageSize));
                        }
                        return View("PostList", posts.OrderByDescending(x => x.Order).ThenByDescending(x => x.Date_Create).ToPagedList(pageNumber, pageSize));
                        #endregion
                    }
                    else if (sluglist1.Post != null)
                    {
                        #region /post/
                        var post = db.Posts.Find(sluglist1.Post);
                        ViewBag.TypeName = post.PostType.PostType_Name.FirstOrDefault(x => x.Lang == culture).Name;
                        if (post.PostType.Has_Parallel_Language && post.MainLang_Id != null)
                        {
                            var posttemp = post;
                            post = db.Posts.Find(posttemp.MainLang_Id);
                            post.Title = posttemp.Title;
                            post.Description = posttemp.Description;
                            post.Content = posttemp.Content;
                            post.Lang = posttemp.Lang;
                        }

                        if (post == null || post.Lang != culture || post.PostType.AsPost == false || post.Active == false || post.PostType.Active == false )
                        {
                            return RedirectToAction("NotFound", "Errors");
                        }

                        if (post.PostType.Code == "mainpage")
                        {
                            return View("PageDetail", post);
                        }
                        if (post.PostType.Code == "product")
                        {
                            return View("ProductDetail", post);
                        }
                        if (post.PostType.Code == "photo")
                        {
                            return View("PhotoDetail", post);
                        }
                        //if (post.PostType.Code == "project")
                        //{
                        //    return View("ProjectDetail", post);
                        //}
                        //check bai viet lien quan
                        ViewBag.related = db.Posts.Include(x => x.Cats).Where(x => x.PostType.Id == post.PostType.Id && x.Id != post.Id && x.Lang == post.Lang).OrderBy(o => o.Date_Create).ToList().Count;

                        return View("PostDetail", post);
                        #endregion
                    }
                    else
                    {
                        return RedirectToRoute("Errors", new { culture = culture });
                    }
                    #endregion
                }
                #region slug 2 3 4 tam thoi khong tac dung
                else
                {
                    var sluglist2 = db.Slugs.FirstOrDefault(x => x.Slug_Full == slug2);
                    if (sluglist2 != null)
                    {
                        if (slug3 == null)
                        {
                            #region Slug 2
                            if (sluglist2.Post != null)
                            {
                                #region /news/post
                                var posttype = db.PostTypes.Find(sluglist1.PostType);
                                if (posttype == null || posttype.AsPost == false || posttype.ShowType == false || posttype.Active == false || langposttype == false)
                                {
                                    return RedirectToRoute("Errors", new { culture = culture });
                                }

                                var post = db.Posts.Find(sluglist2.Post);
                                if (post.PostType.Has_Parallel_Language && post.MainLang_Id != null)
                                {
                                    var posttemp = post;
                                    post = db.Posts.Find(posttemp.MainLang_Id);
                                    post.Title = posttemp.Title;
                                    post.Description = posttemp.Description;
                                    post.Content = posttemp.Content;
                                    post.Lang = posttemp.Lang;
                                }

                                if (post == null || post.Lang != culture || post.Active == false || post.PostType.Id != posttype.Id)
                                {
                                    return RedirectToRoute("Errors", new { culture = culture });
                                }

                                if (posttype.Code == "product")
                                {
                                    return View("ProductDetail", post);
                                }
                                //else if (posttype.Code == "project")
                                //{
                                //    return View("ProjectDetail", post);
                                //}
                                else if (posttype.Code == "customer")
                                {
                                    return View("CustomerDetail", post);
                                }
                                else if (posttype.Code == "photo" || posttype.Code == "hinh-anh")
                                {
                                    return View("PhotoDetail", post);
                                }
                                //check bai viet lien quan
                                ViewBag.related = db.Posts.Include(x => x.Cats).Where(x => x.PostType.Id == post.PostType.Id && x.Id != post.Id && x.Lang == post.Lang).OrderBy(o => o.Date_Create).ToList().Count;
                                return View("PostDetail", post);
                                #endregion
                            }
                            else if (sluglist2.Cat != null)
                            {
                                #region /news/cat
                                var posttype = db.PostTypes.Find(sluglist1.PostType);
                                if (posttype == null || posttype.AsPost == false || posttype.ShowType == false || posttype.Active == false || langposttype == false)
                                {
                                    return RedirectToRoute("Errors", new { culture = culture });
                                }

                                var cat = db.Cats.Find(sluglist2.Cat);
                                cat.RealId = cat.Id;
                                if (cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
                                {
                                    var cattemp = cat;
                                    cat = db.Cats.Find(cattemp.MainLang_Id);
                                    cat.Name = cattemp.Name;
                                    cat.Lang = cattemp.Lang;
                                    cat.RealId = cattemp.Id;
                                }

                                if (cat == null || cat.Active == false || cat.Lang != culture || cat.PostType.Id != posttype.Id)
                                {
                                    return RedirectToRoute("Errors", new { culture = culture });
                                }
                                var posts = db.Posts.Where(x => x.Cats.FirstOrDefault(c => c.Id == cat.Id || c.Owner_Id == cat.Id) != null && x.Active == true).OrderByDescending(x => x.Date_Create).ToList();
                                if (posttype.Has_Order == true)
                                {
                                    posts = posts.OrderBy(x => x.Order).ToList();
                                }
                                int pageSize = 20;
                                int pageNumber = (page ?? 1);
                                ViewBag.TypeName = cat.Name;
                                ViewBag.CatKey = cat.Meta_Key;
                                ViewBag.CatDescription = cat.Meta_Description;
                                ViewBag.CatHead = cat.Html_Head;
                                ViewBag.CatTitle = cat.Meta_Title;
                                ViewBag.PosttypeId = cat.PostType.Id;
                                ViewBag.Des = cat.Description;
                                if (posttype.Code == "product")
                                {
                                    ViewBag.Banner = cat.Banner_Image;
                                    return View("ProductList", posts.OrderBy(x => x.Order).ToPagedList(pageNumber, pageSize));
                                }
                                else if (posttype.Code == "project")
                                {
                                    return View("ProjectList", posts.ToPagedList(pageNumber, pageSize));
                                }
                                else if (posttype.Code == "customer")
                                {
                                    return View("CustomerList", posts.ToPagedList(pageNumber, pageSize));
                                }
                                else if (posttype.Code == "photo" || posttype.Code == "hinh-anh")
                                {
                                    return View("PhotoList", posts.ToPagedList(pageNumber, pageSize));
                                }
                                return View("PostList", posts.ToPagedList(pageNumber, pageSize));
                                #endregion
                            }
                            else
                            {
                                return RedirectToRoute("Errors", new { culture = culture });
                            }
                            #endregion
                        }
                        else
                        {
                            var sluglist3 = db.Slugs.Where(x => x.Slug_Full == slug3).FirstOrDefault();
                            if (sluglist3 != null)
                            {
                                if (slug4 == null)
                                {
                                    #region Slug 3 (/news/cat1/cat/)
                                    if (sluglist3.Cat != null)
                                    {
                                        var posttype = db.PostTypes.Find(sluglist1.PostType);
                                        if (posttype == null || posttype.AsPost == false || posttype.ShowType == false || posttype.Active == false || langposttype == false)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }
                                        var cat1 = db.Cats.Find(sluglist2.Cat);
                                        cat1.RealId = cat1.Id;
                                        if (cat1.PostType.Has_Parallel_Language && cat1.MainLang_Id != null)
                                        {
                                            cat1.Active = db.Cats.Find(cat1.MainLang_Id).Active;
                                        }
                                        if (cat1 == null || cat1.Lang != culture || cat1.Active == false || cat1.PostType.Id != posttype.Id)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }

                                        var cat = db.Cats.Find(sluglist3.Cat);
                                        cat.RealId = cat.Id;
                                        if (cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
                                        {
                                            var cattemp = cat;
                                            cat = db.Cats.Find(cattemp.MainLang_Id);
                                            cat.Name = cattemp.Name;
                                            cat.Lang = cattemp.Lang;
                                            cat.RealId = cattemp.Id;
                                            if (cat.Owner_Id != cat1.MainLang_Id)
                                            {
                                                return RedirectToRoute("Errors", new { culture = culture });
                                            }
                                        }

                                        if (cat == null || cat.Active == false)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }
                                        if (cat.PostType.Has_Parallel_Language == false)
                                        {
                                            if (cat.Owner_Id != cat1.Id)
                                            {
                                                return RedirectToRoute("Errors", new { culture = culture });
                                            }
                                        }

                                        var posts = db.Posts.Where(x => x.Cats.FirstOrDefault(c => c.Id == cat.Id || c.Owner_Id == cat.Id) != null && x.Active == true).OrderByDescending(x => x.Date_Create).ToList();
                                        if (posttype.Has_Order == true)
                                        {
                                            posts = posts.OrderBy(x => x.Order).ToList();
                                        }

                                        int pageSize = 20;
                                        int pageNumber = (page ?? 1);
                                        ViewBag.TypeName = cat.Name;
                                        ViewBag.CatKey = cat.Meta_Key;
                                        ViewBag.CatDescription = cat.Meta_Description;
                                        ViewBag.CatHead = cat.Html_Head;
                                        ViewBag.CatTitle = cat.Meta_Title;
                                        ViewBag.PosttypeId = cat.PostType.Id;
                                        ViewBag.Des = cat.Description;

                                        if (posttype.Code == "product")
                                        {
                                            ViewBag.Banner = cat.Banner_Image;
                                            return View("ProductList", posts.OrderBy(x => x.Order).ToPagedList(pageNumber, pageSize));
                                        }
                                        else if (posttype.Code == "project")
                                        {
                                            return View("ProjectList", posts.ToPagedList(pageNumber, pageSize));
                                        }
                                        else if (posttype.Code == "customer")
                                        {
                                            return View("CustomerList", posts.ToPagedList(pageNumber, pageSize));
                                        }
                                        else if (posttype.Code == "photo" || posttype.Code == "hinh-anh")
                                        {
                                            return View("PhotoList", posts.ToPagedList(pageNumber, pageSize));
                                        }

                                        return View("PostList", posts.ToPagedList(pageNumber, pageSize));
                                    }
                                    else
                                    {
                                        return RedirectToRoute("Errors", new { culture = culture });
                                    }
                                    #endregion
                                }
                                else
                                {
                                    var sluglist4 = db.Slugs.Where(x => x.Slug_Full == slug4).FirstOrDefault();
                                    #region Slug 4 (/news/cat1/cat2/cat/)
                                    if (sluglist4.Cat != null)
                                    {
                                        var posttype = db.PostTypes.Find(sluglist1.PostType);
                                        if (posttype == null || posttype.AsPost == false || posttype.ShowType == false || posttype.Active == false || langposttype == false)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }
                                        var cat1 = db.Cats.Find(sluglist2.Cat);
                                        cat1.RealId = cat1.Id;
                                        if (cat1.PostType.Has_Parallel_Language && cat1.MainLang_Id != null || cat1.PostType.Id != posttype.Id)
                                        {
                                            cat1.Active = db.Cats.Find(cat1.MainLang_Id).Active;
                                        }
                                        if (cat1 == null || cat1.Lang != culture || cat1.Active == false)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }
                                        var cat2 = db.Cats.Find(sluglist3.Cat);
                                        cat2.RealId = cat2.Id;
                                        if (cat2.PostType.Has_Parallel_Language && cat2.MainLang_Id != null)
                                        {
                                            var cattemp = db.Cats.Find(cat2.MainLang_Id);
                                            cat2.Active = cattemp.Active;
                                            cat2.Owner_Id = cattemp.Owner_Id;
                                            if (cat2.Owner_Id != cat1.MainLang_Id)
                                            {
                                                return RedirectToRoute("Errors", new { culture = culture });
                                            }
                                        }
                                        if (cat2 == null || cat2.Active == false)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }

                                        var cat = db.Cats.Find(sluglist4.Cat);
                                        cat.RealId = cat.Id;
                                        if (cat.PostType.Has_Parallel_Language && cat.MainLang_Id != null)
                                        {
                                            var cattemp = cat;
                                            cat = db.Cats.Find(cattemp.MainLang_Id);
                                            cat.Name = cattemp.Name;
                                            cat.Lang = cattemp.Lang;
                                            cat.RealId = cattemp.Id;
                                            if (cat.Owner_Id != cat2.MainLang_Id)
                                            {
                                                return RedirectToRoute("Errors", new { culture = culture });
                                            }
                                        }

                                        if (cat == null || cat.Active == false)
                                        {
                                            return RedirectToRoute("Errors", new { culture = culture });
                                        }
                                        if (cat.PostType.Has_Parallel_Language == false)
                                        {
                                            if (cat.Owner_Id != cat2.Id || cat2.Owner_Id != cat1.Id)
                                            {
                                                return RedirectToRoute("Errors", new { culture = culture });
                                            }
                                        }
                                        var posts = cat.Posts.Where(x => x.Active == true).OrderByDescending(x => x.Date_Create).ToList();
                                        if (posttype.Has_Order == true)
                                        {
                                            posts = posts.OrderBy(x => x.Order).ToList();
                                        }
                                        int pageSize = 20;
                                        int pageNumber = (page ?? 1);
                                        ViewBag.TypeName = cat.Name;
                                        ViewBag.CatKey = cat.Meta_Key;
                                        ViewBag.CatDescription = cat.Meta_Description;
                                        ViewBag.CatHead = cat.Html_Head;
                                        ViewBag.CatTitle = cat.Meta_Title;
                                        ViewBag.PosttypeId = cat.PostType.Id;
                                        ViewBag.Des = cat.Description;

                                        if (posttype.Code == "product")
                                        {
                                            ViewBag.Banner = cat.Banner_Image;
                                            return View("ProductList", posts.OrderBy(x => x.Order).ToPagedList(pageNumber, pageSize));
                                        }
                                        else if (posttype.Code == "project")
                                        {
                                            return View("ProjectList", posts.ToPagedList(pageNumber, pageSize));
                                        }
                                        else if (posttype.Code == "customer")
                                        {
                                            return View("CustomerList", posts.ToPagedList(pageNumber, pageSize));
                                        }
                                        else if (posttype.Code == "photo" || posttype.Code == "hinh-anh")
                                        {
                                            return View("PhotoList", posts.ToPagedList(pageNumber, pageSize));
                                        }

                                        return View("PostList", posts.ToPagedList(pageNumber, pageSize));
                                    }
                                    else
                                    {
                                        return RedirectToRoute("Errors", new { culture = culture });
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                return RedirectToRoute("Errors", new { culture = culture });
                            }
                        }
                    }
                    else
                    {
                        return RedirectToRoute("Errors", new { culture = culture });
                    }
                }
                #endregion
            }
            else
            {
                return RedirectToRoute("Errors", new { culture = culture });
            }
            #endregion

        }

        public ActionResult minidetail(int id)
        {
                var post = db.Posts.Find(id);
                if (post.PostType.Has_Parallel_Language && post.MainLang_Id != null)
                {
                    var posttemp = post;
                    post = db.Posts.Find(posttemp.MainLang_Id);
                    post.Title = posttemp.Title;
                    post.Description = posttemp.Description;
                    post.Content = posttemp.Content;
                    post.Lang = posttemp.Lang;
                }
                return View("PageDetail", post);
        }

        public ActionResult Contact(string result, string culture)
        {
            ViewBag.BreadCrumb = "contact";
            var currentlang = db.Languages.FirstOrDefault(x => x.Code == culture);
            if (currentlang.Construction == true)
            {
                return RedirectToAction("UnderConstruction", "Errors");
            }
            else if (currentlang.Active == false)
            {
                return RedirectToRoute("Errors", new { culture = culture });
            }

            if (result == "success")
            {
                ViewBag.Result = Resources.Resources.contact_form_success;
            }
            else if (result == "fail")
            {
                ViewBag.Result = Resources.Resources.contact_form_fail;
            }
            return View();
        }

        /// <summary>
        /// Wishes the lists.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/26/2014 2:25 PM </created>
        public ActionResult WishLists(string culture)
        {
            return View();
        }

        public ActionResult CompareList(string culture)
        {

            var model = new List<Post>();
            HttpCookie cookierq = Request.Cookies["comparelist"];
            if (cookierq != null)
            {
                var wl = cookierq["compareproductids"].Split(',');
                foreach (var item in wl.Skip(wl.Count() - 4))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int productid = int.Parse(item);
                        var post = db.Posts.Where(x => x.Id == productid);
                        model.AddRange(post);
                    }
                }
            }
            return View(model);
        }

        #region Brand(nhà cung cấp)
        public ActionResult BrandList(int id, string culture, int? page)
        {
            var brand = db.Manufacturers.FirstOrDefault(x => x.Id == id);
            if (brand != null)
                ViewBag.TypeName = brand.Name;
            var results = db.Posts.Where(x => x.Active && x.Product_Id != null && x.Product.Supplier_Id == id).ToList();
            int pageSize = 28;
            int pageNumber = (page ?? 1);
            return View(results.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        public ActionResult Search(string culture, int? page, string keywords)
        {
            ViewBag.BreadCrumb = "search";
            var currentlang = db.Languages.FirstOrDefault(x => x.Code == culture);
            if (currentlang.Construction == true)
            {
                return RedirectToAction("UnderConstruction", "Errors");
            }
            else if (currentlang.Active == false)
            {
                return RedirectToRoute("Errors", new { culture = culture });
            }
            ViewBag.Keywords = keywords;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            var results = db.Posts.Where(x => x.Lang == culture && (x.Title.ToUpper().Contains(keywords.ToUpper()) || x.Slugs.Where(s => s.Slug_Full.ToUpper().Replace("-", " ").Contains(keywords.ToUpper())).FirstOrDefault() != null || x.Description.ToUpper().Contains(keywords.ToUpper()) || x.Meta_key.ToUpper().Contains(keywords.ToUpper()) || x.Meta_Description.ToUpper().Contains(keywords.ToUpper()))).OrderByDescending(o => o.Date_Create);
            return View(results.Where(x => x.PostType.AsPost == true).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture
            return RedirectToAction("Index");
        }

        public ActionResult GetLinkCat(string slug)
        {
            var y = db.Cats.FirstOrDefault(x => x.Slugs.FirstOrDefault(a => a.Slug_Full == slug) != null);
            var z = db.Posts.FirstOrDefault(x => x.Slugs.FirstOrDefault(a => a.Slug_Full == slug) != null);




            /* if (y != null)
             {
                 // lấy loại chuyên mục
                 var type = db.PostTypes.FirstOrDefault(x => x.Id == y.TypeId);
                 if (type != null)
                 {
                     //nếu loại chuyên mục là sản phẩm load list sản phẩm
                     if (type.Slug == "san-pham")
                     {
                         return RedirectToAction("ListProducts", "Categories", new { id = y.Id, slug = y.Slug });
                     }
                     // else load list tin tức
                     if (type.Slug == "tin-tuc")
                     {
                         return RedirectToAction("ListPost", "Categories", new { id = y.Id, slug = y.Slug });
                     }
                     // else load 1 bài viết (details)
                     if (type.Slug == "bai-viet")
                     {
                         if (z != null)
                         {
                             return RedirectToAction("PostDetails", "Categories", new { id = z.Id, slug = z.Slug });
                         }
                     }
                 }
                 // else load list tin tức
                 return RedirectToAction("ListPost", "Categories", new { id = y.Id, slug = y.Slug });
             }*/

            return RedirectToAction("index", "Home");
        }

    }
}
