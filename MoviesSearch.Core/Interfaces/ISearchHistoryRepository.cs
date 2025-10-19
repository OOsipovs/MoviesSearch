using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesSearch.Core.Interfaces
{
    public interface ISearchHistoryRepository
    {
        Task AddSearchQueryAsync(string query);
        Task<List<string>> GetLatestQueriesAsync(int count);
    }
}
