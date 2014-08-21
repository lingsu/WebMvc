using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Lxs.Core.Infrastructure;
using StackExchange.Profiling;

namespace Lxs.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            EngineContext.Initialize(false);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        
        protected void Application_BeginRequest()
        {
            //开启性能检测
            MiniProfiler.Start();
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}
