using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesSearch.Application.UseCases
{
    public class SearchMovies
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ISearchHistoryRepository _searchHistoryRepository;

        public SearchMovies(IMovieRepository movieRepository, ISearchHistoryRepository searchHistoryRepository)
        {
            _movieRepository = movieRepository;
            _searchHistoryRepository = searchHistoryRepository;
        }

        public async Task<List<Movie>> ExecuteAsync(string title, CancellationToken cancellationToken = default)
        {
            // Stub for authentication: In future, check user auth here
            // if (!IsAuthenticated()) throw new UnauthorizedException();

            var results = await _movieRepository.SearchMoviesAsync(title, cancellationToken);
            await _searchHistoryRepository.AddSearchQueryAsync(title);
            return results;
        }
    }
}
