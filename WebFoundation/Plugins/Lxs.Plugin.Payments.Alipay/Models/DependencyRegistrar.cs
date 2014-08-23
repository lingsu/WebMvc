using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Autofac;
using Lxs.Core.Infrastructure;
using Lxs.Core.Infrastructure.DependencyManagement;

namespace Lxs.Plugin.Payments.Alipay.Models
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            Debug.WriteLine("注册支付宝插件");
        }

        public int Order
        {
            get { return 10; }
        }
    }
}