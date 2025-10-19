using FluentAssertions;
using Moq;
using MoviesSearch.Application.UseCases;
using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MoviesSearch.Tests.Application
{
    public class LatestSearchesTests
    {
        private readonly Mock<ISearchHistoryRepository> _searchHistoryRepositoryMock;
        private readonly GetLatestSearches _useCase;

        public LatestSearchesTests()
        {
            _searchHistoryRepositoryMock = new Mock<ISearchHistoryRepository>();
            _useCase = new GetLatestSearches(_searchHistoryRepositoryMock.Object);
        }

        [Fact]
        public async Task GetLatestQueries_NoSearches_ReturnsEmptyList()
        {

            _searchHistoryRepositoryMock.Setup(r => r.GetLatestQueriesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());
            var result = await _useCase.ExecuteAsync(5);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLatestQueriesAsync_HasSearches_ReturnsRecentSearches()
        {
            var searches = new List<string> { "Matrix", "Inception", "Avatar" };
            _searchHistoryRepositoryMock.Setup(r => r.GetLatestQueriesAsync(3))
                .ReturnsAsync(searches);
            var result = await _useCase.ExecuteAsync(3);
            result.Should().BeEquivalentTo(searches);
            _searchHistoryRepositoryMock.Verify(r => r.GetLatestQueriesAsync(3), Times.Once());
        }

        [Fact]
        public async Task GetLatestQueries_Zero_ReturnsEmptyList()
        {
            _searchHistoryRepositoryMock.Setup(r => r.GetLatestQueriesAsync(0))
                .ReturnsAsync(new List<string>());
            var result = await _useCase.ExecuteAsync(0);
            result.Should().BeEmpty();
        }

    }
    
}