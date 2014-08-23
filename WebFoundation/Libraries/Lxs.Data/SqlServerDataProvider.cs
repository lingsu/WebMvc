using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Data.Initializers;

namespace Lxs.Data
{
    public class SqlServerDataProvider : IDataProvider
    {
       

        public void InitDatabase()
        {
            //InitConnectionFactory();
            SetDatabaseInitializer();
        }

        public void InitConnectionFactory()
        {
            var connectionFactory =
                new SqlConnectionFactory(
                    "Data Source=.;Initial Catalog=lxsOa;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456)");

            Database.DefaultConnectionFactory = connectionFactory;
        }

        public void SetDatabaseInitializer()
        {
           var initializer = new CreateTablesIfNotExist<LxsObjectContext>();
           Database.SetInitializer(initializer);
           // Database.SetInitializer(new CreateDatabaseIfNotExists<LxsObjectContext>());
        }

        public DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        public bool StoredProceduredSupported
        {
            get { return true; }
        }
    }
}
