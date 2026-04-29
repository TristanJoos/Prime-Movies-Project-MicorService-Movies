using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Movies;
using Xunit;

namespace UnitTests.Application.Movies;

public class MovieDataFiltersTests
{
    [Fact]
    public void ByTitleAndGenres_WithTitleAndGenres_FilterCorrectly()
    {
        // Arrange
        var filter = MovieDataFilters.ByTitleAndGenres("Inception", new[] { new GenreData("Action") });
        var compiledFilter = filter.Compile();

        var matchingMovie = new MovieData(Guid.NewGuid(), "", "Inception", new[] { new GenreData("Action") }, Array.Empty<ActorData>(), 13, 2010, 148, "");
        var nonMatchingMovieTitle = new MovieData(Guid.NewGuid(), "", "Interstellar", new[] { new GenreData("Action") }, Array.Empty<ActorData>(), 13, 2010, 148, "");
        var nonMatchingMovieGenre = new MovieData(Guid.NewGuid(), "", "Inception", new[] { new GenreData("Comedy") }, Array.Empty<ActorData>(), 13, 2010, 148, "");

        // Act & Assert
        Assert.True(compiledFilter(matchingMovie));
        Assert.False(compiledFilter(nonMatchingMovieTitle));
        Assert.False(compiledFilter(nonMatchingMovieGenre));
    }

    [Fact]
    public void ByTitleAndGenres_WithEmptyTitleAndEmptyGenres_AllowsAll()
    {
        // Arrange
        var filter = MovieDataFilters.ByTitleAndGenres("", Array.Empty<GenreData>());
        var compiledFilter = filter.Compile();

        var movie = new MovieData(Guid.NewGuid(), "", "Any Title", new[] { new GenreData("Comedy") }, Array.Empty<ActorData>(), 13, 2010, 148, "");

        // Act & Assert
        Assert.True(compiledFilter(movie));
    }

    [Fact]
    public void AuthorizationOptions_Defaults_AreSet()
    {
        var options = new AuthorizationOptions();
        Assert.Empty(options.PermissionsByRole);

        var rolePerm = new RolePermission();
        Assert.Equal(string.Empty, rolePerm.Name);
        Assert.Empty(rolePerm.Permissions);

        rolePerm.Name = "User";
        rolePerm.Permissions = new List<string> { "Read" };
        options.PermissionsByRole.Add(rolePerm);

        Assert.Single(options.PermissionsByRole);
        Assert.Equal("User", options.PermissionsByRole[0].Name);
        Assert.Single(options.PermissionsByRole[0].Permissions);
    }

    [Fact]
    public void MovieData_DefaultConstructor_SetsDefaults()
    {
        var movieData = new MovieData();
        Assert.Equal(Guid.Empty, movieData.Id);
        Assert.Equal(string.Empty, movieData.Title);
        Assert.Empty(movieData.Genres);
        Assert.Empty(movieData.Actors);
        Assert.Equal(0, movieData.AgeRating);
        Assert.Equal(0, movieData.ReleaseYear);
        Assert.Equal(0, movieData.Duration);
        Assert.Equal(string.Empty, movieData.Description);
        Assert.Equal(string.Empty, movieData.PosterUrl);

        var movieData2 = new MovieData();
        Assert.False(movieData.Equals(null));
        Assert.NotNull(movieData.ToString());

        var actor1 = new ActorData("Leo");
        var actor2 = new ActorData("Leo");
        Assert.True(actor1.Equals(actor2));
        Assert.Equal(actor1.GetHashCode(), actor2.GetHashCode());
        Assert.NotNull(actor1.ToString());

        var genre1 = new GenreData("Action");
        var genre2 = new GenreData("Action");
        Assert.True(genre1.Equals(genre2));
        Assert.Equal(genre1.GetHashCode(), genre2.GetHashCode());
        Assert.NotNull(genre1.ToString());

        var searchInput1 = new SearchMovieCatalogInput("Title", new[] { "Action" }, "User");
        Assert.False(searchInput1.Equals(null));
        Assert.NotNull(searchInput1.ToString());
    }
}
