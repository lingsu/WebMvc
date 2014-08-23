using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Lxs.Core.Infrastructure;
using Lxs.Data;
using StackExchange.Profiling;

namespace Lxs.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            if (HttpContext.Current != null)
            {
                Debug.WriteLine("HttpContext.Current != null");
            }
            EngineContext.Initialize(false);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //Entity Framework  预热
            //Entity Framework的版本至少是6.0才支持
            //对程序中定义的所有DbContext逐一进行这个操作

            var dbcontext = EngineContext.Current.Resolve<IDbContext>();

            var objectContext = ((IObjectContextAdapter) dbcontext).ObjectContext;
            var mappingCollection =
                (StorageMappingItemCollection) objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
            mappingCollection.GenerateViews(new List<EdmSchemaError>());


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
