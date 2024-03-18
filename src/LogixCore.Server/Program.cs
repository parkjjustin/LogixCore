using Duende.Bff.Yarp;
using LogixCore.Server.Middleware.IdentityServerApiExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddBff().AddRemoteApis().AddServerSideSessions();
builder.Services.AddDistributedMemoryCache();

// I think this needs to be moved to the Identity Provider server,
// OR I could reference the IDP but I think that might defeat the purpose of having separate servers.
// and the reason why I need to do this is to verify user credentials when logging in
// The current login implementation doesn't work because I have not provided the authentication middleware services for login.
// The registration however does work.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = "logixcore_auth";
        options.Authority = "https://localhost:5001"; // address of identity provider
        options.ClientId = "logixcore-client";
        options.ClientSecret = "secret";

        options.ResponseType = "code";
        //options.ResponseMode = "query";
        //options.UsePkce = true;

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        options.ClaimActions.Remove("aud");
        options.ClaimActions.DeleteClaim("sid");
        options.ClaimActions.DeleteClaim("idp");
        options.Scope.Add("roles");

        options.TokenValidationParameters = new()
        {
            //NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseBff();
app.UseAuthorization();
app.UseSession();

app.MapIdentityServerEndpoint();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();