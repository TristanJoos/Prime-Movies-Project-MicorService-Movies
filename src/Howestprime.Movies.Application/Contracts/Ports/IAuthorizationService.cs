namespace Howestprime.Movies.Application.Contracts.Ports;

public interface IAuthorizationService
{
    void Authorize(string role, string permission);
}