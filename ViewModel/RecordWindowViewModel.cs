using Real_time_Spectrum_Analyzer.Enum;
using Real_time_Spectrum_Analyzer.Service;
using Real_time_Spectrum_Analyzer.Mediator;
using System.Collections.Generic;

namespace Real_time_Spectrum_Analyzer.ViewModel
{
    class RecordWindowViewModel : ViewModelBase, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "RecordWindowViewModel";
            }
        }

        public RecordWindowViewModel()
        {
            // Add available pages.
            PageViewModels.Add(new RecordFileWindowViewModel());
            PageViewModels.Add(new RecordAnalysisWindowViewModel());

            // Set starting page.
            CurrentPageViewModel = PageViewModels[0];
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

        PageRecordView _pageView = PageRecordView.File;

        public PageRecordView PageRecordView
        {
            get { return _pageView; }
            set
            {
                if (_pageView == value)
                    return;

                _pageView = value;
                OnPropertyChanged("PageView");
                OnPropertyChanged("IsFilePage");
                OnPropertyChanged("IsAnalysisPage");

                switch (PageRecordView)
                {
                    case PageRecordView.File:
                        CurrentPageViewModel = PageViewModels[0];
                        break;
                    case PageRecordView.Analysis:
                        CurrentPageViewModel = PageViewModels[1];
                        break;
                }
            }
        }

        public bool IsFilePage
        {
            get { return PageRecordView == PageRecordView.File; }
            set { PageRecordView = value ? PageRecordView.File : PageRecordView; }
        }

        public bool IsAnalysisPage
        {
            get { return PageRecordView == PageRecordView.Analysis; }
            set { PageRecordView = value ? PageRecordView.Analysis : PageRecordView; }
        }

        #endregion
    }
}
