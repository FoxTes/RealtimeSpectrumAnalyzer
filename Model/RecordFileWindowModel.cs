using Real_time_Spectrum_Analyzer.Service;
using System.Windows.Media;

namespace Real_time_Spectrum_Analyzer.Model
{
    public class RecordFileWindowModel : ViewModelBase
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private Color _colorObject;
        public Color ColorObject
        {
            get { return _colorObject; }
            set
            {
                _colorObject = value;
                OnPropertyChanged("ColorObject");
            }
        }

        private Brush _foregroundColor = Brushes.DarkSeaGreen;
        public Brush ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                _foregroundColor = value;
                OnPropertyChanged("ForegroundColor");
            }
        }
    }
}
