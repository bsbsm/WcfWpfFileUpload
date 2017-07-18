using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary.Contracts;
using WcfServiceLibrary.Repositories;

namespace WcfServiceLibrary.Services
{
    public class SearchRowService : ISearchRowService
    {
        readonly IFileDatabaseRepository repo;
        public SearchRowService()
        {
            repo = new FileDatabaseRepository();
        }
        public SearchRowService(IFileDatabaseRepository repository)
        {
            repo = repository;
        }

        public SearchResultDto SearchRow(string query, int page)
        {
            if(!String.IsNullOrWhiteSpace(query) && page > 0)
            {
                return repo.SearchRow(query, page);
            }
            else
            {
                throw new ArgumentNullException("null arguments");            
            }
        }
    }
}
