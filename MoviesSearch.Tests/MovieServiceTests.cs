using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using MoviesSearch.Core.Entities;
using MoviesSearch.Infrastructure.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MoviesSearch.Tests.Infrastructure
{
    public class MovieSearchTests
    {
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly OmdbClient _omdbClient;

        public MovieSearchTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Omdb:ApiKey", "test-api-key" },
                    { "Omdb:BaseUrl", "http://www.omdbapi.com/" }
                })
                .Build();
            _configuration = config;
            _omdbClient = new OmdbClient(_httpClient, _configuration);
        }

        [Fact]
        public async Task SearchingMoviesAsync_CorrectTitle_ReturnsMovies()
        {
            var responseContent = "{\"Search\":[{\"imdbID\":\"tt0133093\",\"Title\":\"The Matrix\",\"Year\":\"1999\"}],\"Response\":\"True\"}";
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("s=Matrix")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var movies = await _omdbClient.SearchMoviesAsync("Matrix");

            movies.Should().HaveCount(1);
            movies[0].ImdbID.Should().Be("tt0133093");
            movies[0].Title.Should().Be("The Matrix");
            movies[0].Year.Should().Be("1999");
            movies[0].Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task SearchingMoviesAsync_NoTitle_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _omdbClient.SearchMoviesAsync(""));
        }

        [Fact]
        public async Task SearchingMovies_ApiError_ThrowsException()
        {

            var responseContent = "{\"Response\":\"False\",\"Error\":\"Movie not found!\"}";
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            await Assert.ThrowsAsync<Exception>(() => _omdbClient.SearchMoviesAsync("Invalid"));
        }

        [Fact]
        public async Task GettingMovieDetails_ValidImdbId_ReturnsDetails()
        {
            var responseContent = "{\"Title\":\"The Matrix\",\"Plot\":\"A computer hacker learns about the true nature of reality.\",\"imdbRating\":\"8.7\",\"Director\":\"Wachowski Sisters\",\"Actors\":\"Keanu Reeves, Laurence Fishburne\",\"Poster\":\"http://example.com/matrix.jpg\",\"Response\":\"True\"}";
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("i=tt0133093")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var details = await _omdbClient.GetMovieDetailsAsync("tt0133093");

            details.Title.Should().Be("The Matrix");
            details.Plot.Should().Be("A computer hacker learns about the true nature of reality.");
            details.ImdbRating.Should().Be("8.7");
            details.Director.Should().Be("Wachowski Sisters");
            details.Actors.Should().Be("Keanu Reeves, Laurence Fishburne");
            details.Poster.Should().Be("http://example.com/matrix.jpg");
            details.Id.Should().NotBeEmpty();
            details.Response.Should().Be("True");
        }

        [Fact]
        public async Task GettingMovieDetails_NoImdbId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _omdbClient.GetMovieDetailsAsync(""));
        }

        [Fact]
        public async Task GettingMovieDetails_ApiError_ThrowsException()
        {
            var responseContent = "{\"Response\":\"False\",\"Error\":\"Invalid ID\"}";
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            await Assert.ThrowsAsync<Exception>(() => _omdbClient.GetMovieDetailsAsync("invalid"));
        }
    }
}