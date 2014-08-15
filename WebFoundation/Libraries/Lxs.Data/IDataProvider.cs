using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lxs.Data
{
    public interface IDataProvider
    {
        void InitDatabase();
        void InitConnectionFactory();
        void SetDatabaseInitializer();
        DbParameter GetParameter();
        bool StoredProceduredSupported { get; }
    }
}
