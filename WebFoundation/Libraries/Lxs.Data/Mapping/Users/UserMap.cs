using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Domain.Users;

namespace Lxs.Data.Mapping.Users
{
    public class UserMap:EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("User");
            //Property(x=>x.Username)
        }
    }
}
