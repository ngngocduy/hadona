using System.Web;
using System.Web.Optimization;

namespace PixelCMS
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-1.11.0.min.js"
                //  "~/Scripts/jquery-1.8.2.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(

                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/additional-methods.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/Theme/js/js").Include(
                "~/Theme/js/jquery-1.7.1.min.js",
                "~/Theme/js/jquery.easing.1.3.js",
                "~/Theme/js/jquery-ui-1.10.3.min.js",
                "~/Theme/js/jquery.flexslider.js",
                "~/Theme/js/idangerous.swiper-1.9.min.js",
                "~/Theme/js/jquery.sharrre-1.3.4.min.js",
                "~/Theme/js/kenburns.js",
                "~/Theme/js/twitter/jquery.tweet.js",
                "~/Theme/js/jquery.fitvids.min.js",
                "~/Theme/js/jquery.jqzoom-core-pack.js",
                "~/Theme/js/jquery.prettyPopin.js",
                "~/Theme/js/jquery.prettyPhoto.js",
                "~/Theme/js/jquery.prettyPhoto.js",
                "~/Theme/js/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                // "~/Content/jqueryui/jquery-ui-1.11.2.custom/jquery-ui.min.js"
                       "~/Content/jqueryui/xam/jquery-ui-1.11.2.custom/jquery-ui.js"
                       ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
           /* bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                       "~/Scripts/modernizr-*"));*/

            bundles.Add(new StyleBundle("~/Content/jqueryui/css").Include(
                // màu xám 
                        "~/Content/jqueryui/xam/jquery-ui-1.11.2.custom/jquery-ui.theme.min.css",
                        "~/Content/jqueryui/xam/jquery-ui-1.11.2.custom/jquery-ui.structure.min.css",
                        "~/Content/jqueryui/xam/jquery-ui-1.11.2.custom/jquery-ui.min.css"
                        ));

            bundles.Add(new StyleBundle("~/Areas/Admin/Content/css/css").Include(
                        "~/Areas/Admin/Content/css/base.css",
                        "~/Areas/Admin/Content/css/skeleton.css",
                        "~/Areas/Admin/Content/css/layout.css",
                        "~/Areas/Admin/Content/css/font-awesome.min.css",
                        "~/Areas/Admin/Content/css/style.css",
                        "~/Areas/Admin/Content/css/jquery.dropdown.css",
                        "~/Areas/Admin/Content/css/PagedList.css"));

            bundles.Add(new StyleBundle("~/Theme/css/css").Include(
                "~/Theme/css/reset.css",
                "~/Theme/css/text.css",
                "~/Theme/css/prettyPhoto.css",
                "~/Theme/css/jquery.jqzoom.css",
                "~/Theme/css/icons/icons.css",
                "~/Theme/css/custom.css",
                "~/Theme/css/responsive.css",
                "~/Theme/css/styles/deep-blue.css",
                "~/Content/Common.css",
                "~/Content/PagedList.css"));
            #region elFinder bundles
            bundles.Add(new ScriptBundle("~/Scripts/elfinder").Include(
                             "~/Scripts/elfinder/js/elfinder.full.js"
                //"~/Content/elfinder/js/i18n/elfinder.pt_BR.js"
                             ));

            bundles.Add(new StyleBundle("~/Content/elfinder").Include(
                            "~/Scripts/elfinder/css/elfinder.full.css",
                            "~/Scripts/elfinder/css/theme.css"));

            #endregion

            // SignalR bundle
            bundles.Add(new ScriptBundle("~/bundles/SignalR").Include(
                        "~/Scripts/jquery.signalR-{version}.js"));


            //Mobile
            bundles.Add(new ScriptBundle("~/bundles/MobileJS").Include(
                        "~/Scripts/jquery.mobile-1.*",
                        "~/Scripts/jquery-1.*"
                        ));

            bundles.Add(new StyleBundle("~/Content/Mobile/css").Include(
                       "~/Content/jquery.mobile-1.4.5.min.css",
                       "~/Content/jquery.mobile.structure-1.4.5.min.css",
                       "~/Content/jquery.mobile.theme-1.4.5.min.css"

                       ));
            //End Mobile
        }
    }
}