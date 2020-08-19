using System.Collections.Generic;

namespace Real_time_Spectrum_Analyzer.DataBase.Model
{
    public class Protocol
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Record> Records { get; set; }

        public Protocol()
        {
            Records = new List<Record>();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
