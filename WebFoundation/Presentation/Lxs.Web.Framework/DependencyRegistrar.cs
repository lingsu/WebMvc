using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using Lxs.Core;
using Lxs.Core.Data;
using Lxs.Core.Infrastructure;
using Lxs.Core.Infrastructure.DependencyManagement;
using Lxs.Data;
using Lxs.Services.Authentication;
using Lxs.Services.Installation;
using Lxs.Services.Users;

namespace Lxs.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.Register(x => new HttpContextWrapper(HttpContext.Current) as HttpContextBase)
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            builder.RegisterType<WebHelper>().As<IWebHelper>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserService>().As<IUserService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FormsAuthenticationService>()
                .As<IAuthenticationService>()
                .InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer

            IDataProvider dataProvider = new SqlServerDataProvider();
            dataProvider.InitConnectionFactory();


            builder.Register<IDbContext>(
                c =>
                    new LxsObjectContext(
                        "Data Source=.;Initial Catalog=lxsOa;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456"))
                .InstancePerLifetimeScope();


            //var sqlbuilder = new SqlConnectionStringBuilder("Data Source=.;Initial Catalog=lxsOa;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456");

           //var databaseName = sqlbuilder.InitialCatalog;
           ////now create connection string to 'master' dabatase. It always exists.
           //sqlbuilder.InitialCatalog = "master";
           //var masterCatalogConnectionString = sqlbuilder.ToString();
           //string query = string.Format("CREATE DATABASE [{0}]", databaseName);

           // var collation = "";
           //if (!String.IsNullOrWhiteSpace(collation))
           //    query = string.Format("{0} COLLATE {1}", query, collation);
           //using (var conn = new SqlConnection(masterCatalogConnectionString))
           //{
           //    conn.Open();
           //    using (var command = new SqlCommand(query, conn))
           //    {
           //        command.ExecuteNonQuery();
           //    }
           //}
            dataProvider.InitDatabase();
            //builder.Register<IDbContext>(
            //    x =>
            //        new LxsObjectContext(
            //            "Data Source=.;Initial Catalog=lxsOa;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456"))
            //    .InstancePerLifetimeScope();


            

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();


            //plugins
           // builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();



            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            builder.RegisterType<CodeFirstInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();


            Debug.WriteLine("hello world!");
        }

        public int Order
        {
            get { return -1000; }
        }
    }
}
