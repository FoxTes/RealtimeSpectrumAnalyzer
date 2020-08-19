using Real_time_Spectrum_Analyzer.Service;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Examples.ExternalDependencies.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Real_time_Spectrum_Analyzer.ViewModel
{
    class RecordAnalysisWindowViewModel : ViewModelBase, IPageViewModel
    {
        public string Name 
        {
            get { return "RecordAnalysisWindow"; }
        }

        private const int SeriesCount = 1;
        private const int PointCount = 400 * 30;

        public RecordAnalysisWindowViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            RenderableSeriesViewModels = new ObservableCollection<IRenderableSeriesViewModel>();
            var generator = new RandomWalkGenerator();

            for (int i = 0; i < SeriesCount; i++)
            {
                var dataSeries = new XyDataSeries<double, double>();
                var someData = generator.GetRandomWalkSeries(PointCount);

                generator.Reset();
                dataSeries.Append(someData.XData, someData.YData);

                RenderableSeriesViewModels.Add(new LineRenderableSeriesViewModel
                {
                    DataSeries = dataSeries,
                    AntiAliasing = true,
                    StrokeThickness = 2,
                    Stroke = Colors.Red,
                }); ;
            }
        }

        public ObservableCollection<IRenderableSeriesViewModel> RenderableSeriesViewModels { get; set; }
    }
}
