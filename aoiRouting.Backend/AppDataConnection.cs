using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using aoiRouting.Backend.Models;
using UserManagement.Models;
using aoiRouting.Shared.Models;
namespace aoiRouting.Backend
{
    public class AppDataConnection : DataConnection
    {
        public ITable<Pin> Pins => GetTable<DbPin>();
        public ITable<AOI> AOIs => GetTable<DbAOI>();
        public AppDataConnection(LinqToDbConnectionOptions<AppDataConnection> options) : base(options) { }
    }
}