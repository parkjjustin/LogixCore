using LogixCore.Server.Data;
using LogixCore.Server.Middleware.SecurityHeader;
using LogixCore.Server.Security.Login;
using LogixCore.Server.Security.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddAuthentication("logixcore_auth")
    .AddCookie("logixcore_auth", options =>
    {
        options.Cookie.Name = "logixcore_auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents()
        {
            OnRedirectToLogin = (ctx) =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    ctx.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = (ctx) =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    ctx.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
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
app.UseAuthorization();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseAntiforgery();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
