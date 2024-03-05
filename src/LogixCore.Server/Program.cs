using Duende.Bff.Yarp;
using LogixCore.Server.Data;
using LogixCore.Server.Middleware.SecurityHeader;
using LogixCore.Server.Security.Login;
using LogixCore.Server.Security.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddBff().AddRemoteApis().AddServerSideSessions();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

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
        options.ClientId = "interactive.confidential";
        options.ClientSecret = "secret";

        options.ResponseType = "code";
        options.ResponseMode = "query";
        options.UsePkce = true;

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        options.Scope.Add("openid");
        options.Scope.Add("api");

        options.TokenValidationParameters = new()
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services
    .AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.SuppressXFrameOptionsHeader = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ILoginManager, LoginManager>();
builder.Services.AddScoped(typeof(IUserManager<>), typeof(UserManager<>));
builder.Services.AddScoped(typeof(IUserStore<>), typeof(UserStore<>));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseAntiforgery();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
