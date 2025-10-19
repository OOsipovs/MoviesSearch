using Microsoft.Extensions.Configuration;
using MoviesSearch.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesSearch.Infrastructure.Repositories
{
    public class InMemorySearchHistoryRepository : ISearchHistoryRepository
    {
        private readonly List<string> _queries = new List<string>();
        private readonly int _maxHistory;

        public InMemorySearchHistoryRepository(IConfiguration configuration)
        {
            _maxHistory = configuration.GetValue<int>("SearchHistory:MaxLimit", 5);
        }

        public Task AddSearchQueryAsync(string query)
        {
            _queries.Add(query);
            if (_queries.Count > _maxHistory)
            {
                _queries.RemoveAt(0);
            }
            return Task.CompletedTask;
        }

        public Task<List<string>> GetLatestQueriesAsync(int count)
        {
            return Task.FromResult(_queries.TakeLast(count).ToList());
        }
    }
}