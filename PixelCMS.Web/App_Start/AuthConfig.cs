using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using PixelCMS.Core.Models;
using WebMatrix.WebData;

namespace PixelCMS
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            WebSecurity.InitializeDatabaseConnection("pixeluserEntities", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

             OAuthWebSecurity.RegisterFacebookClient(
                 appId: "301207210065790",//
                 appSecret: "d12a02921bb8a80a871717524bdfebf0");


           


            // OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
