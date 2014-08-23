using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lxs.Data.Initializers
{
    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public CreateTablesIfNotExist()
            : base()
        {
        }

        public void InitializeDatabase(TContext context)
        {
            Debug.WriteLine("ef 初始化");
            bool dbExists;
            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                dbExists = context.Database.Exists();
            }
            if (!dbExists)
            {
                var sqlbuilder = new SqlConnectionStringBuilder("Data Source=.;Initial Catalog=lxsOa;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456");

                var databaseName = sqlbuilder.InitialCatalog;
                //now create connection string to 'master' dabatase. It always exists.
                sqlbuilder.InitialCatalog = "master";
                var masterCatalogConnectionString = sqlbuilder.ToString();
                string query = string.Format("CREATE DATABASE [{0}]", databaseName);

                var collation = "";
                if (!String.IsNullOrWhiteSpace(collation))
                    query = string.Format("{0} COLLATE {1}", query, collation);
                using (var conn = new SqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            if (!dbExists)
            {
                var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                context.Database.ExecuteSqlCommand(dbCreationScript);

                //Seed(context);
                context.SaveChanges();
            }
            //else
            //{
            //    throw new ApplicationException("No database instance");
            //}
            
        }
    }
}
