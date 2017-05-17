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

namespace PixelCMS.Areas.Admin.Controllers
{
    public class VariantOptionController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Index(int? page)
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

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var OptionVariants = db.OptionVariants.ToList();
            return View(OptionVariants.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Create()
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
            ViewBag.GroupVariant_Id = new SelectList(db.GroupVariants, "Id", "Name");
            return View();
        }

        /// <summary>
        /// Creates the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(OptionVariant OptionVariant)
        {

            if (ModelState.IsValid)
            {
                //OptionVariant.CreateDate = DateTime.Now;
                db.OptionVariants.Add(OptionVariant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupVariant_Id = new SelectList(db.GroupVariants, "Id", "Name", OptionVariant.GroupVariant_Id);
            return View(OptionVariant);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        public ActionResult Edit(int id = 0)
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
            var OptionVariant = db.OptionVariants.Find(id);
            if (OptionVariant == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupVariant_Id = new SelectList(db.GroupVariants, "Id", "Name", OptionVariant.GroupVariant_Id);
            return View(OptionVariant);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(OptionVariant OptionVariant)
        {
            if (ModelState.IsValid)
            {
               // OptionVariant.CreateDate = DateTime.Now;
                db.Entry(OptionVariant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupVariant_Id = new SelectList(db.GroupVariants, "Id", "Name", OptionVariant.GroupVariant_Id);
            return View(OptionVariant);
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
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
                    if (db.Roles_Access.Find(roleid).Admin == false)
                    {
                        return RedirectToAction("Error", "Dashboard");
                    }
                }
            }
            #endregion
            try
            {
                var dc = db.OptionVariants.Find(id);
                db.OptionVariants.Remove(dc);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region Group

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Group(int? page)
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

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var group = db.GroupVariants.OrderBy(x => x.Order).ToList();
            return View(group.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult CreateGroup()
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

            return View();
        }

        /// <summary>
        /// Creates the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateGroup(GroupVariant group)
        {
            if (ModelState.IsValid)
            {
                db.GroupVariants.Add(group);
                db.SaveChanges();
                return RedirectToAction("Group");
            }
            return View(group);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        public ActionResult EditGroup(int id = 0)
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

            var group = db.GroupVariants.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditGroup(GroupVariant group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Group");
            }
            return View(group);
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult DeleteGroup(int id)
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
            #endregion

            try
            {
                var dc = db.GroupVariants.Find(id);
                db.GroupVariants.Remove(dc);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        /*#region thuộc tính cho từng sản phẩm

        public ActionResult P_PAttList(int productid, int? page)
        {
            ViewBag.productid = productid;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var list = db.Products.Find(productid).Product_OptionVariant.ToList();
            return View(list.ToPagedList(pageNumber, pageSize));
        }


        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult CreateP_PAtt(int productid)
        {
            var model = new Product_OptionVariant();
            model.Product_Id = productid;
            return View(model);
        }

        /// <summary>
        /// Creates the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        [HttpPost]
        public ActionResult CreateP_PAtt(Product_OptionVariant P_PAtt)
        {
            var check = db.Product_OptionVariants.Any(
                    x => x.Product_Id == P_PAtt.Product_Id && x.Product_Id == P_PAtt.OptionVariant_Id);
            if (check)
            {
                ModelState.AddModelError("", "Thuộc tính này đã tồn tại không thêm mới được nữa!!");
                return View(P_PAtt);
            }

            if (ModelState.IsValid)
            {
                P_PAtt.CreateDate = DateTime.Now;
                db.Product_OptionVariants.Add(P_PAtt);
                db.SaveChanges();
                return RedirectToAction("P_PAttList", new { productid = P_PAtt.Product_Id });
            }
            return View(P_PAtt);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        public ActionResult EditP_PAtt(int productid, int proAttid)
        {
            var proatt = db.Product_OptionVariants.FirstOrDefault(x => x.OptionVariant_Id == proAttid && x.Product_Id == productid);
            if (proatt == null)
            {
                return HttpNotFound();
            }
            return View(proatt);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult EditP_PAtt(Product_OptionVariant P_PAtt)
        {
            if (ModelState.IsValid)
            {
                P_PAtt.CreateDate = DateTime.Now;
                db.Entry(P_PAtt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("P_PAttList", new { productid = P_PAtt.Product_Id });
            }
            return View(P_PAtt);
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult DeleteP_PAtt(int productid, int proAttid)
        {
            try
            {
                var proatt = db.Product_OptionVariants.FirstOrDefault(x => x.OptionVariant_Id == proAttid && x.Product_Id == productid);
                db.Product_OptionVariants.Remove(proatt);
                db.SaveChanges();
                return Content(Boolean.TrueString);
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }
        }

        [HttpGet]
        public JsonResult getAtt(string query = "")
        {
            var list = db.OptionVariants.Where(p => p.Name.Contains(query)).Select(x => new
            {
                Name = x.Name,
                Id = x.Id,
                Category = x.GroupVariant.Name
            }).Take(10).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion*/

    }
}