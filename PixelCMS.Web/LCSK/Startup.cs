using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PixelCMS.Web.LCSK.Startup))]

namespace PixelCMS.Web.LCSK
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			// If you already have a Startup class, please move this to your 
			// own Startup class and delete this file
            app.MapSignalR();
        }
    }
}
