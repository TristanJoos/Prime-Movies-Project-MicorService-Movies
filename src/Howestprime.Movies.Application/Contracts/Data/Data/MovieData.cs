namespace Howestprime.Movies.Application.Contracts.Data;


/*Id (uuid)	Unique identifier
PosterUrl (string)	URL to the movie poster
Title (string)	Title of the movie
Genres (List<GenreData>)	List of genres (e.g., “Action, Adventure, Sci-Fi”)
Actors (List<ActorData>)	List of actors (e.g., “Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss”)
AgeRating (int)	Age rating of the movie
ReleaseYear (int)	Release year
Duration (int)	Duration in minutes
Description (string)	Description of the movie*/
public record MovieData(
    Guid id,
    string PosterUrl,
    string Title,
    IEnumerable<GenreData> Genres
);