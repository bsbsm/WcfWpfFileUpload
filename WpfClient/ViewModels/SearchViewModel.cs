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
        public int PreviousPage { get; set; }

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

        private int _desiredPage;

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if(_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged("SearchQuery");
                }
            }
        }

        private string _searchQueryBuffer;

        public ICommand NextPageCommand;
        public ICommand PreviousPageCommand;
        public ICommand FindCommand;

        public SearchViewModel()
        {
            Results = new ObservableCollection<FileRowModel> { new FileRowModel { RowNumber = 2, RowText = "gecn" }, new FileRowModel { RowNumber = 1, RowText = "gecn" } , new FileRowModel { RowNumber = 89, RowText = "gecn" } };
            NextPageCommand = new RelayCommand(param => this.GetNextPage());
            PreviousPageCommand = new RelayCommand(param => this.GetPreviousPage());
            FindCommand = new RelayCommand(param => this.Find());
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

        private void Find()
        {
            _searchQueryBuffer = SearchText;

            SendSearchRequest(_desiredPage, SearchText);
        }

        private async void SendSearchRequest(int page, string query = null)
        {
            string _query;

            if (!String.IsNullOrEmpty(query))
            {
                _query = query;
            }
            else
            {
                if (!String.IsNullOrEmpty(_searchQueryBuffer))
                    _query = _searchQueryBuffer;
                else
                    return;              
            }

            var srClient = new SearchRowServiceReference.SearchRowServiceClient();
            var chanel = srClient.ChannelFactory.CreateChannel();

            var searchResult = await chanel.SearchRowAsync(_query, page);

            Results = new ObservableCollection<FileRowModel>(searchResult.ResultsOnPage);
            CurrentPage = searchResult.CorrentPage;
            TotalResultsCount = searchResult.ResultsCount;
            TotalPagesCount = searchResult.ResultsCount;
        }
    }
}
