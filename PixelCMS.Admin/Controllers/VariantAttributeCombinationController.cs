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
    public class VariantAttributeCombinationController : Controller
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
            var VariantAttributeCombinations = db.VariantAttributeCombinations.Where(x => x.PostId == postId).ToList();
            ViewBag.postId = postId;//lấy postId để thêm 
            return View(VariantAttributeCombinations.ToPagedList(pageNumber, pageSize));
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
            var model = new VariantAttributeCombination();
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
        public ActionResult Create(VariantAttributeCombination VariantAttributeCombination)
        {
            if (ModelState.IsValid)
            {
                // VariantAttributeCombination.CreateDate = DateTime.Now;
                db.VariantAttributeCombinations.Add(VariantAttributeCombination);
                db.SaveChanges();
                return RedirectToAction("Index", new { postId = VariantAttributeCombination.PostId });
            }
            ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name");
            return View(VariantAttributeCombination);
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
            var VariantAttributeCombination = db.VariantAttributeCombinations.Find(id);
            if (VariantAttributeCombination == null)
            {
                return HttpNotFound();
            }
           // ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name", VariantAttributeCombination.VariantGroup_Id);
            return View(VariantAttributeCombination);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult Edit(VariantAttributeCombination VariantAttributeCombination)
        {
            if (ModelState.IsValid)
            {
                // VariantAttributeCombination.CreateDate = DateTime.Now;
                db.Entry(VariantAttributeCombination).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { postId = VariantAttributeCombination.PostId });
            }
           // ViewBag.VariantGroup_Id = new SelectList(db.GroupVariants, "Id", "Name", VariantAttributeCombination.VariantGroup_Id);
            return View(VariantAttributeCombination);
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
                var dc = db.VariantAttributeCombinations.Find(id);
                db.VariantAttributeCombinations.Remove(dc);
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