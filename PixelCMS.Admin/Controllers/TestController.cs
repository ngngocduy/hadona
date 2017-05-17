using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        //
        // GET: /Test/

        public ActionResult Index(string filename = "1.png")
        {
            var discounts = db.Discounts.Include(d => d.webpages_Roles);
            ViewBag.images = filename;
            return View(discounts.ToList());
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            string path = Server.MapPath("~/Upload/images");
            file.SaveAs(Path.Combine(path, file.FileName));
            return RedirectToAction("Index", new { filename = file.FileName });
        }

        //
        // GET: /Test/Details/5

        public ActionResult Details(int id = 0)
        {
            Discount discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return View(discount);
        }

        //
        // GET: /Test/Create

        public ActionResult Create()
        {
            ViewBag.Role_Id = new SelectList(db.webpages_Roles, "RoleId", "RoleName");
            return View();
        }

        //
        // POST: /Test/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Discount discount)
        {
            if (ModelState.IsValid)
            {
                db.Discounts.Add(discount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Role_Id = new SelectList(db.webpages_Roles, "RoleId", "RoleName", discount.Role_Id);
            return View(discount);
        }

        //
        // GET: /Test/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Discount discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role_Id = new SelectList(db.webpages_Roles, "RoleId", "RoleName", discount.Role_Id);
            return View(discount);
        }

        //
        // POST: /Test/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Discount discount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Role_Id = new SelectList(db.webpages_Roles, "RoleId", "RoleName", discount.Role_Id);
            return View(discount);
        }

        //
        // GET: /Test/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Discount discount = db.Discounts.Find(id);
            if (discount == null)
            {
                return HttpNotFound();
            }
            return View(discount);
        }

        //
        // POST: /Test/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Discount discount = db.Discounts.Find(id);
            db.Discounts.Remove(discount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}