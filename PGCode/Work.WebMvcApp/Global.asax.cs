using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.Optimization;

namespace DotWeb.AppStart
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        String VirCookie = "SouthWestUniversity";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("en-US")
            {
                ContextCondition = (Context => (Context.Request.Cookies[VirCookie + ".Lang"].Value == "en-US"))
            });

            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode("zh-CN")
            {
                ContextCondition = (Context => (Context.Request.Cookies[VirCookie + ".Lang"].Value == "zh-CN"))
            });

        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpCookie WebLang = Request.Cookies[VirCookie + ".Lang"];

            if (WebLang == null)
            {
                //WebLang = new HttpCookie("CarPurchase.Lang", System.Globalization.CultureInfo.CurrentCulture.Name);
                WebLang = new HttpCookie(VirCookie + ".Lang", "en-US");
                Response.Cookies.Add(WebLang);
            }

            if (WebLang != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(WebLang.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(WebLang.Value);
            }
        }
    }
}