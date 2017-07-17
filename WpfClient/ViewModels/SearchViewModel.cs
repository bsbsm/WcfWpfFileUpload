using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WcfServiceLibrary.Contracts;

namespace WpfClient.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        public ObservableCollection<FileRowModel> Results { get; set; }

        public SearchViewModel()
        {
            NextPageCommand = new RelayCommand(param => this.GetNextPage(), param => CanSwitchPage);
            PreviousPageCommand = new RelayCommand(param => this.GetPreviousPage(), param => CanSwitchPage);
            FirstPageCommand = new RelayCommand(param => this.GetFirstPage(), param => CanSwitchPage);
            LastPageCommand = new RelayCommand(param => this.GetLastPage(), param => CanSwitchPage);
            FindCommand = new RelayCommand(param => this.Find());
            
            //Тест
            //TotalResultsCount = 319;
            //Results = new ObservableCollection<FileRowModel> { new FileRowModel { RowNumber = 2, RowText = "Тест1" }, new FileRowModel { RowNumber = 1, RowText = "Текст2" }, new FileRowModel { RowNumber = 89, RowText = "Строка3" } };         
        }

        #region Page Commands

        #region Page properties
        private int _desiredPage;

        public int PreviousPage { get; set; }

        private int _currentPage;

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        private int _totalPagesCount;
        public int TotalPagesCount
        {
            get
            {
                return _totalPagesCount;
            }
            set
            {
                if (_totalPagesCount != value)
                {
                    _totalPagesCount = value;
                    OnPropertyChanged("TotalPagesCount");
                }
            }
        }
        #endregion //Page properties

        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand FirstPageCommand { get; set; }
        public ICommand LastPageCommand { get; set; }

        private bool CanSwitchPage
        {
            get
            {
                var canExecute = (Results != null && Results.Any())
                                 && TotalPagesCount > 1;

                return canExecute;
            }
        }
    
        private void GetNextPage()
        {
            PreviousPage = CurrentPage;
            _desiredPage = ++CurrentPage;
        
            SendSearchRequest(_desiredPage);
        }

        private void GetPreviousPage()
        {
            _desiredPage = PreviousPage;
            PreviousPage = CurrentPage;

            SendSearchRequest(_desiredPage);
        }

        private void GetFirstPage()
        {
            _desiredPage = 1;
            PreviousPage = CurrentPage;

            SendSearchRequest(_desiredPage);
        }

        private void GetLastPage()
        {
            _desiredPage = TotalPagesCount;
            PreviousPage = CurrentPage;

            //Results.Add(new FileRowModel { RowNumber = 310, RowText = "Added" });
            //TotalResultsCount = Results.Count;

            SendSearchRequest(_desiredPage);
        }
        #endregion //Page Commands

        #region Find Command

        #region Search properties
        private string _searchText;

        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged("SearchText");
                }
            }
        }

        private int _totalResultsCount;
        public int TotalResultsCount
        {
            get
            {
                return _totalResultsCount;
            }
            set
            {
                if (_totalResultsCount != value)
                {
                    _totalResultsCount = value;
                    OnPropertyChanged("TotalResultsCount");
                }
            }
        }
        #endregion //Search properties

        public ICommand FindCommand { get; set; }

        private void Find()
        {
            SendSearchRequest();
        }

        private SearchRowServiceReference.ISearchRowService channel;

        private async void SendSearchRequest(int page = 1, string query = null)
        {
            string _query;

            if (!String.IsNullOrEmpty(query))
            {
                _query = query;
            }
            else
            {
                if (!String.IsNullOrEmpty(SearchText))
                    _query = SearchText;
                else
                    return;              
            }

            if(channel == null)
            {
                var srClient = new SearchRowServiceReference.SearchRowServiceClient();

                channel = srClient.ChannelFactory.CreateChannel();
            }          

            var searchResult = await channel.SearchRowAsync(_query, page);

            Results = new ObservableCollection<FileRowModel>(searchResult.ResultsOnPage);
            CurrentPage = searchResult.CurrentPage;
            TotalResultsCount = searchResult.ResultsCount;
            TotalPagesCount = searchResult.ResultsCount;          
        }
        #endregion //Find Command
    }
}
