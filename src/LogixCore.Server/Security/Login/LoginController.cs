﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogixCore.Server.Security;

[Route("api")]
public class LoginController : ControllerBase
{
    private readonly ILoginManager loginManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IAntiforgery antiforgery;

    public LoginController(ILoginManager loginManager, IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery)
    {
        this.loginManager = loginManager;
        this.httpContextAccessor = httpContextAccessor;
        this.antiforgery = antiforgery;
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

    [HttpGet("antiforgery")]
    [Authorize]
    public ActionResult<string> GetAntiforgeryToken()
    {
        var httpContext = this.httpContextAccessor.HttpContext!;
        var token = this.antiforgery.GetAndStoreTokens(httpContext).RequestToken!;
        httpContext.Response.Headers.Append("X-XSRF-TOKEN", token);
        return this.Ok(token);
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
