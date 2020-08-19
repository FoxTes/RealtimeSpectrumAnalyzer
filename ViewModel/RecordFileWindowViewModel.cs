using Real_time_Spectrum_Analyzer.DataBase;
using Real_time_Spectrum_Analyzer.DataBase.Model;
using Real_time_Spectrum_Analyzer.Mediator;
using Real_time_Spectrum_Analyzer.Model;
using Real_time_Spectrum_Analyzer.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Real_time_Spectrum_Analyzer.ViewModel
{
    class RecordFileWindowViewModel : ViewModelBase, IPageViewModel
    {

        #region Propery

        public string Name
        {
            get
            {
                return "RecordFileWindowViewMode";
            }
        }

        private ObservableCollection<RecordFileWindowModel> _dataListBox;
        public ObservableCollection<RecordFileWindowModel> DataListBox
        {
            get { return _dataListBox ?? (_dataListBox = new ObservableCollection<RecordFileWindowModel>()); }
        }

        #endregion

        #region Command

        private RelayCommand _windowLoaded;
        public RelayCommand WindowLoaded
        {
            get
            {
                return _windowLoaded ??
                  (_windowLoaded = new RelayCommand(obj =>
                  {
                      using (UserContext db = new UserContext())
                      {
                          // Получаем протоколы из БД.
                          List<Protocol> protocols = db.Protocols.ToList();

                          DataListBox.Clear();
                          foreach (Protocol pt in protocols)
                          {
                              DataListBox.Add(new RecordFileWindowModel { ForegroundColor = Brushes.Gainsboro, Value = $"Протокол: {pt}" });
                          }
                      }
                  }));
            }
        }

        private RelayCommand _startRecord;
        public RelayCommand StartRecord
        {
            get
            {
                return _startRecord ??
                  (_startRecord = new RelayCommand(obj =>
                  {
                      MediatorService.NotifyColleagues("StartRecord", true);
                  }));
            }
        }


        private RelayCommand _clearAllDataBase;
        public RelayCommand ClearAllDataBase
        {
            get
            {
                return _clearAllDataBase ??
                  (_clearAllDataBase = new RelayCommand(obj =>
                  {
                      using (UserContext db = new UserContext())
                      {
                          //db.Database.ExecuteSqlCommand("ALTER TABLE dbo.Records ADD CONSTRAINT Records_Protocols FOREIGN KEY (ProtocolId) REFERENCES dbo.Protocols (Id) ON DELETE SET NULL");

                          Protocol protocolDelete = db.Protocols.FirstOrDefault();
                          if (protocolDelete != null)
                          {
                              db.Protocols.Remove(protocolDelete);
                              db.SaveChanges();
                          }

                          DataListBox.Clear();
                      }
                  }));
            }
        }

        #endregion
    }
}
