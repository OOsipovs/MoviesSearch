using Microsoft.Extensions.Configuration;
using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesSearch.Infrastructure.Services
{
    public class OmdbClient : IMovieRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OmdbClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDB API key is not configured.");
            _baseUrl = configuration["Omdb:BaseUrl"] ?? throw new InvalidOperationException("OMDB base URL is not configured.");
        }

        public async Task<List<Movie>> SearchMoviesAsync(string title, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            var response = await _httpClient.GetAsync($"{_baseUrl}?s={Uri.EscapeDataString(title)}&apikey={_apiKey}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<OmdbSearchResponse>(content);

            if (json.Response == "False") throw new Exception(json.Error);

            foreach (var movie in json.Search ?? new List<Movie>())
            {
                movie.Id = Guid.NewGuid();
            }

            return json.Search ?? new List<Movie>();
        }

        public async Task<MovieDetails> GetMovieDetailsAsync(string imdbID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(imdbID)) throw new ArgumentException("IMDB ID cannot be null or empty.", nameof(imdbID));

            var response = await _httpClient.GetAsync($"{_baseUrl}?i={imdbID}&apikey={_apiKey}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var details = JsonSerializer.Deserialize<MovieDetails>(content);

            if (details.Response == "False") throw new Exception(details.Error);

            details.Id = Guid.NewGuid();

            return details;
        }

        private class OmdbSearchResponse
        {
            public List<Movie> Search { get; set; }
            public string Response { get; set; }
            public string Error { get; set; }
        }
    }
}