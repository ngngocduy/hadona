using PixelCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PixelCMS.Controllers
{
    public class ErrorsController : BaseController
    {
        //
        // GET: /Errors/

        public ActionResult NotFound(string culture)
        {
            return View();
        }

        public ActionResult UnderConstruction(string culture)
        {
            return View();
        }

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture
            return RedirectToAction("Index");
        }
    }
}
