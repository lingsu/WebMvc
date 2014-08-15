using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Caching
{
    public static class CacheExtensions
    {
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> func)
        {

            return Get(cacheManager,key,60,func);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> func)
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }

            var m = func();

            if (cacheTime > 0)
                cacheManager.Set(key, m, cacheTime);

            return m;
        } 
    }
}
