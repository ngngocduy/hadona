using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using PixelCMS.Core.Models;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / CommonController.cs v1.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class CommonController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();
        //
        // GET: /Admin/Common/

        public ActionResult MainMenu(string culture)
        {
            var menulist = db.PostTypes.Where(x => x.Active == true).ToList();
            ViewBag.Culture = culture;
            return PartialView("_MainMenu", menulist);
        }

        public ActionResult Quickcreate()
        {
            var language = db.Languages.Where(x => x.Active == true).ToList();
            return PartialView("_Quickcreate", language);
        }

        public ActionResult Quicksetting()
        {
            return PartialView("_Quicksetting");
        }

        public ActionResult Emailbox()
        {
            /* #region User Permission
             var roles = Roles.GetRolesForUser().ToList();
             if(roles.FirstOrDefault() != null){
                 foreach (var item in roles)
                 {
                     var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                     var roleaccess = db.Roles_Access.Find(roleid);
                     if (roleaccess.Email == false)
                     {
                         ViewBag.Disable = "false";
                     }
                 }
             }
             else
             {
                 return RedirectToAction("Index", "Dashboard");
             }
             #endregion*/
            var emails = db.Emails.Where(r => r.Read == false).Count();
            ViewBag.emailcount = emails;
            return PartialView("_EmailBox");
        }
        #region Option
        //
        // GET: /Common/
        public string LoadOption(string code)
        {
            try
            {
                Option conf = db.Options.FirstOrDefault(c => c.Code == code);
                conf.Value = System.Web.HttpUtility.HtmlDecode(conf.Value);
                return conf.Value;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
        public ActionResult Index()
        {
            /* if (TempData["filename"]==null)
             {
                 TempData["filename"] = "noimage.png";
             }*/
            return PartialView("_UploadImage");
        }

        [HttpPost]
        public ActionResult Index(List<HttpPostedFileBase> files)
        {
            TempData["filename"] = "";
            string[] filemane = new string[Request.Files.Count];
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string path = Server.MapPath("~/Upload/images");
                if (!string.IsNullOrEmpty(file.FileName))
                {
                    file.SaveAs(Path.Combine(path, file.FileName));
                }
                filemane[i] = file.FileName;
                //string.Join(file.FileName, filemane);
            }
            TempData["filename"] = filemane;

            return RedirectToAction("Files", "File");
        }
    }
}
