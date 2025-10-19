using FluentAssertions;
using Moq;
using MoviesSearch.Application.UseCases;
using MoviesSearch.Core.Entities;
using MoviesSearch.Core.Interfaces;


namespace MoviesSearch.Tests
{
    public class GetMovieDetailsTests
    {
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly GetMovieDetails _useCase;

        public GetMovieDetailsTests()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _useCase = new GetMovieDetails(_movieRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ValidImdbId_ReturnsMovieDetails()
        {
            // Arrange
            var randomGuid = Guid.NewGuid();
            var expectedDetails = new MovieDetails
            {
                Id = randomGuid,
                Title = "The Matrix",
                Plot = "A computer hacker learns about the true nature of reality.",
                ImdbRating = "8.7",
                Director = "Wachowski Sisters",
                Actors = "Keanu Reeves, Laurence Fishburne",
                Poster = "http://example.com/matrix.jpg",
                Response = "True"
            };
            _movieRepositoryMock.Setup(r => r.GetMovieDetailsAsync("tt0133093", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDetails);

            // Act
            var result = await _useCase.ExecuteAsync("tt0133093", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(randomGuid);
            result.Id.Should().NotBe(Guid.Empty);
            result.Title.Should().Be("The Matrix");
            result.Plot.Should().Be("A computer hacker learns about the true nature of reality.");
            result.ImdbRating.Should().Be("8.7");
            result.Director.Should().Be("Wachowski Sisters");
            result.Actors.Should().Be("Keanu Reeves, Laurence Fishburne");
            result.Poster.Should().Be("http://example.com/matrix.jpg");
            result.Response.Should().Be("True");
            _movieRepositoryMock.Verify(r => r.GetMovieDetailsAsync("tt0133093", It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task ExecuteAsync_InvalidImdbId_ThrowsException()
        {
            // Arrange
            _movieRepositoryMock.Setup(r => r.GetMovieDetailsAsync("invalid", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Invalid ID"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _useCase.ExecuteAsync("invalid", CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_Canceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            _movieRepositoryMock.Setup(r => r.GetMovieDetailsAsync("tt0133093", It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _useCase.ExecuteAsync("tt0133093", cts.Token));
        }
    }
}
