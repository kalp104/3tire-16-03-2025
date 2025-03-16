using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Core;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PizzaShop.Core.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserTableService, UserTableService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// Filters
builder.Services.AddScoped<AuthorizePermissionUserTable>();
builder.Services.AddScoped<AuthorizePermissionRoles>();
builder.Services.AddScoped<AuthorizePermissionMenu>();

// Connection string + dependency injection
builder.Services.AddDbContext<PizzaShopContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Add HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

// Configure JWT Authentication (optional, if you want to use built-in middleware alongside TokenMiddleware)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Privacy"); // General error page for unhandled exceptions
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Custom TokenMiddleware for token validation from cookies
app.UseTokenMiddleware();

// Handle 404s by redirecting to /Home/Privacy
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Home/Privacy");
    }
    await Task.CompletedTask;
});

// Use built-in authentication and authorization (if configured)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//      DependencyInjection.RegisterServices(builder.Services, connectionString!);

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//         };
//         options.Events = new JwtBearerEvents
//         {
//             OnMessageReceived = context =>
//             {
//                 if (context.Request.Cookies.ContainsKey("auth_token"))
//                 {
//                     context.Token = context.Request.Cookies["auth_token"];
//                     // Access HttpContext.Items instead of context.Items
//                     var user = context.HttpContext.User;
//                     context.HttpContext.Items["UserRole"] = user.FindFirst(ClaimTypes.Role)?.Value;
//                     context.HttpContext.Items["UserEmail"] = user.FindFirst(ClaimTypes.Email)?.Value;
//                 }
//                 return Task.CompletedTask;
//             },
//             OnChallenge = context =>
//             {
//                 context.HandleResponse();
//                 context.Response.Redirect("/Home/Index");
//                 return Task.CompletedTask;
//             }
//         };
//     });


/*****************************middleware : will change its position*********************************/
// app.Use(async (context, next) =>
// {

//     if (context.User.Identity != null && !context.User.Identity.IsAuthenticated && context.Request.Cookies.ContainsKey("auth_token"))
//     {
//         var token = context.Request.Cookies["auth_token"];

//         try
//         {
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
//             var parameters = new TokenValidationParameters
//             {
//                 ValidateIssuerSigningKey = true,
//                 IssuerSigningKey = new SymmetricSecurityKey(key),
//                 ValidateIssuer = true,
//                 ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                 ValidateAudience = true,
//                 ValidAudience = builder.Configuration["Jwt:Audience"],
//                 ValidateLifetime = true,
//                 ClockSkew = TimeSpan.Zero
//             };

//             var principal = tokenHandler.ValidateToken(token, parameters, out _);
//             context.User = principal;

//             // if (context.User.Identity != null &&context.User.Identity.IsAuthenticated)
//             // {
//             //     // Console.WriteLine("User successfully authenticated from auth_token cookie.");
//             // }
//             var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
//             var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
//             if (!string.IsNullOrEmpty(emailClaim) && !string.IsNullOrEmpty(roleClaim))
//             {
//                 context.Items["UserEmail"] = emailClaim;

//                 context.Items["UserRole"] = roleClaim;


//             }
//             else
//             {
//                 Console.WriteLine("email not found");
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Token validation failed: {ex.Message}");
//             context.Response.Redirect("/Home/Index");
//             return;
//         }

//         if (context.User.Identity != null && !context.User.Identity.IsAuthenticated)
//         {
//             // Console.WriteLine("inside the Account autologin"); 
//             context.Response.Redirect("/Home/Index");
//             return;
//         }
//     }
//     await next();

// }); 


/**************************************************************/