using Real_time_Spectrum_Analyzer.Functional;
using Real_time_Spectrum_Analyzer.ViewModel;
using SciChart.Examples.ExternalDependencies.Controls.ExceptionView;
using System.Windows;

namespace Real_time_Spectrum_Analyzer
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.OnStartup(e);

            if (FftiDeviceApi.OpenPortToDevice())
            {
                Service.Status.isConnectFTDI = true;
            }
            else
            {
                string message = "Приложение не смогло установить связь с устройством FTDI.\nПродолжить работу без подключения?";
                string title = "Установка соединения";
                System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.YesNo;
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show(message, title, buttons, System.Windows.Forms.MessageBoxIcon.Warning);

                if (result == System.Windows.Forms.DialogResult.No)
                {
                    Current.Shutdown();
                }
            }


            HomeWindow app = new HomeWindow();
            HomeWindowViewModel context = new HomeWindowViewModel();
            app.DataContext = context;
            app.Show();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionView = new ExceptionView(e.Exception)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            exceptionView.ShowDialog();

            e.Handled = true;
        }
    }
}
