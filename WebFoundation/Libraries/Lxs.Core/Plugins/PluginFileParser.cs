using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Plugins
{
    public static class PluginFileParser
    {
        public static IList<string> ParseInstalledPluginsFile(string path)
        {
            var installeds = new List<string>();

            if (!File.Exists(path))
                return installeds;

            var text = File.ReadAllText(path);

            if (String.IsNullOrEmpty(text))
                return installeds;

            using (var reader=new StringReader(text))
            {
                string str;
                while ((str=reader.ReadLine())!=null)
                {
                    if (!String.IsNullOrWhiteSpace(str))
                        installeds.Add(str.Trim());
                }
            }

            return installeds;
        } 
    }
}
