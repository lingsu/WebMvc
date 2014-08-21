using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Infrastructure
{
    public class Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }
}
