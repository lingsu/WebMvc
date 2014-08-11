using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Lxs.Core.ComponentModel;
using Lxs.Core.Plugins;

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
namespace Lxs.Core.Plugins
{
    public class PluginManager
    {
        #region Const

        private const string InstalledPluginsFilePath = "/App_Data/InstalledPlugin.config";//插件安装文件
        private const string PluginsPath = "/Plugins";//插件目录
        private const string ShadowCopyPath = "/Plugins/bin";//插件影子目录

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
       // private static DirectoryInfo _shadowCopyFolder;
        private static bool _clearShadowDirectoryOnStartup = true;

        #endregion

        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {
                //插件目录
                var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
                //插件bin目录
                var _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ShadowCopyPath));

                try
                {
                    if (!pluginFolder.Exists)
                    {
                        Directory.CreateDirectory(pluginFolder.FullName);
                    }
                    if (!_shadowCopyFolder.Exists)
                    {
                        Directory.CreateDirectory(_shadowCopyFolder.FullName);
                    }
                    else
                    {
                        if (_clearShadowDirectoryOnStartup)
                        {
                            //清空影子复制目录中的dll文件
                            foreach (var f in _shadowCopyFolder.GetFiles())
                            {
                                Debug.WriteLine("Deleting " + f.Name);
                                f.Delete();
                            }
                        }
                    }

                    
                }
                catch (Exception)
                {
                    
                    throw;
                }

            }

        }

    }
}
