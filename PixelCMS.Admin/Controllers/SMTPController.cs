using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class SMTPController : Controller
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        //
        // GET: /SMTP/

        public ActionResult Index()
        {
            return View(db.SMTPs.ToList());
        }

        //
        // GET: /SMTP/Details/5

        public ActionResult Details(int id = 0)
        {
            SMTP smtp = db.SMTPs.Find(id);
            if (smtp == null)
            {
                return HttpNotFound();
            }
            return View(smtp);
        }

        //
        // GET: /SMTP/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SMTP/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SMTP smtp)
        {
            if (ModelState.IsValid)
            {
                db.SMTPs.Add(smtp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(smtp);
        }

        //
        // GET: /SMTP/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SMTP smtp = db.SMTPs.Find(id);
            if (smtp == null)
            {
                return HttpNotFound();
            }
            return View(smtp);
        }

        //
        // POST: /SMTP/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SMTP smtp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(smtp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(smtp);
        }

        //
        // GET: /SMTP/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SMTP smtp = db.SMTPs.Find(id);
            if (smtp == null)
            {
                return HttpNotFound();
            }
            return View(smtp);
        }

        //
        // POST: /SMTP/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SMTP smtp = db.SMTPs.Find(id);
            db.SMTPs.Remove(smtp);
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