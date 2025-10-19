using MoviesSearch.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesSearch.Core.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> SearchMoviesAsync(string title, CancellationToken cancellationToken = default);
        Task<MovieDetails> GetMovieDetailsAsync(string imdbId, CancellationToken cancellationToken = default);
    }
}
