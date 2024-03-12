namespace LogixCore.Server.Middleware.IdentityServerApiExtensions;

public static class IdentityServerApiEndpointExtensions
{
    public static void MapIdentityServerEndpoint(this WebApplication app)
    {
        app.MapRemoteBffApiEndpoint("/api/register", "https://localhost:5001/api/register").SkipAntiforgery();
        app.MapRemoteBffApiEndpoint("/api/login", "https://localhost:5001/api/login").SkipAntiforgery();
    }
}
