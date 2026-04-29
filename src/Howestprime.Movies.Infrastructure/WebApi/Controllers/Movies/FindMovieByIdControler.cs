
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record FindMovieByIdRequest(
    [FromRoute] Guid Id,
    [FromHeader(Name = "x-user-role")] string UserRole,
    [FromServices] IUseCase<FindMovieByIdInput, MovieData> UseCase
);

public static class FindMovieByIdController
{
    public static async Task<Results<Ok<MovieData>, BadRequest>> Invoke(
        [AsParameters] FindMovieByIdRequest request
    )
    {

        FindMovieByIdInput input = new(request.Id, request.UserRole);
        MovieData? movieData = await request.UseCase.Execute(input);

        if (movieData == null)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(movieData);
    }
}
