using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Lxs.Core;
using Lxs.Core.Infrastructure;
using Lxs.Core.Infrastructure.DependencyManagement;
using Lxs.Services.Authentication;
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


            Debug.WriteLine("hello world!");
        }

        public int Order
        {
            get { return -1000; }
        }
    }
}
