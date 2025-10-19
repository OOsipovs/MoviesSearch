using FluentAssertions;
using Moq;
using MoviesSearch.Application.UseCases;
using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MoviesSearch.Tests.Application
{
    public class SearchMoviesTests
    {
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<ISearchHistoryRepository> _searchHistoryRepositoryMock;
        private readonly SearchMovies _useCase;

        public SearchMoviesTests()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _searchHistoryRepositoryMock = new Mock<ISearchHistoryRepository>();
            _useCase = new SearchMovies(_movieRepositoryMock.Object, _searchHistoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Searchingmovies_ValidTitle_ReturnsMoviesAndAddsToHistory()
        {

            var movies = new List<Movie> { new Movie { ImdbID = "tt0133093", Title = "The Matrix", Year = "1999" } };
            _movieRepositoryMock.Setup(r => r.SearchMoviesAsync("Matrix", It.IsAny<CancellationToken>()))
                .ReturnsAsync(movies);
            _searchHistoryRepositoryMock.Setup(r => r.AddSearchQueryAsync("Matrix"))
                .Returns(Task.CompletedTask);

            var result = await _useCase.ExecuteAsync("Matrix");

            result.Should().BeEquivalentTo(movies);
            _searchHistoryRepositoryMock.Verify(r => r.AddSearchQueryAsync("Matrix") , Times.Once());
        }


        [Fact]
        public async Task SearchingMovies_Cancelled_OperationCanceledException()
        {
            var cts = new CancellationTokenSource();
            _movieRepositoryMock.Setup(r => r.SearchMoviesAsync("Matrix", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => _useCase.ExecuteAsync("Matrix", cts.Token));
        }
    }
}