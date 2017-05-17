using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using PixelCMS.Core.Models;
using PixelCMS.Mailer;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class EmailCampaignController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/18/2014 9:30 AM </created>
        public ActionResult Index(int? page)
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

            var List = db.EmailCampaigns.ToList();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(List.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/14/2014 11:07 AM </created>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/14/2014 11:07 AM </created>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EmailCampaign model)
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
            if (ModelState.IsValid)
            {
                model.CreateDate = DateTime.Now;
                db.EmailCampaigns.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/14/2014 5:10 PM </created>
        public ActionResult Edit(int id)
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
            var model = db.EmailCampaigns.Find(id);

            var Groups = db.webpages_Roles.Select(x => new { Id = x.RoleId, Name = x.RoleName }).ToList();
            ViewBag.MultiselectGroup = new MultiSelectList(Groups, "Id", "Name");
            return View(model);
        }

        /// <summary>
        /// Edits the specified save.
        /// </summary>
        /// <param name="save">The save.</param>
        /// <param name="sendmail">The sendmail.</param>
        /// <param name="model">The model.</param>
        /// <param name="email">The email.</param>
        /// <param name="Groups">The groups.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/18/2014 10:56 AM </created>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string save, string sendmail, string sendmailall, EmailCampaign model, string email, int[] Groups)
        {
            var GroupsModel = db.webpages_Roles.Select(x => new { Id = x.RoleId, Name = x.RoleName }).ToList();
            ViewBag.MultiselectGroup = new MultiSelectList(GroupsModel, "Id", "Name");
            if (ModelState.IsValid)
            {
                UserMailer mailer = new UserMailer();
                //validate củ chuối 
                if (model.SMTP_Id == null)
                {
                    ModelState.AddModelError("", "(*)Vui lòng chọn SMTP");
                    return View(model);
                }
                var SMTP = db.SMTPs.Find(model.SMTP_Id);
                //nếu click lưu
                if (save != null)
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //nếu click gửi mail cho nhóm or cho 1 mail
                if (sendmail != null)
                {
                    if (email != "")
                    {
                        mailer.SendMail(SMTP.User, "", SMTP.Password, int.Parse(SMTP.Port), SMTP.Server, email, model.Title, model.Content, true, SMTP.User);
                    }

                    // Group
                    if (Groups != null)
                    {
                        var Mail = new List<EmailSubscribe>();
                        for (int i = 0; i < Groups.Length; i++)
                        {
                            int groupid = Groups[i];
                            var ListMailInGroup = db.EmailSubscribes.Where(y => y.webpages_Roles.FirstOrDefault(x => x.RoleId == groupid) != null);
                            Mail.AddRange(ListMailInGroup);
                        }
                        foreach (var item in Mail.Distinct())//loại mail trùng
                        {
                            mailer.SendMail(SMTP.User, "", SMTP.Password, int.Parse(SMTP.Port), SMTP.Server, item.Email, model.Title, model.Content, true, SMTP.User);
                        }
                    }
                    TempData["mess"] = "Gửi thành công";
                }

                if (sendmailall != null)
                {
                    var listmail = db.EmailSubscribes;
                    foreach (var item in listmail)
                    {
                        mailer.SendMail(SMTP.User, "", SMTP.Password, int.Parse(SMTP.Port), SMTP.Server, item.Email, model.Title, model.Content, true, SMTP.User);
                    }
                    TempData["mess"] = "Gửi thành công";
                }
            }
            return View(model);
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/14/2014 5:11 PM </created>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var ojb = db.EmailCampaigns.Find(id);
                db.EmailCampaigns.Remove(ojb);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        /// <summary>
        /// Getsmtps the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/18/2014 10:39 AM </created>
        [HttpGet]
        public JsonResult getsmtp(string query = "")
        {
            var list = db.SMTPs.Where(p => p.User.Contains(query)).Select(x => new
            {
                Title = x.User,
                Id = x.Id,
            }).Take(10).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
