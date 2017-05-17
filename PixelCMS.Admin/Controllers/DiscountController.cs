using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class DiscountController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Index(int productid,int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var discounts = db.Discounts.Where(x=>x.Product_Id==productid).ToList();
            ViewBag.productid = productid;//lấy productid để thêm 
            return View(discounts.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Create(int productid)
        {
            ViewBag.Role_Id = new SelectList(db.webpages_Roles.Where(x => x.RoleName != "tkmadmin"), "RoleId", "RoleName");
            var model = new Discount();
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
        public ActionResult Create(Discount discount)
        {
           
            if (ModelState.IsValid)
            {
                discount.CreateDate = DateTime.Now;
                db.Discounts.Add(discount);
                db.SaveChanges();
                return RedirectToAction("Index",new {productid=discount.Product_Id});
            }
            ViewBag.Role_Id = new SelectList(db.webpages_Roles.Where(x => x.RoleName != "tkmadmin"), "RoleId", "RoleName", discount.Role_Id);
            return View(discount);
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
            var discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role_Id = new SelectList(db.webpages_Roles.Where(x=>x.RoleName!="tkmadmin"), "RoleId", "RoleName", discount.Role_Id);
            return View(discount);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult Edit(Discount discount)
        {
            if (ModelState.IsValid)
            {
                discount.CreateDate = DateTime.Now;
                db.Entry(discount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { productid = discount.Product_Id });
            }
            ViewBag.Role_Id = new SelectList(db.webpages_Roles.Where(x => x.RoleName != "tkmadmin"), "RoleId", "RoleName", discount.Role_Id);
            return View(discount);
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
            try
            {
                var dc = db.Discounts.Find(id);
                db.Discounts.Remove(dc);
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
    }
}