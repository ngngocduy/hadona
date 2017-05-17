using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PixelCMS.Core.Models;
using PixelCMS.Helpers;

namespace PixelCMS.Controllers
{
    public class HomeController : BaseController
    {
        private pixelcmsEntities db = new pixelcmsEntities();

        public ActionResult Index(string culture)
        {
           // return View();
            try
            {
                var currentlang = db.Languages.FirstOrDefault(x => x.Code == culture);
                if (currentlang.Construction == true)
                {
                    return RedirectToAction("UnderConstruction", "Errors");
                }
                else if (currentlang.Active == false)
                {
                    return RedirectToAction("NotFound", "Errors");
                }
                return View();
            }
            catch
            {
                return RedirectToAction("NotFound", "Errors");
            }
        }

        public ActionResult Intro(string culture)
        {
            return View();
        }
        public ActionResult laithuxe(string culture)
        {

            return View();
        }
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            HttpCookie cookie = new HttpCookie("lang");
            cookie["culture"] = culture;
            Response.Cookies.Add(cookie);
            cookie.Expires = DateTime.Now.AddDays(1);

            //Session["CurrentLanguage"] = new CultureInfo(culture);

            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture
            return RedirectToAction("Index");
        }
    }
}
