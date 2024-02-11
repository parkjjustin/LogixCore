using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogixCore.Server.Security.Login;

public interface ILoginManager
{
    public LoginResponse Login(UserLoginModel user);
}

public class LoginManager : ILoginManager
{
    private readonly IConfiguration configuration;

    public LoginManager(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public LoginResponse Login(UserLoginModel user)
    {
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new LoginResponse
        {
            IsAuthenticated = true,
            JwtToken = token,
            RefreshToken = refreshToken
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
}

public class LoginResponse
{
    public bool IsAuthenticated { get; set; } = false;
    public string JwtToken { get; init; } = default!;
    public string RefreshToken { get; internal set; } = default!;
}
