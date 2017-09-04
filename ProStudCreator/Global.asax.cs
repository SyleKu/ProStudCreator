using System;
using System.Reflection;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using iTextSharp.text.io;

namespace ProStudCreator
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code, der beim Anwendungsstart ausgeführt wird
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StreamUtil.AddToResourceSearch(Assembly.Load("itext-hyph-xml"));
        }
    }
}