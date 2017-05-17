using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PixelCMS.Helpers;

namespace PixelCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // elFinder's connector route
            routes.MapRoute(null, "connector", new { controller = "FileManagement", action = "Index" });
             //routes.IgnoreRoute("elfinder.connector");
            #region Special Page
            // Admin
            routes.MapRoute(
                name: "Admin",
                url: "quantriweb/",
                defaults: new { area = "Admin", controller = "dashboard", action = "index" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
            //image
            routes.MapRoute(
                "Image", "images/{filename}",
                new {controller = "Images", action = "Index", filename = "" }
            );

            routes.MapRoute(
                name: "Errors",
                url: "{culture}/Errors/NotFound",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Errors", action = "NotFound" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            routes.MapRoute(
                name: "UnderConstruction",
                url: "{culture}/Errors/UnderConstruction",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Errors", action = "UnderConstruction" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );


            //// intro
            routes.MapRoute(
                name: "intro",
                url: "introduce",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Intro" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
            //// Home
            //routes.MapRoute(
            //    name: "Home",
            //    url: "{culture}/Home/{action}",
            //    defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", type = "", slug = "" },
            //    namespaces: new[] { "PixelCMS.Controllers" }
            //);

            // home
            routes.MapRoute(
                name: "home",
                url: "{culture}/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "index" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // brand
            routes.MapRoute(
                name: "Brand",
                url: "{culture}/Brand/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "BrandList", id = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
            // minidetail
            routes.MapRoute(
                name: "minidetail",
                url: "{culture}/minidetail/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "minidetail", id = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // Common
            routes.MapRoute(
                name: "Common",
                url: "Common/{action}/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Common", action = "", id = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // Common
            routes.MapRoute(
                name: "Widget",
                url: "Widget/{action}/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Widget", action = "", id = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // Login
            routes.MapRoute(
                name: "LoginPanel",
                url: "account/LoginPanel",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Account", action = "LoginPanel" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // Login
            routes.MapRoute(
                name: "Account",
                url: "{culture}/account/{action}/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // Login
            routes.MapRoute(
                name: "extend",
                url: "account/ExternalLoginConfirmation",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Account", action = "ExternalLoginConfirmation" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // lien he
            routes.MapRoute(
                name: "lien he",
                url: "{culture}/lien-he/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "Contact" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
            // Contact
            routes.MapRoute(
                name: "Contact",
                url: "{culture}/contact/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "Contact" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
            //  联系
            routes.MapRoute(
                name: " 联系",
                url: "{culture}/联系/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "Contact" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
           
            // chat
            routes.MapRoute(
                name: "chat",
                url: "{culture}/chat/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "chat", action = "index" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // wishtlist
            routes.MapRoute(
                name: "wishtlist",
                url: "{culture}/wishtlist/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "WishLists" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // wishtlist
            routes.MapRoute(
                name: "CompareList",
                url: "{culture}/CompareList/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "CompareList" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // Search
            routes.MapRoute(
                name: "Search",
                url: "{culture}/Search/",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "Search" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );

            // cart
            routes.MapRoute(
                name: "ShoppingCart",
                url: "{culture}/ShoppingCart/{action}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "ShoppingCart", action = "Index" },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
            #endregion

            // Slugf
           /* routes.MapRoute(
                name: "Slug",
                url: "{culture}/{slug1}/{slug2}/{slug3}/{slug4}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "Slug", slug2 = UrlParameter.Optional, slug3 = UrlParameter.Optional, slug4 = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );*/

            routes.MapRoute(
               name: "SlugLang",
               url: "{culture}/{slug1}/{slug2}/{slug3}/{slug4}",
               defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Content", action = "Slug", slug1 = UrlParameter.Optional, slug2 = UrlParameter.Optional, slug3 = UrlParameter.Optional, slug4 = UrlParameter.Optional },
               constraints: new { culture = "[a-z]{2}" },
               namespaces: new[] { "PixelCMS.Controllers" }
           );

           /* routes.MapRoute(
                name: "Slug",
                url: "{slug1}/{slug2}/{slug3}/{slug4}",
                defaults: new { controller = "Content", action = "Slug", slug1 = UrlParameter.Optional, slug2 = UrlParameter.Optional, slug3 = UrlParameter.Optional, slug4 = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );*/

            // Default  
            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PixelCMS.Controllers" }
            );
        }
    }
}