using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Domain.Common
{
    public class GenericAttribute:BaseEntity
    {
       /// <summary>
       /// 用户id
       /// </summary>
        public int EntityId { get; set; }

        public string KeyGroup { get; set; }

        
        public string Key { get; set; }

      
        public string Value { get; set; }
    }
}
