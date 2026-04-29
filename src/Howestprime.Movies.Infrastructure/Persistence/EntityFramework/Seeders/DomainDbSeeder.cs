using Microsoft.Extensions.Logging;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Seeders;

public class DomainDbSeeder(DomainDbContext context, ILogger<DomainDbSeeder> _logger)
{
    public async Task Seed()
    {
        if (!context.Rooms.Any())
        {
            var rooms = new List<Room>
            {
                Room.Create(EntityId.New<RoomId>(Guid.Parse("019d059e-d220-71db-8a1a-ec7569492999")),"Blue Room", 100),
                Room.Create(EntityId.New<RoomId>(Guid.Parse("019d059e-d220-75fe-b936-0a97cd75216e")),"Yellow Room", 80)
            };

            context.Rooms.AddRange(rooms);
        }
        if (!context.Movies.Any())
        {
            var movies = new List<Movie>
            {
                Movie.Create("The Shawshank Redemption", "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.", ReleaseYear.Create(1994), Duration.Create(142), new List<Genres> { Genres.Create("Drama") }, new List<Actors> { Actors.Create("Tim Robbins"), Actors.Create("Morgan Freeman"), Actors.Create("Bob Gunton") }, AgeRating.Create(18), PosterUrl.Create("https://m.media-amazon.com/images/I/51NiGlapXlL._AC_.jpg")),
                Movie.Create("The Godfather", "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", ReleaseYear.Create(1972), Duration.Create(175), new List<Genres> { Genres.Create("Crime"), Genres.Create("Drama") }, new List<Actors> { Actors.Create("Marlon Brando"), Actors.Create("Al Pacino") }, AgeRating.Create(18), PosterUrl.Create("https://m.media-amazon.com/images/I/51NiGlapXlL._AC_.jpg")),
                Movie.Create("The Dark Knight", "When the menace known as the Joker emerges from his mysterious past, he wreaks havoc and chaos on the people of Gotham. The Dark Knight must accept one of the greatest psychological and physical tests of his ability to fight injustice.", ReleaseYear.Create(2008), Duration.Create(152), new List<Genres> { Genres.Create("Action"), Genres.Create("Crime"), Genres.Create("Drama") }, new List<Actors> { Actors.Create("Christian Bale"), Actors.Create("Heath Ledger") }, AgeRating.Create(16), PosterUrl.Create("https://m.media-amazon.com/images/I/51NiGlapXlL._AC_.jpg")),
                Movie.Create("Pulp Fiction", "The lives of two mob hitmen, a boxer, a gangster's wife, and a pair of diner bandits intertwine in four tales of violence and redemption.", ReleaseYear.Create(1994), Duration.Create(154), new List<Genres> { Genres.Create("Crime"), Genres.Create("Drama") }, new List<Actors> { Actors.Create("John Travolta"), Actors.Create("Uma Thurman"), Actors.Create("Samuel L. Jackson") }, AgeRating.Create(18), PosterUrl.Create("https://m.media-amazon.com/images/I/51NiGlapXlL._AC_.jpg"))
            };

            context.Movies.AddRange(movies);
        }
        await context.SaveChangesAsync();
    }
}
