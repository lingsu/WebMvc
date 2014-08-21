using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Infrastructure.DependencyManagement;

namespace Lxs.Core.Infrastructure
{
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        T[] ResolveAll<T>();
        void Initialize();
    }
}
