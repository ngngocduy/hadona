using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;
using PixelCMS.Core.Models;
using PixelCMS.Controllers;

namespace PixelCMS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        #region Declare
        pixelcmsEntities db = new pixelcmsEntities();
        #endregion

        #region Counter
        protected void Application_Start()
        {
            //Tao biến applcation đếm số lượng truy cập
            Application["HomNay"] = 0;
            Application["HomQua"] = 0;
            Application["TuanNay"] = 0;
            Application["TuanTruoc"] = 0;
            Application["ThangNay"] = 0;
            Application["ThangTruoc"] = 0;
            Application["TatCa"] = 0;
            Application["visitors_online"] = 0;

            AreaRegistration.RegisterAllAreas();
           //  var display = DisplayModeProvider.Instance.Modes;
            var mobileModel = DisplayModeProvider.Instance.Modes.FirstOrDefault(a => a.DisplayModeId == "Mobile");
            if (mobileModel != null)
            {
                DisplayModeProvider.Instance.Modes.Remove(mobileModel);
            }

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;

            AuthConfig.RegisterAuth();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        /// <summary>
        /// Thongs the ke truy cap.
        /// </summary>
        /// <returns></returns>
        /// <author> Ba Luan </author>
        /// <created> 12/24/2013 10:14 AM </created>
        public static IEnumerable<sp_CMS_TKTruyCap_Result> ThongKeTruyCap()
        {
            var context = new pixelcmsEntities();
            var st = context.sp_CMS_TKTruyCap();
            return st;
        }
        void Session_Start(object sender, EventArgs e)
        {
            Session.Timeout = 150;
            Application.Lock();
            Application["visitors_online"] = Convert.ToInt32(Application["visitors_online"]) + 1;
            Application.UnLock();
            try
            {
                IEnumerable<sp_CMS_TKTruyCap_Result> s = ThongKeTruyCap();
                var item = s.FirstOrDefault();
                Application["HomNay"] = item.HomNay.NullIsZero();
                Application["HomQua"] = item.HomQua.NullIsZero();
                Application["TuanNay"] = item.TuanNay.NullIsZero();
                Application["TuanTruoc"] = item.TuanTruoc.NullIsZero();
                Application["ThangNay"] = item.ThangNay.NullIsZero();
                Application["ThangTruoc"] = item.ThangTruoc.NullIsZero();
                Application["TatCa"] = item.TatCa.NullIsZero();
            }
            catch { }
            Response.Redirect("/introduce");
        }

        void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["visitors_online"] = Convert.ToUInt32(Application["visitors_online"]) - 1;
            Application.UnLock();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest()
        {

        }
        #endregion

        protected void Application_Error(object sender, EventArgs e)
        {
            //Server.ClearError();
            //Response.RedirectToRoute("Errors", new { culture = PixelCMS.Helpers.CultureHelper.GetCurrentNeutralCulture() });
        }
      /*  protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session != null)
            {
                CultureInfo ci = (CultureInfo)Session["CurrentLanguage"];

                if (ci == null)
                {
                    ci = new CultureInfo("");
                    Session["CurrentLanguage"] = ci;
                }
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);


            }
        }*/
    }
    
}