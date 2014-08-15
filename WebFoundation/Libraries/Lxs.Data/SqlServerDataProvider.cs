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
            InitConnectionFactory();
            SetDatabaseInitializer();
        }

        public void InitConnectionFactory()
        {
            var connectionFactory = new SqlConnectionFactory();

            Database.DefaultConnectionFactory = connectionFactory;
        }

        public void SetDatabaseInitializer()
        {
            var initializer = new CreateTablesIfNotExist<LxsObjectContext>();
            Database.SetInitializer(initializer);
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
