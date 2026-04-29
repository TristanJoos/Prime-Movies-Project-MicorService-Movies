using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Xunit;

namespace UnitTests.Application.Movies;

public class FindMovieByIdTests
{
    [Fact]
    public async Task Execute_WithValidInput_ShouldReturnMovie_And_AuthorizeUser()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var fakeQuery = new FakeSearchMovieCatalogQuery(movieId);
        var fakeAuthService = new FakeAuthorizationService();
        var sut = new FindMovieById(fakeQuery, fakeAuthService);

        var input = new FindMovieByIdInput(
            MovieId: movieId,
            UserRole: "User"
        );

        // Act
        var result = await sut.Execute(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Inception", result.Title);
        Assert.Equal(movieId, result.Id);

        Assert.True(fakeAuthService.IsAuthorizeCalled);
        Assert.Equal(input.UserRole, fakeAuthService.RoleCalledWith);
        Assert.Equal(nameof(FindMovieById), fakeAuthService.PermissionCalledWith);

        Assert.True(fakeQuery.IsFetchCalled);

        // test basic record stuff for 100% coverage
        var input2 = new FindMovieByIdInput(movieId, "User");
        Assert.True(input.Equals(input2));
        Assert.Equal(input.GetHashCode(), input2.GetHashCode());
        Assert.NotNull(input.ToString());
    }

    [Fact]
    public async Task Execute_WithUnauthorizedUserRole_ShouldThrowException()
    {
        // Arrange
        var fakeQuery = new FakeSearchMovieCatalogQuery();
        var fakeAuthService = new FakeAuthorizationService(throwOnAuthorize: true);
        var sut = new FindMovieById(fakeQuery, fakeAuthService);

        var input = new FindMovieByIdInput(
            MovieId: Guid.NewGuid(),
            UserRole: "UnauthorizedRole"
        );

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(input));
        Assert.True(fakeAuthService.IsAuthorizeCalled);
        Assert.False(fakeQuery.IsFetchCalled);
    }

    [Fact]
    public async Task Execute_WhenMovieNotFound_ShouldThrowException()
    {
        // Arrange
        var fakeQuery = new FakeSearchMovieCatalogQuery(returnEmpty: true);
        var fakeAuthService = new FakeAuthorizationService();
        var sut = new FindMovieById(fakeQuery, fakeAuthService);

        var input = new FindMovieByIdInput(
            MovieId: Guid.NewGuid(),
            UserRole: "User"
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => sut.Execute(input));
        Assert.Equal("Movie not found", ex.Message);
        Assert.True(fakeQuery.IsFetchCalled);
    }

    private class FakeSearchMovieCatalogQuery : ISearchMovieCatalogQuery
    {
        private readonly bool _returnEmpty;
        private readonly Guid _expectedId;

        public FakeSearchMovieCatalogQuery(Guid expectedId = default, bool returnEmpty = false)
        {
            _expectedId = expectedId == default ? Guid.NewGuid() : expectedId;
            _returnEmpty = returnEmpty;
        }

        public bool IsFetchCalled { get; private set; }
        public Expression<Func<MovieData, bool>>? FilterCalledWith { get; private set; }

        public Task<IReadOnlyList<MovieData>> Fetch(Expression<Func<MovieData, bool>> filter)
        {
            IsFetchCalled = true;
            FilterCalledWith = filter;

            if (_returnEmpty)
            {
                return Task.FromResult<IReadOnlyList<MovieData>>(new List<MovieData>());
            }

            var movies = new List<MovieData>
            {
                new MovieData(
                    _expectedId,
                    "https://example.com/inception.jpg",
                    "Inception",
                    new[] { new GenreData("Action"), new GenreData("Sci-Fi") },
                    new[] { new ActorData("Leonardo DiCaprio") },
                    13,
                    2010,
                    148,
                    "A thief who steals corporate secrets..."
                )
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