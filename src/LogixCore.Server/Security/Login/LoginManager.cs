using Microsoft.AspNetCore.Antiforgery;
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
        //var token = GenerateJwtToken(user);
        //var refreshToken = GenerateRefreshToken();
        return new LoginResponse
        {
            IsAuthenticated = authenticate,
            //JwtToken = token,
            //RefreshToken = refreshToken
        };
    }

    private string GenerateJwtToken(UserLoginModel user)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes
        (configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(5), //short lifespan
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        return jwtToken;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> GenerateScheme(UserLoginModel user)
    {
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
        var httpContext = this.httpContextAccessor.HttpContext!;

        if (!httpContext.Request.Headers.Origin.First()!.StartsWith("https://localhost:5173") || !httpContext.Request.Headers.Referer.First()!.StartsWith("https://localhost:5173/"))
        {
            return false;
        }

        httpContext.Response.Headers.XFrameOptions = "DENY";
        await httpContext.SignInAsync(Cookie, claimsPrincipal, authProperties);
        return true;
    }

    public async Task Logout()
    {
        var httpContext = this.httpContextAccessor.HttpContext!;
        await httpContext.SignOutAsync();
        httpContext.Session.Clear();
        httpContext.Response.Cookies.Delete(Cookie);
    }
}

public class LoginResponse
{
    public bool IsAuthenticated { get; set; } = false;
    public string Token { get; init; } = default!;
    //public string JwtToken { get; init; } = default!;
    //public string RefreshToken { get; internal set; } = default!;
}
