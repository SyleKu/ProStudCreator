using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Optimization;
using System.Web.Routing;
using iTextSharp.text.io;

namespace ProStudCreator
{
    public class Global : HttpApplication
    {

        private const string DummyCacheItemKey = "DummyCacheItem";
        private string DummyPage = "CheckAllTasks.aspx";

        private void Application_Start(object sender, EventArgs e)
        {
            // Code, der beim Anwendungsstart ausgeführt wird
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StreamUtil.AddToResourceSearch(Assembly.Load("itext-hyph-xml"));

            Application.Add("dummyRequest", "");

            //TaskHandler.CheckAllTasks();
        }


        private void RegisterChacheEntry()
        {
            if (null != HttpContext.Current.Cache[DummyCacheItemKey]) return;
            HttpContext.Current.Cache.Add(DummyCacheItemKey, "DummyTest", null,
                DateTime.MaxValue, TimeSpan.FromMinutes(1),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));
        }


        public void CacheItemRemovedCallback(string key,
            object value, CacheItemRemovedReason reason)
        {
            HitPage();

            // Do the service works
            CheckAllTasks();
        }

        private void HitPage()
        {
            var client = new WebClient();
            client.DownloadData(Application.Get("dummyRequest").ToString());
        }



        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (Application.Get("dummyRequest").ToString() == "")
            {
                Application.Set("dummyRequest", "http://" + Request.ServerVariables["HTTP_HOST"] + "/" + DummyPage);
                RegisterChacheEntry();
            }

            if (HttpContext.Current.Request.Url.ToString() == Application.Get("dummyRequest").ToString())
            {
                //Add the item in cache and when succesful, do the work.

                RegisterChacheEntry();
            }


        }


        private void CheckAllTasks() //checks for open tasks And generate Emails to remind users of their open Tasks
        {
            //TaskHandler.CheckAllTasks();
        }
    }
}