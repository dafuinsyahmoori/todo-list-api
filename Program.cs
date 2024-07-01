using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("TodoList")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAuthorization(options => options.AddPolicy("UserOnly", policy => policy.RequireClaim("Role", "User").RequireClaim("ID")));

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
