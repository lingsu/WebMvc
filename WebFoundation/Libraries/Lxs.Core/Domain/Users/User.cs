using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Core.Domain.Users
{
    public class User:BaseEntity
    {
        public string Username { get; set; }

        public bool Active { get; set; }

        public bool Deleted { get; set; }

        public Guid UserGuid { get; set; }
    }
}
