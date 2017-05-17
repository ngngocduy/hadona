using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using PixelCMS.Helpers;
// -----------------------------------------
// PIXEL CMS
// WidgetController.cs v1.2
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Controllers
{
    public class WidgetController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Widget/
        #region SocialLink and FlagList
        public ActionResult SocialLink()
        {
            return PartialView("_SocialLink");
        }

        public ActionResult FlagList()
        {
            var langlist = db.Languages.Where(x => x.Active == true).OrderBy(o => o.Order);
            return PartialView("_FlagList", langlist.ToList());
        }
        #endregion

        #region Email Subscribe
        public ActionResult SubscribeBox()
        {
            return PartialView("_SubscribeBox");
        }

        [HttpPost]
        public ActionResult Subscribe(string email)
        {
            //if (ModelState.IsValid)
            //{
            var emailsubscribe = new EmailSubscribe();
            emailsubscribe.Email = email;
            emailsubscribe.Date = DateTime.Now;
            db.EmailSubscribes.Add(emailsubscribe);
            db.SaveChanges();
            return Content("Thanks", "text/html");
        }
        #endregion

        #region Related post
        public ActionResult RelatedPosts(int currentpost)
        {
            var post = db.Posts.Find(currentpost);
            if(post.Cats.FirstOrDefault()!=null)
            {
                int catid = post.Cats.FirstOrDefault().Id;
                var postincat = db.Posts.Where(x => (x.Cats.FirstOrDefault(z => z.Id == catid) != null) & x.Id != post.Id && x.Lang == post.Lang & x.PostType.Code != "trang").OrderByDescending(o => o.Date_Create).Take(5).ToList();
              
                return PartialView("_RelatedPosts", postincat);
            }
            var postintype = db.Posts.Include(x => x.Cats).Where(x => x.PostType.Id == post.PostType.Id && x.Id != post.Id && x.Lang == post.Lang & x.PostType.Code != "trang").OrderByDescending(o => o.Date_Create).Take(5).ToList();
           return PartialView("_RelatedPosts", postintype);
        }
        #endregion

        #region FeatureCat
        public ActionResult FeatureProductCat()
        {
            var cats = db.Cats.Where(x => x.Active == true && x.Feature == true && x.PostType.Code == "product").OrderBy(o => o.Order).ToList();
            return PartialView("_FeatureProductCat",cats);
        }
        #endregion

        #region FeatureCat
        public ActionResult FeatureProduct()
        {
            var posts = db.Posts.Where(x => x.Active == true && x.feature == true && x.PostType.Code == "product").Take(8).ToList();
            return PartialView("_FeatureProduct", posts);
        }
        #endregion

        #region RelatedProduct
        public ActionResult RelatedProduct(int id)
        {
           /* var cat = db.Posts.Find(id).Cats.FirstOrDefault(x => x.Position == 0);
            if (cat != null)
            {
                var catid = cat.Id;
                var posts = db.Posts.Where(x =>x.Lang=="vi"& x.Active && x.PostType.Code == "product" && x.Id != id && x.Cats.FirstOrDefault(z => z.Id == catid) != null).OrderBy(r => Guid.NewGuid()).Take(5).ToList();
                return PartialView("_RelatedProduct", posts);
            }*/
            var post = db.Posts.Find(id);
            if (post.Cats.FirstOrDefault() != null)
            {
                int catid = post.Cats.FirstOrDefault().Id;
                var postincat = db.Posts.Where(x => (x.Cats.FirstOrDefault(z => z.Id == catid) != null) & x.Id != post.Id && x.Lang == post.Lang & x.PostType.Code == "product").OrderByDescending(o => o.Date_Create).Take(12).ToList();

                return PartialView("_RelatedProduct", postincat);
            }
            else
            {
                var posts = db.Posts.Where(x => x.Lang == "vi" & x.Active && x.PostType.Code == "product" && x.Id != id).OrderBy(r => Guid.NewGuid()).Take(5).ToList();
                return PartialView("_RelatedProduct", posts);
            }
        }
        #endregion

        #region Home Banner
        public ActionResult HomeBanner()
        {
            var banner = db.Posts.Where(x => x.Active == true && x.PostType.Code == "home-banner"&&x.Lang=="vi").OrderBy(o => o.Order).ToList();
            return PartialView("_HomeBanner", banner);
        }

        #endregion

        #region Home Banner
        public ActionResult Video()
        {
            var video = db.Posts.Where(x => x.Active == true && x.PostType.Code == "video").OrderBy(o => o.Order).ToList();
            return PartialView("_Video", video);
        }
        #endregion

        #region Home slide
        public ActionResult HomeSlide()
        {
            var banner = db.Posts.Where(x => x.Active == true && x.PostType.Code == "home-slide").OrderByDescending(o => o.Order).ThenByDescending(x=>x.Date_Create).ToList();
            return PartialView("_HomeSlide", banner);
        }

        public ActionResult SlideDoiTac()
        {
            var banner = db.Posts.Where(x => x.Active == true && x.PostType.Code == "slide-doi-tac").OrderByDescending(o => o.Order).ThenByDescending(x => x.Date_Create).ToList();
            return PartialView("_SlideDoiTac", banner);
        }
        #endregion

        public ActionResult ADLefRight()
        {
            var banner = db.Posts.Where(x => x.Active == true && x.PostType.Code == "banner-left-right").OrderBy(o => o.Order).ToList();
            return PartialView("_ADLefRight", banner);
        }

        #region Home Background
        public ActionResult IntroBG(string culture)
        {
            var banner = db.Posts.Where(x => x.Active == true && x.PostType.Code == "intro-background").OrderBy(o => o.Order).ToList();
            return PartialView("_IntroBg", banner);
        }
        #endregion

        #region news in home
        public ActionResult NewsInHome(string culture)
        {
            var news = db.Posts.Where(x => x.Active && x.PostType.Code == "tin-tuc").OrderBy(o => o.Order).ToList();
            return PartialView("_NewsInHome", news);
        }
        #endregion

        #region dịch vụ & dự án
        public ActionResult DichVu(string culture)
        {
            var news = db.Posts.Where(x => x.Active & x.PostType.Code == "gallery"&x.Lang=="vi").OrderBy(o => o.Order).ToList();
            return PartialView("_Gallery", news);
        }
        public ActionResult DuAn(string culture)
        {
            var news = db.Posts.Where(x => x.Active & x.PostType.Code == "project" & x.Lang == "vi").OrderBy(o => o.Order).ToList();
            return PartialView("_Project", news);
        }
        #endregion

        #region PageBackground
        public ActionResult PageBg()
        {
            // ViewBag.Banner = db.Posts.Where(x => x.Active == true && x.PostType.Code == "banner-content-background").OrderBy(r => Guid.NewGuid()).First().Thumb;
            return PartialView("_PageBg");
        }
        #endregion

        #region Lấy bài viết theo chuyên mục(SP bán chạy/SP trang chủ)
        public ActionResult GetPostInCat(int vitri)
        {
            var cat = db.Cats.Where(x => x.Position == vitri && x.Active).OrderByDescending(x => x.Order).ToList();
            if (vitri == 6)
                return PartialView("_SPBanChay", cat);
            if (vitri == 5)
                return PartialView("_NewsInfooter", cat);
            if (vitri == 4)
                return PartialView("_NewsInHome", cat);//vi tri sidebar
            if (vitri == 3)//vị trí 3 trang chủ
                return PartialView("_HomeContent1", cat);
            if (vitri == 2)//vị trí 2 tin tuc  trang chủ
                return PartialView("_NewsInHome1", cat);

            return PartialView("_HomeContent", cat);
        }
        #endregion

        #region Lấy bài viết theo thuộc tính (Banner left)
        //get att dropdow
        public ActionResult BannerLeft(int idATT)
        {
            string stridATT = idATT.ToString();
            var banner = db.Posts.Where(x => x.Active && x.Post_PostAttribute.FirstOrDefault(z => z.Value == stridATT) != null).OrderBy(o => o.Order).ToList();
            if (idATT == 9)
                return PartialView("_BannerFooter", banner);

            return PartialView("_BannerLeft", banner);
        }
        #endregion

        #region  1 bài viết
        public ActionResult baiviet(string culture, string box)
        {
            if (box == "box1")
            {
                ViewBag.box1 = box;
                var loiich = db.Posts.Where(x => x.Active && x.Slugs.FirstOrDefault().Slug_Full == "box-1").FirstOrDefault();
                return PartialView("_1baiviet", loiich);
            }
            else if (box == "box2")
            {
                ViewBag.box2 = box;
                var loiich = db.Posts.Where(x => x.Active && x.Slugs.FirstOrDefault().Slug_Full == "box-2").FirstOrDefault();
                return PartialView("_1baiviet", loiich);
            }
            else if (box == "box3")
            {
                ViewBag.box3 = box;
                var loiich = db.Posts.Where(x => x.Active && x.Slugs.FirstOrDefault().Slug_Full == "box-3").FirstOrDefault();
                return PartialView("_1baiviet", loiich);
            }
            else if (box == "box4")
            {
                ViewBag.box4 = box;
                var loiich = db.Posts.Where(x => x.Active && x.Slugs.FirstOrDefault().Slug_Full == "box-4").FirstOrDefault();
                return PartialView("_1baiviet", loiich);
            }
            else if (box == "dichvu")
            {
                ViewBag.dichvu = box;
                var loiich = db.Posts.Where(x => x.Active && x.Slugs.FirstOrDefault().Slug_Full == "dich-vu").FirstOrDefault();
                return PartialView("_1baiviet", loiich);
            }
            var model = db.Posts.Where(x => x.Active && x.Slugs.FirstOrDefault().Slug_Full == box).FirstOrDefault();
            return PartialView("_1baiviet",model);
        }
        #endregion

        #region nhiều bài viết
        public ActionResult nbaiviet(string culture)
        {
            var banner = db.Posts.Where(x => x.Active && x.PostType.Code == "6boxinhome" && x.Lang == "vi").OrderBy(o => o.Order).ToList();
            return PartialView("_nbaiviet", banner);
        }
        #endregion

        #region NewsSlide
        public ActionResult NewsSlide()
        {
            var sp = db.Posts.Where(x => x.Active == true && x.PostType.Code == "tin-tuc").OrderBy(o => o.Order).ToList();
            return PartialView("_NewsSlide", sp);
        }
        #endregion

        #region Mobile
        public ActionResult HomeSlideMobile()
        {
            var banner = db.Posts.Where(x => x.Active && x.PostType.Code == "home-slide-mobile").OrderByDescending(o => o.Order).ThenByDescending(z=>z.Date_Create).ToList();
            return PartialView("_HomeSlideMobile", banner);
        }

        public ActionResult GetPostInCatMobile(int vitri)
        {
            var cat = db.Cats.Where(x => x.Position == vitri && x.Active).OrderByDescending(x => x.Order).ToList();
            if (vitri == 2)
                return PartialView("_SPBanChay", cat);

            return PartialView("_HomeContentMobile", cat);
        }

        public ActionResult RelatedPostsMobile(int currentpost)
        {
            var post = db.Posts.Find(currentpost);
            var relatedposts = db.Posts.Include(x => x.Cats).Where(x => x.PostType.Id == post.PostType.Id && x.Id != post.Id && x.Lang == post.Lang).OrderBy(o => o.Date_Create).Take(10).ToList();
            return PartialView("_RelatedPostsMobile", relatedposts);
        }
        #endregion
    }
}
