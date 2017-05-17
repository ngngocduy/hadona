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
    public class VariantAttributeController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Index(int postId, int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var VariantAttributes = db.VariantAttributes.Where(x => x.PostId == postId).ToList();
            ViewBag.postId = postId;//lấy postId để thêm 
            return View(VariantAttributes.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Create(int postId)
        {
            ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name");
            var model = new VariantAttribute();
            model.PostId = postId;
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
        public ActionResult Create(VariantAttribute VariantAttribute)
        {

            if (ModelState.IsValid)
            {
                // VariantAttribute.CreateDate = DateTime.Now;
                db.VariantAttributes.Add(VariantAttribute);
                db.SaveChanges();
                return RedirectToAction("Index", new { postId = VariantAttribute.PostId });
            }
            ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name");
            return View(VariantAttribute);
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
            var VariantAttribute = db.VariantAttributes.Find(id);
            if (VariantAttribute == null)
            {
                return HttpNotFound();
            }
            ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name", VariantAttribute.VariantGroup_Id);

            var TypeList = new List<SelectListItem>()
                               {
                                   new SelectListItem {Value = "0", Text = "Chọn"},
                                   new SelectListItem {Value = "1", Text = "Ô màu sắc"},
                                   new SelectListItem {Value = "2", Text = "Ô check(Cho chọn nhiều)"},
                                   new SelectListItem {Value = "3", Text = "Ô tròn radio(Cho chọn 1)"},
                                   new SelectListItem {Value = "5", Text = "Ô vuông radio(Cho chọn 1)"},
                                   new SelectListItem {Value = "4", Text = "Danh sách đổ xuống"},
                               };
            ViewBag.Type = new SelectList(TypeList, "Value", "Text", VariantAttribute.Type);
            return View(VariantAttribute);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult Edit(VariantAttribute VariantAttribute)
        {
            if (ModelState.IsValid)
            {
                // VariantAttribute.CreateDate = DateTime.Now;
                db.Entry(VariantAttribute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { postId = VariantAttribute.PostId });
            }
            var TypeList = new List<SelectListItem>()
                               {
                                   new SelectListItem {Value = "0", Text = "Chọn"},
                                   new SelectListItem {Value = "1", Text = "Ô màu sắc"},
                                   new SelectListItem {Value = "2", Text = "Ô check(Cho chọn nhiều)"},
                                   new SelectListItem {Value = "3", Text = "Ô radio(Cho chọn 1)"},
                                    new SelectListItem {Value = "5", Text = "Ô vuông radio(Cho chọn 1)"},
                                   new SelectListItem {Value = "4", Text = "Danh sách đổ xuống"},
                               };
            ViewBag.Type = new SelectList(TypeList, "Value", "Text", VariantAttribute.Type);
            ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name", VariantAttribute.VariantGroup_Id);
            return View(VariantAttribute);
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
                var dc = db.VariantAttributes.Find(id);
                db.VariantAttributes.Remove(dc);
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