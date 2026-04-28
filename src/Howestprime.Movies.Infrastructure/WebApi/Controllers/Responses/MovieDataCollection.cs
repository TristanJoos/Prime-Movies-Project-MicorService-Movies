using Howestprime.Movies.Application.Contracts.Data;
namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Responses;

public sealed record MovieDataCollection(
    IReadOnlyList<MovieData> Data
);
