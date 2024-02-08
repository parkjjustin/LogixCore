using Microsoft.AspNetCore.Mvc;

namespace LogixCore.Server.Security.Login;

[Route("api")]
public class LoginController : ControllerBase
{
    private readonly ILoginManager loginManager;

    public LoginController(ILoginManager loginManager)
    {
        this.loginManager = loginManager;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] UserLoginModel user)
    {
        // if there was an actual database lookup, I'd rewrite this to be
        // public async Task<ActionResult<LoginResponse>> Login([FromBody] UserLoginModel user, CancellationToken ct)
        if (user.Username == "baduser")
        {
            return this.Unauthorized();
        }

        // var response = await this.loginManager.Login(user, ct);
        var response = loginManager.Login(user);
        return this.Ok(response);
    }
}

public record UserLoginModel
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
