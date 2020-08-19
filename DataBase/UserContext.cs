using Real_time_Spectrum_Analyzer.DataBase.Model;
using System.Data.Entity;

namespace Real_time_Spectrum_Analyzer.DataBase
{
    class UserContext : DbContext
    {
        public UserContext() : base("DbConnection") { }

        public DbSet<Record> Records { get; set; }
        public DbSet<Protocol> Protocols { get; set; }
    }
}
