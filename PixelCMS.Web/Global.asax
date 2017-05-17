<%@ Application Codebehind="Global.asax.cs" Inherits="PixelCMS.Web.MvcApplication" Language="C#" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.IO.Compression" %>
 <script runat="server">
//protected void Application_BeginRequest(object sender, EventArgs e)
//{
//    HttpApplication app = (HttpApplication)sender;
//    string acceptEncoding = app.Request.Headers["Accept-Encoding"];
//    System.IO.Stream prevUncompressedStream = app.Response.Filter;

//    if (acceptEncoding == null || acceptEncoding.Length == 0)
//        return;

//    acceptEncoding = acceptEncoding.ToLower();

//    if (acceptEncoding.Contains("gzip"))
//    {
//        // gzip
//        app.Response.Filter = new System.IO.Compression.GZipStream(prevUncompressedStream,
//            System.IO.Compression.CompressionMode.Compress);
//        app.Response.AppendHeader("Content-Encoding",
//            "gzip");
//    }
//    else if (acceptEncoding.Contains("deflate"))
//    {
//        // defalte
//        app.Response.Filter = new System.IO.Compression.DeflateStream(prevUncompressedStream,
//            System.IO.Compression.CompressionMode.Compress);
//        app.Response.AppendHeader("Content-Encoding",
//            "deflate");
//    }
//}

//void Application_Start(object sender, EventArgs e)
//{
//    // Code that runs on application startup

//}

//void Application_End(object sender, EventArgs e)
//{
//    //  Code that runs on application shutdown

//}

//void Application_Error(object sender, EventArgs e)
//{
//    // Code that runs when an unhandled error occurs

//}

//void Session_Start(object sender, EventArgs e)
//{
//    // Code that runs when a new session is started

//}

//void Session_End(object sender, EventArgs e)
//{
//    // Code that runs when a session ends.
//    // Note: The Session_End event is raised only when the sessionstate mode
//    // is set to InProc in the Web.config file. If session mode is set to StateServer
//    // or SQLServer, the event is not raised.

//}

</script>


