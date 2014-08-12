using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using Lxs.Core.ComponentModel;
using Lxs.Core.Plugins;

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]
namespace Lxs.Core.Plugins
{
    public class PluginManager
    {
        #region Const

        private const string InstalledPluginsFilePath = "/App_Data/InstalledPlugin.txt";//插件安装文件
        private const string PluginsPath = "/Plugins";//插件目录
        private const string ShadowCopyPath = "/Plugins/bin";//插件影子目录

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;
        private static bool _clearShadowDirectoryOnStartup = true;

        public static IEnumerable<PluginInfo> ReferencedPlugins { get; set; }

        #endregion

        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {
                //插件目录
                var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
                //插件bin目录
                _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ShadowCopyPath));

                var referencedPlugins = new List<PluginInfo>();

                try
                {
                    var installedPlugins = PluginFileParser.ParseInstalledPluginsFile(HostingEnvironment.MapPath(InstalledPluginsFilePath));

                    if (!pluginFolder.Exists)
                    {
                        pluginFolder.Create();
                    }
                    if (!_shadowCopyFolder.Exists)
                    {
                        _shadowCopyFolder.Create();
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


                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolder))
                    {
                        var pluginFile = dfd.Key;
                        var pluginInfo = dfd.Value;

                        if(!pluginInfo.SupVersion.Contains(LxsVersion.CurrentVersion))
                            continue;

                        pluginInfo.IsInsert =
                            installedPlugins.FirstOrDefault(
                                x => x.Equals(pluginInfo.SystemName, StringComparison.InvariantCultureIgnoreCase)) != null;

                        try
                        {
                            var pluginFiles = pluginFile.Directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                            var mainPluginFile = pluginFiles.FirstOrDefault(x => x.Name.Equals(pluginInfo.Folder, StringComparison.InvariantCultureIgnoreCase));
                            
                            pluginInfo.OriginalAssemblyFile = mainPluginFile;
                            pluginInfo.ReferencedAssembly = PerformFileDeploy(mainPluginFile);

                            foreach (var fileInfo in pluginFiles.Where(x => ! x.Name.Equals(mainPluginFile.Name, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                PerformFileDeploy(fileInfo);
                            }

                            foreach (var t in pluginInfo.ReferencedAssembly.GetTypes())
                            {
                                Debug.WriteLine(t.Name);
                                if (typeof(IPlugin).IsAssignableFrom(t))
                                {
                                    if (!t.IsInterface)
                                    {
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            pluginInfo.PluginType = t;
                                            break;
                                        }
                                    }
                                }
                            }

                            referencedPlugins.Add(pluginInfo);

                        }
                        catch (ReflectionTypeLoadException ex)
                        {

                            throw new Exception(ex.Message, ex); ;
                        }

                    }

                    ReferencedPlugins = referencedPlugins;

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message, ex); ;
                }

            }
        }

        private static Assembly PerformFileDeploy(FileInfo plug)
        {
            FileInfo shadowCopiedPlug;

            if (CommonHelper.GetTrustLevel() == AspNetHostingPermissionLevel.Unrestricted)
            {
                var directory = AppDomain.CurrentDomain.DynamicDirectory;
                shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
            }
            else
            {
                var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
                shadowCopiedPlug = InitializeFullTrust(plug, shadowCopyPlugFolder);
            }

            var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));
            BuildManager.AddReferencedAssembly(shadowCopiedAssembly);

            return shadowCopiedAssembly;
        }

      

        private static FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo directoryInfo)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(directoryInfo.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName,shadowCopiedPlug.FullName,true);
            }
            catch (IOException)
            {
                var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".lock";
                File.Move(shadowCopiedPlug.FullName,oldFile);

                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }

        private static IEnumerable<KeyValuePair<FileInfo, PluginInfo>> GetDescriptionFilesAndDescriptors(
            DirectoryInfo pluginFolder)
        {

            var result = new List<KeyValuePair<FileInfo, PluginInfo>>();

            foreach (var plugin in pluginFolder.GetFiles("Plugin.txt", SearchOption.AllDirectories))
            {
                var pluginInfo = ParsePluginInfo(plugin.FullName);
                result.Add(new KeyValuePair<FileInfo, PluginInfo>(plugin,pluginInfo));
            }

            result.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

            return result;
        }

        private static PluginInfo ParsePluginInfo(string path)
        {
            var pluginInfo = new PluginInfo();
            var text = File.ReadAllText(path);

            if (String.IsNullOrEmpty(text)) ;

            var settings =new List<string>();
            using (var reader=new StringReader(text))
            {
                string str;
                while (( str = reader.ReadLine()) !=null)
                {
                    if(String.IsNullOrEmpty(str))
                        continue;
                    settings.Add(str.Trim());
                }
            }

            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(":");
                if (separatorIndex == -1)
                    continue;

                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex+1).Trim();

                switch (key)
                {
                    case "SystemName":
                        pluginInfo.SystemName = value;
                        break;
                    case "FriendlyName":
                        pluginInfo.FriendlyName = value;
                        break;
                    case "ClassFullName":
                        pluginInfo.ClassFullName = value;
                        break;
                    case "Folder":
                        pluginInfo.Folder = value;
                        break;
                    case "Description":
                        pluginInfo.Description = value;
                        break;
                    case "Type":
                        pluginInfo.Type = value;
                        break;
                    case "Author":
                        pluginInfo.Author = value;
                        break;
                    case "Version":
                        pluginInfo.Version = value;
                        break;
                    case "SupVersion":
                    {
                        pluginInfo.SupVersion =
                            value.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .ToList();

                    }
                        break;
                    case "DisplayOrder":
                    {
                        int displayOrder;
                        int.TryParse(value, out displayOrder);
                        pluginInfo.DisplayOrder = displayOrder;
                        
                    }
                        break;
                    case "IsDefault":
                    {
                        bool isDefaul;
                        Boolean.TryParse(value, out isDefaul);
                        pluginInfo.IsDefault = isDefaul;
                    }
                        break;
                }
            }

            return pluginInfo;
        }



        public static void MarkPluginAsInstalled(string p)
        {
            var filePath = HostingEnvironment.MapPath(InstalledPluginsFilePath);


        }

        public static void MarkPluginAsUninstalled(string p)
        {
            
        }
    }
}
