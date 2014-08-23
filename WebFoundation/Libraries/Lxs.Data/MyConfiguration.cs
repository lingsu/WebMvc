using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Data
{
    public class MyConfiguration:DbConfiguration
    {
        protected internal MyConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient",()=>new SqlAzureExecutionStrategy());
            SetDefaultConnectionFactory(new SqlConnectionFactory("Data Source=.;Initial Catalog=lxsOa;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456)"));
        }
    }
}
