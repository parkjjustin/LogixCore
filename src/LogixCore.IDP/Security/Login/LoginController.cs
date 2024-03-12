using Duende.Bff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixCore.IDP.Security;

[Route("bff")]
[BffApi]
public class LoginController : ControllerBase
{
    private readonly ILoginManager loginManager;
    private readonly IHttpContextAccessor httpContextAccessor;

    public LoginController(ILoginManager loginManager, IHttpContextAccessor httpContextAccessor)
    {
        this.loginManager = loginManager;
        this.httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] UserLoginModel user)
    {
        if (user.Username == "baduser")
        {
            return this.Unauthorized();
        }

        var response = await this.loginManager.Login(user);

        if (!response.IsAuthenticated)
        {
            return this.BadRequest();
        }

        return this.Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        this.loginManager.Logout();
        return this.Ok();
    }
}

public record UserLoginModel
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
