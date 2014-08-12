using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        protected BasePlugin()
        {
        }

        public virtual PluginInfo PluginInfo { get; set; }
        public virtual void Install()
        {
            PluginManager.MarkPluginAsInstalled(this.PluginInfo.SystemName);
        }

        public virtual void Uninstall()
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginInfo.SystemName);
        }
    }
}
