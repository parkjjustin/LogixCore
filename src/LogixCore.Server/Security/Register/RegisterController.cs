using LogixCore.Server.Security.Users;
using Microsoft.AspNetCore.Mvc;

namespace LogixCore.Server.Security.Register;

[Route("api")]
public class RegisterController : ControllerBase
{
    private readonly IUserManager<RegisterUserModel> userManager;

    public RegisterController(IUserManager<RegisterUserModel> userManager)
    {
        this.userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody] RegisterUserModel registerUserModel, CancellationToken ct)
    {
        try
        {
            await this.userManager.RegisterUserAsync(registerUserModel, ct);

        } catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return this.Ok();
    }
}
