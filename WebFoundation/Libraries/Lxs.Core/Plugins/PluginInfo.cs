using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Plugins
{
    public class PluginInfo : IComparable<PluginInfo>
    {
        public string SystemName { get; set; }

        public string FriendlyName { get; set; }

        public string ClassFullName { get; set; }
        public string Folder { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public List<string> SupVersion { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsInsert { get; set; }
        public virtual FileInfo OriginalAssemblyFile { get; internal set; }
        public virtual Assembly ReferencedAssembly { get; internal set; }
        public virtual Type PluginType { get; set; }

        public int CompareTo(PluginInfo other)
        {
            if (this.DisplayOrder > other.DisplayOrder)
            {
                return 1;
            }
            else if (this.DisplayOrder < other.DisplayOrder)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
