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
    public class CommentController : Controller
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
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var orders = db.Comments.ToList();
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified Comment.
        /// </summary>
        /// <param name="Comment">The Comment.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:03 AM </created>
        [HttpPost]
        public ActionResult Create(Comment Comment)
        {
            if (string.IsNullOrEmpty(Comment.Name))
            {
                ModelState.AddModelError("", "Vui lòng điền tên Comment");
                return View(Comment);
            }

            if (ModelState.IsValid)
            {
                db.Comments.Add(Comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Comment);
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
            var Comments = db.Comments.Find(id);
            if (Comments == null)
            {
                return HttpNotFound();
            }
            return View(Comments);
        }

        /// <summary>
        /// Edits the specified Comment.
        /// </summary>
        /// <param name="Comment">The Comment.</param>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 8/20/2014 11:04 AM </created>
        [HttpPost]
        public ActionResult Edit(Comment Comment)
        {
            if (string.IsNullOrEmpty(Comment.Name))
            {
                ModelState.AddModelError("", "Vui lòng điền thông tin Comment");
                return View(Comment);
            }
            if (ModelState.IsValid)
            {
                Comment.View = true;
                db.Entry(Comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Comment);
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
                var Comments = db.Comments.Find(id);
                db.Comments.Remove(Comments);
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