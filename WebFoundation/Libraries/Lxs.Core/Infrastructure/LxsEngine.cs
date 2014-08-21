using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Lxs.Core.Infrastructure.DependencyManagement;

namespace Lxs.Core.Infrastructure
{
    public class LxsEngine:IEngine
    {
        public ContainerManager ContainerManager { get; private set; }

        public T Resolve<T>() where T : class
        {

            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }


        public void Initialize()
        {
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            //dependencies
            var typeFinder = new WebAppTypeFinder();
            builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();
            builder.Update(container);

            //注册其它程序集的依赖关系
            builder = new ContainerBuilder();
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();

            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

            //排序 有些需要优先注册
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();

            foreach (var dependencyRegistrar in drInstances)
            {
                dependencyRegistrar.Register(builder, typeFinder);
            }
            builder.Update(container);


            var containerManager = new ContainerManager(container);
            ContainerManager = containerManager;
            //设置依赖解析
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


        }
    }
}
