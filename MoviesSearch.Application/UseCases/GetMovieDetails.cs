using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesSearch.Application.UseCases
{
    public class GetMovieDetails
    {
        private readonly IMovieRepository _movieRepository;

        public GetMovieDetails(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<MovieDetails> ExecuteAsync(string imdbID, CancellationToken cancellationToken = default)
        {
            // Stub for authentication: In future, check user auth here
            // if (!IsAuthenticated()) throw new UnauthorizedException();

            return await _movieRepository.GetMovieDetailsAsync(imdbID, cancellationToken);
        }
    }
}
