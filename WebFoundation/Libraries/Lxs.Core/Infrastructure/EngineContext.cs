using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Infrastructure
{
    public class EngineContext
    {
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        public static IEngine Current
        {
            get { return Singleton<IEngine>.Instance; }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IEngine>.Instance == null || forceRecreate)
            {
                Singleton<IEngine>.Instance = CreateEngineInstance();
                Singleton<IEngine>.Instance.Initialize();
            }

            return Singleton<IEngine>.Instance;
        }

        private static IEngine CreateEngineInstance()
        {
            return new LxsEngine();
        }
    }
}
