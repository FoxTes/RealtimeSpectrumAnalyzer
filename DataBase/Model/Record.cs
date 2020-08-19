using System.Data.Entity;

namespace Real_time_Spectrum_Analyzer.DataBase.Model
{
    public class Record
    {
        public int Id { get; set; }

        public int AxisX { get; set; }
        public int AxisY { get; set; }
        public int AxisZ { get; set; }

        public int? ProtocolId { get; set; }
        public virtual Protocol Protocol { get; set; }
    }
}
