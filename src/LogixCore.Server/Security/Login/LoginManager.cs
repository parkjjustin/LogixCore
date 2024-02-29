﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogixCore.Server.Security.Login;

public interface ILoginManager
{
    public Task<LoginResponse> Login(UserLoginModel user);
    public Task Logout();
}

public class LoginManager : ILoginManager
{
    private const string Cookie = "logixcore_auth";
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IAntiforgery antiforgery;

    public LoginManager(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery)
    {
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
        this.antiforgery = antiforgery;
    }

    public async Task<LoginResponse> Login(UserLoginModel user)
    {

        var authenticate = await GenerateScheme(user);

        return new LoginResponse
        {
            IsAuthenticated = authenticate,
        };
    }

    public async Task<bool> GenerateScheme(UserLoginModel user)
    {
        var httpContext = this.httpContextAccessor.HttpContext!;

        // Fix this issue
        if (String.IsNullOrEmpty(httpContext.Request.Headers.Origin.FirstOrDefault()) && String.IsNullOrEmpty(httpContext.Request.Headers.Referer.FirstOrDefault()))
        {
            return false;
        }

        if (!httpContext.Request.Headers.Origin.FirstOrDefault()!.StartsWith("https://localhost:5173") || !httpContext.Request.Headers.Referer.FirstOrDefault()!.StartsWith("https://localhost:5173"))
        {
            return false;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var identity = new ClaimsIdentity(claims, Cookie);

        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false
        };

        await httpContext.SignInAsync(Cookie, claimsPrincipal, authProperties);
        return true;
    }

    public async Task Logout()
    {
        var httpContext = this.httpContextAccessor.HttpContext!;
        await httpContext.SignOutAsync(Cookie);
    }
}

public class LoginResponse
{
    public bool IsAuthenticated { get; set; } = false;
}
