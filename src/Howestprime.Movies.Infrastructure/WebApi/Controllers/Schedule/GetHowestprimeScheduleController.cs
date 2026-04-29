
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Movies;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record GetHowestprimeScheduleRequest(
    [FromRoute] Guid Id,
    [FromHeader(Name = "x-user-role")] string UserRole,
    [FromServices] IUseCase<GetHowestprimeScheduleInput, IEnumerable<MovieEventData>> UseCase
);

public static class GetHowestprimeScheduleController
{
    public static async Task<Results<Ok<IEnumerable<MovieEventData>>, BadRequest>> Invoke(
        [AsParameters] GetHowestprimeScheduleRequest request
    )
    {

        GetHowestprimeScheduleInput input = new(request.Id, request.UserRole);
        IEnumerable<MovieEventData>? movieEvents = await request.UseCase.Execute(input);

        if (movieEvents == null)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(movieEvents);
    }
}
