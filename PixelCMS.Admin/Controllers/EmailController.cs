using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using PixelCMS.Core.Models;
using PagedList;
using System.Text;
using PixelCMS.Filters;
using System.Web.Security;
using WebMatrix.WebData;
// -----------------------------------------
// PIXEL CMS
// Admin Area / EmailController.cs v2.0
// Feb.2014
// Author: Zhouhai - tuanhai.chau@gmail.com
// Copyright (c) 2014
// -----------------------------------------
namespace PixelCMS.Areas.Admin.Controllers
{
    public class EmailController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        #region Email
        //
        // GET: /Admin/Email/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Email == false)
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
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SubjectSortParm = String.IsNullOrEmpty(sortOrder) ? "Subject_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var emails = from s in db.Emails select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                emails = emails.Where(s => s.Subject.ToUpper().Contains(searchString.ToUpper())
                                   || s.Subject.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Subject_desc":
                    emails = emails.OrderByDescending(s => s.Subject);
                    break;
                case "Date":
                    emails = emails.OrderBy(s => s.DateSend);
                    break;
                case "Date_desc":
                    emails = emails.OrderByDescending(s => s.DateSend);
                    break;
                default:
                    emails = emails.OrderByDescending(s => s.DateSend);
                    break;
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(emails.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Admin/Email/Details/5
        public ActionResult Details(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Email == false)
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
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            email.Read = true;
            db.SaveChanges();
            return View(email);
        }

        // DELETE Email
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Email email = db.Emails.Find(id);
                db.Emails.Remove(email);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Subscribe
        public ActionResult Subscribe(int? page)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Email == false)
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
            var list = db.EmailSubscribes.OrderBy(x => x.Date);
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateSubscribe()
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Email == false)
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

            var Groups = db.webpages_Roles.Select(x => new { Id = x.RoleId, Name = x.RoleName }).ToList();
            ViewBag.MultiselectGroup = new MultiSelectList(Groups, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubscribe(EmailSubscribe model, int[] Groups)
        {
            //check mail đã tồn tại
            var IsExist = db.EmailSubscribes.Any(x => x.Email == model.Email);
            if (IsExist)
            {
                ModelState.AddModelError("", "Email đã tồn tại.Vui lòng chọn Email khác");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                model.Date = DateTime.Now;
                db.EmailSubscribes.Add(model);

                // Group
                if (Groups != null)
                {
                    // int a = Groups.Count();
                    for (int i = 0; i < Groups.Length; i++)
                    {
                        var role = db.webpages_Roles.Find(Groups[i]);
                        model.webpages_Roles.Add(role);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Subscribe");
            }
            return View(model);
        }

        public ActionResult EditSubscribe(int id = 0)
        {
            #region User Permission
            var roles = Roles.GetRolesForUser().ToList();
            if (roles.FirstOrDefault() != null)
            {
                foreach (var item in roles)
                {
                    var roleid = db.webpages_Roles.FirstOrDefault(x => x.RoleName == item).RoleId;
                    var roleaccess = db.Roles_Access.Find(roleid);
                    if (roleaccess.Admin == false || roleaccess.Email == false)
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

            var email = db.EmailSubscribes.Find(id);
            ArrayList selectedCategories = new ArrayList();
            foreach (var group in email.webpages_Roles)
            {
                selectedCategories.Add(group.RoleId).ToString();
            }
            var Groups = db.webpages_Roles.Select(x => new { Id = x.RoleId, Name = x.RoleName }).ToList();
            ViewBag.MultiselectGroup = new MultiSelectList(Groups, "Id", "Name", selectedCategories);

            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        [HttpPost]
        public ActionResult EditSubscribe(EmailSubscribe model, int[] Groups)
        {
            //check mail đã tồn tại
            var IsExist = db.EmailSubscribes.Any(x => x.Id != model.Id && x.Email == model.Email);
            if (IsExist)
            {
                ModelState.AddModelError("", "Email đã tồn tại.Vui lòng chọn Email khác");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;

                // Delete old groups 
                var groupold = db.EmailSubscribes.Include(x => x.webpages_Roles).FirstOrDefault(p => p.Id == model.Id);
                groupold.webpages_Roles.Clear();

                //Add new select groups
                if (Groups != null)
                {
                    for (int i = 0; i < Groups.Length; i++)
                    {
                        var role = db.webpages_Roles.Find(Groups[i]);
                        model.webpages_Roles.Add(role);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Subscribe");
            }
            return View(model);
        }

        // DELETE Email
        [HttpPost]
        public ActionResult DeleteSubscribe(int id)
        {
            try
            {
                EmailSubscribe email = db.EmailSubscribes.Find(id);
                db.EmailSubscribes.Remove(email);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        /// <summary>
        /// Groups the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/15/2014 3:01 PM </created>
        public ActionResult Group(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var list = db.webpages_Roles.ToList().ToPagedList(pageNumber, pageSize);
            return PartialView("_Group", list);
        }


        //add email into group 
        /// <summary>
        /// Adds the email to group.
        /// </summary>
        /// <param name="selectedIds">The selected ids.</param>
        /// <param name="selectedIdsGroup">The selected ids group.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 6/25/2014 4:54 PM </created>
        [HttpPost]
        public ActionResult addEmailToGroup(string selectedIds, string selectedIdsGroup)
        {
            var group = selectedIdsGroup.Split(',');
            var mail = selectedIds.Split(',');
            //var table = _NewsLetter_Group_Mapping.Table;
            for (int i = 0; i < group.Length; i++)
            {
                for (int j = 0; j < mail.Length; j++)
                {
                    int mailId = int.Parse(mail[j]);
                    int groupId = int.Parse(group[i]);

                    var Email = db.EmailSubscribes.Find(mailId);
                    var groupMail = db.webpages_Roles.Find(groupId); ;
                    var isExist = groupMail.EmailSubscribes.Any(x => x.Id == mailId);
                    if (!isExist)
                        Email.webpages_Roles.Add(groupMail);
                }
            }
            db.SaveChanges();

            return Json(new { succsess = true });
        }

        #endregion

        #region Export Subscribe
        public FileContentResult DownloadSubscribe()
        {
            var emails = db.EmailSubscribes.Where(x => x.Active == true).ToList();
            StringBuilder str = new StringBuilder();
            str.AppendLine("Email,Date");
            foreach (var item in emails)
            {
                str.AppendLine(item.Email + "," + string.Format("{0:dd/MM/yyy HH:mm:ss}", ((DateTime)item.Date)));
            }
            string csv = str.ToString();
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "EmailSubscribeReport" + string.Format("{0:dd/MM/yyy HH:mm:ss}", (DateTime.Now)) + ".csv");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string path = null;
            try
            {
                if (file.ContentLength>0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = AppDomain.CurrentDomain.BaseDirectory + "Upload\\" + fileName;
                    file.SaveAs(path);

                    var csv = new CsvReader(new StreamReader(path));
                    var ClientsList = csv.GetRecords<EmailSubscribeModel>().ToList();
                    foreach (var c in ClientsList)
                    {
                        EmailSubscribe mail = new EmailSubscribe();

                        mail.Email = c.Email;
                        mail.Date = c.Date;

                        //check
                        var IsExist = db.EmailSubscribes.Any(x => x.Email == c.Email);
                        if (!IsExist)
                        db.EmailSubscribes.Add(mail);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                TempData["error"] = "Upload failed hoặc tên file đã tồn tại";
            }

            return RedirectToAction("Subscribe");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
