using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Xunit;

namespace UnitTests.Application.Movies;

public class SearchMovieCatalogTests
{
    [Fact]
    public async Task Execute_WithValidInput_ShouldReturnMovies_And_AuthorizeUser()
    {
        // Arrange
        var fakeQuery = new FakeSearchMovieCatalogQuery();
        var fakeAuthService = new FakeAuthorizationService();
        var sut = new SearchMovieCatalog(fakeQuery, fakeAuthService);

        var input = new SearchMovieCatalogInput(
            Title: "Inception",
            Genres: new[] { "Action", "Sci-Fi" },
            UserRole: "User"
        );

        // Act
        var result = await sut.Execute(input);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Inception", result[0].Title);

        Assert.True(fakeAuthService.IsAuthorizeCalled);
        Assert.Equal(input.UserRole, fakeAuthService.RoleCalledWith);
        Assert.Equal(nameof(SearchMovieCatalog), fakeAuthService.PermissionCalledWith);

        Assert.True(fakeQuery.IsFetchCalled);
    }

    [Fact]
    public async Task Execute_WithUnauthorizedUserRole_ShouldThrowException()
    {
        // Arrange
        var fakeQuery = new FakeSearchMovieCatalogQuery();
        var fakeAuthService = new FakeAuthorizationService(throwOnAuthorize: true);
        var sut = new SearchMovieCatalog(fakeQuery, fakeAuthService);

        var input = new SearchMovieCatalogInput(
            Title: "Inception",
            Genres: new[] { "Action" },
            UserRole: "UnauthorizedRole"
        );

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(input));
        Assert.True(fakeAuthService.IsAuthorizeCalled);
        Assert.False(fakeQuery.IsFetchCalled);
    }

    private class FakeSearchMovieCatalogQuery : ISearchMovieCatalogQuery
    {
        public bool IsFetchCalled { get; private set; }
        public Expression<Func<MovieData, bool>>? FilterCalledWith { get; private set; }

        public Task<IReadOnlyList<MovieData>> Fetch(Expression<Func<MovieData, bool>> filter)
        {
            IsFetchCalled = true;
            FilterCalledWith = filter;

            var movies = new List<MovieData>
            {
                new MovieData {
                    Id = Guid.NewGuid(),
                    PosterUrl = "https://example.com/inception.jpg",
                    Title = "Inception",
                    Genres = new[] { new GenreData("Action"), new GenreData("Sci-Fi") },
                    Actors = new[] { new ActorData("Leonardo DiCaprio") },
                    AgeRating = 13,
                    ReleaseYear = 2010,
                    Duration = 148,
                    Description = "A thief who steals corporate secrets..."
                }
            };

            return Task.FromResult<IReadOnlyList<MovieData>>(movies);
        }
    }

    private class FakeAuthorizationService : IAuthorizationService
    {
        private readonly bool _throwOnAuthorize;

        public bool IsAuthorizeCalled { get; private set; }
        public string? RoleCalledWith { get; private set; }
        public string? PermissionCalledWith { get; private set; }

        public FakeAuthorizationService(bool throwOnAuthorize = false)
        {
            _throwOnAuthorize = throwOnAuthorize;
        }

        public void Authorize(string role, string permission)
        {
            IsAuthorizeCalled = true;
            RoleCalledWith = role;
            PermissionCalledWith = permission;

            if (_throwOnAuthorize)
            {
                throw new UnauthorizedAccessException($"Role '{role}' is not authorized for '{permission}'.");
            }
        }
    }
}
