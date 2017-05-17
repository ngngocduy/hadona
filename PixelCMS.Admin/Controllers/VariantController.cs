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
    public class VariantController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Index(int VariantAtt_Id, int VariantGroup_Id,int PostId, int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var Variants = db.Variants.Where(x => x.VariantAtt_Id == VariantAtt_Id).ToList();
            ViewBag.VariantAtt_Id = VariantAtt_Id;//lấy VariantAtt_Id để thêm 
            ViewBag.VariantGroup_Id = VariantGroup_Id;
            ViewBag.PostId = PostId;
            return View(Variants.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Create(int VariantAtt_Id, int VariantGroup_Id)
        {
            var model = new Variant();
            model.VariantAtt_Id = VariantAtt_Id;

            var gVariant = db.GroupVariants.Find(VariantGroup_Id);
            ViewBag.type = gVariant.Type;

            ViewBag.VariantGroup_Id = VariantGroup_Id;
            ViewBag.VariantOption_Id = new SelectList(db.OptionVariants.Where(x => x.GroupVariant_Id == VariantGroup_Id), "Id", "Name");
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
        public ActionResult Create(Variant Variant, int VariantGroup_Id)
        {
            if (ModelState.IsValid)
            {
                // Variant.CreateDate = DateTime.Now;
                db.Variants.Add(Variant);
                db.SaveChanges();
                var VariantAtt = db.VariantAttributes.FirstOrDefault(x => x.Id == Variant.VariantAtt_Id);
                return RedirectToAction("Index", new { VariantAtt_Id = Variant.VariantAtt_Id, VariantGroup_Id = VariantGroup_Id, PostId = VariantAtt.PostId });
            }
            return View(Variant);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        public ActionResult Edit(int id, int VariantGroup_Id)
        {
            var Variant = db.Variants.Find(id);
            if (Variant == null)
            {
                return HttpNotFound();
            }
            ViewBag.VariantGroup_Id = VariantGroup_Id;
            ViewBag.VariantOption_Id = new SelectList(db.OptionVariants.Where(x => x.GroupVariant_Id == VariantGroup_Id), "Id", "Name",Variant.VariantOption_Id);
            return View(Variant);
        }

        /// <summary>
        /// Edits the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult Edit(Variant Variant, int VariantGroup_Id)
        {
            if (ModelState.IsValid)
            {
                // Variant.CreateDate = DateTime.Now;
                db.Entry(Variant).State = EntityState.Modified;
                db.SaveChanges();
                var VariantAtt = db.VariantAttributes.FirstOrDefault(x => x.Id == Variant.VariantAtt_Id);
                return RedirectToAction("Index", new { VariantAtt_Id = Variant.VariantAtt_Id, VariantGroup_Id = VariantGroup_Id, PostId = VariantAtt.PostId });
            }
            return View(Variant);
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
                var dc = db.Variants.Find(id);
                db.Variants.Remove(dc);
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