using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.Repositories
{
    public interface IFileDatabaseRepository
    {
        bool SaveFile(System.IO.Stream file);
        Contracts.SearchResultDto SearchRow(string query, int page);
    }
}
