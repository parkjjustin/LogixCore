using Duende.Bff.Yarp;
using LogixCore.Server.Middleware.IdentityServerApiExtensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddBff().AddRemoteApis().AddServerSideSessions();
builder.Services.AddDistributedMemoryCache();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "logixcore_auth";
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie("logixcore_auth", options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents()
        {
            OnRedirectToLogin = (ctx) =>
            {
                ctx.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = (ctx) =>
            {
                ctx.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        };
    }).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.SignInScheme = "logixcore_auth";
        options.Authority = "https://localhost:5001"; // address of identity provider
        options.ClientId = "logixcore-client";
        options.ClientSecret = "secret";

        options.ResponseType = "code";
        options.ResponseMode = "query";
        options.UsePkce = true;

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("api");

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