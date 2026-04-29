using System.Linq.Expressions;

namespace Howestprime.Movies.Application.Contracts.Data;

public static class MovieDataFilters
{
    public static Expression<Func<MovieData, bool>> ByTitleAndGenres(string? title, IEnumerable<GenreData> genres)
    {
        var genreValues = genres.Select(g => g.Value).ToList();

        return (movie) => 
            (string.IsNullOrWhiteSpace(title) || movie.Title.Contains(title)) &&
            (!genreValues.Any() || movie.Genres.Any(mg => genreValues.Contains(mg.Value)));
    }

    public static Expression<Func<MovieData, bool>> ById(Guid id)
    {
        return (movie) => movie.Id == id;
    }
}