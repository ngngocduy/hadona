using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PixelCMS.Core.Models;
using Simple.ImageResizer.MvcExtensions;

namespace PixelCMS.Controllers
{
    public class ImagesController : Controller
    {
        [OutputCache(VaryByParam = "*", Duration = 60 * 60 * 24 * 365)]
        public ImageResult Index(string filename, int w = 0, int h = 0)
        {
            string filepath = Path.Combine(Server.MapPath("~/Upload/images"), filename);
            return new ImageResult(filepath, w, h);
        }
    }
}