using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesSearch.Application.UseCases
{
    public class GetLatestSearches
    {
        private readonly ISearchHistoryRepository _searchHistoryRepository;

        public GetLatestSearches(ISearchHistoryRepository searchHistoryRepository)
        {
            _searchHistoryRepository = searchHistoryRepository;
        }

        public async Task<List<string>> ExecuteAsync(int count)
        {
            // Stub for authentication: In future, filter by user
            return await _searchHistoryRepository.GetLatestQueriesAsync(count);
        }
    }
}
