using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElFinder;
using PagedList;
using PixelCMS.Areas.Admin.Controllers;
using PixelCMS.Core.Models;

namespace PixelCMS.Areas.Admin.Controllers
{
    public class FileManagementController : BaseController
    {
        public virtual ActionResult Index(string folder, string subFolder)
        {
            FileSystemDriver driver = new FileSystemDriver();

            var root = new Root(
                    new DirectoryInfo(Server.MapPath("~/Upload" )),
                    "http://" + Request.Url.Authority + "/Upload/")
            {
                // Sample using ASP.NET built in Membership functionality...
                // Only the super user can READ (download files) & WRITE (create folders/files/upload files).
                // Other users can only READ (download files)
                // IsReadOnly = !User.IsInRole(AccountController.SuperUser)

                IsReadOnly = false, // Can be readonly according to user's membership permission
                Alias = "Upload", // Beautiful name given to the root/home folder
                MaxUploadSizeInKb = 50000, // Limit imposed to user uploaded file <= 500 KB
                LockedFolders = new List<string>(new string[] { "Folder1" })
            };

            // Was a subfolder selected in Home Index page?
            if (!string.IsNullOrEmpty(subFolder))
            {
                root.StartPath = new DirectoryInfo(Server.MapPath("~/Upload/"+ subFolder));
            }
            driver.AddRoot(root);

            var connector = new Connector(driver);

            return connector.Process(this.HttpContext.Request);
        }

        public virtual ActionResult SelectFile(string target)
        {
            FileSystemDriver driver = new FileSystemDriver();

            driver.AddRoot(
                new Root(
                    new DirectoryInfo(Server.MapPath("~/Upload")),
                    "http://" + Request.Url.Authority + "/Upload") { IsReadOnly = false });

            var connector = new Connector(driver);

            return Json(connector.GetFileByHash(target).FullName);
        }
    }
}
