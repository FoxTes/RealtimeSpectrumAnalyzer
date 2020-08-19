using Real_time_Spectrum_Analyzer.DataBase;
using Real_time_Spectrum_Analyzer.DataBase.Model;
using Real_time_Spectrum_Analyzer.Enum;
using Real_time_Spectrum_Analyzer.Mediator;
using Real_time_Spectrum_Analyzer.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace Real_time_Spectrum_Analyzer.ViewModel
{
    class HomeWindowViewModel : ViewModelBase
    {
        public HomeWindowViewModel()
        {
            using (UserContext db = new UserContext())
            {
                // Тестовый вариант.
                Protocol protocol = new Protocol { Name = DateTime.Now.ToString() };
                db.Protocols.Add(protocol);
                db.SaveChanges();
            }

            // Регистрируем событие.
            MediatorService.Register("StartRecord", StartRecord);

            // Add available pages.
            PageViewModels.Add(new MainWindowViewModel());
            PageViewModels.Add(new RecordWindowViewModel());
            PageViewModels.Add(new SettingWindowViewModel());

            // Set starting page.
            CurrentPageViewModel = PageViewModels[0];
        }

        public void StartRecord(object obj)
        {
            Status.isRecord = true;
            // Set starting page.
            CurrentPageViewModel = PageViewModels[0];
            PageView = PageView.Main;
        }

        #region Property

        private List<IPageViewModel> _pageViewModels;
        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        private IPageViewModel _currentPageViewModel;
        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        PageView _pageView = PageView.Main;

        public PageView PageView
        {
            get { return _pageView; }
            set
            {
                if (_pageView == value)
                    return;

                _pageView = value;
                OnPropertyChanged("PageView");
                OnPropertyChanged("IsMainPage");
                OnPropertyChanged("IsRecordPage");
                OnPropertyChanged("IsAdditionalPage");

                switch (PageView)
                {
                    case PageView.Main:
                        CurrentPageViewModel = PageViewModels[0];
                        break;
                    case PageView.Record:
                        if (Status.isRecord)
                        {
                            string message = "Вы уверены, что хотите прервать запись?";
                            string title = "Закрыть окно";
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                            DialogResult result = System.Windows.Forms.MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);

                            if (result == DialogResult.Yes)
                            {
                                Status.isRecord = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        CurrentPageViewModel = PageViewModels[1];
                        break;
                    case PageView.Additional:
                        if (Status.isRecord)
                        {
                            string message = "Вы уверены, что хотите прервать запись?";
                            string title = "Закрыть окно";
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                            DialogResult result = System.Windows.Forms.MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);

                            if (result == DialogResult.Yes)
                            {
                                Status.isRecord = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        CurrentPageViewModel = PageViewModels[2];
                        break;
                }
            }
        }

        public bool IsMainPage
        {
            get { return PageView == PageView.Main; }
            set { PageView = value ? PageView.Main : PageView; }
        }

        public bool IsRecordPage
        {
            get { return PageView == PageView.Record; }
            set { PageView = value ? PageView.Record : PageView; }
        }

        public bool IsAdditionalPage
        {
            get { return PageView == PageView.Additional; }
            set { PageView = value ? PageView.Additional : PageView; }
        }

        #endregion

        #region Command

        #endregion
    }
}