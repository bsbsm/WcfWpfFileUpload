using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.Contracts
{
    [ServiceContract]
    public interface ISearchRowService
    {
        [OperationContract]
        SearchResultDto SearchRow(string query, int page);
    }

    [DataContract]
    public class SearchResultDto
    {
        [DataMember]
        public int ResultsCount { get; set; }

        [DataMember]
        public IEnumerable<FileRowModel> ResultsOnPage { get; set; }

        [DataMember]
        public int CurrentPage { get; set; }

        [DataMember]
        public int PagesCount { get; set; }
    }

    [DataContract]
    public class FileRowModel
    {
        [DataMember]
        public int RowNumber { get; set; }

        [DataMember]
        public string RowText { get; set; }
    }
}
