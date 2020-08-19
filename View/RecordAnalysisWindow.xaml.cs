using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Real_time_Spectrum_Analyzer.View
{
    /// <summary>
    /// Логика взаимодействия для RecordAnalysisWindow.xaml
    /// </summary>
    public partial class RecordAnalysisWindow : UserControl
    {
        public RecordAnalysisWindow()
        {
            InitializeComponent();
        }

        private void OnOverviewSurfaceLoaded(object sender, RoutedEventArgs e)
        {
            OverviewSurface.ZoomExtents();
        }
    }
}
