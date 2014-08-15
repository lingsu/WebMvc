using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Data.Initializers;

namespace Lxs.Data
{
    public class EfStartUpTask:IStartupTask
    {
        public void Execute()
        {
            var sqlProvider = new SqlServerDataProvider();
            sqlProvider.InitDatabase();
        }

        public int Order
        {   
            //确保运行这个任务放在第一位
            get { return -1000; }
        }
    }
}
