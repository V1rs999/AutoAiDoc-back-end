using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Data;
using WebApi.Interface;
using WebApi.Middleware;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDropFileRepository, DropFileRepository>();
builder.Services.AddScoped<IListOFErrorRepository, ListOFErrorRepository>();
builder.Services.AddScoped<IFileReader, FileReader>();
builder.Services.AddScoped<IToken, Token>();
builder.Services.AddDbContext<AppDbContext>(e =>
    e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789абвгґдеєжзиіїйклмнопрстуфхцчшщьюяАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ!@#$%^&*()-_+=<>?/.,:; ";
    })
    //.AddUserValidator<CustomUserValidator<AppUser>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("JWT:ValidIssuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("JWT:ValidAudience"),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:Secret"))),
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
.AddGoogle(options =>
{
    options.ClientId = "500684865018-bniojefur61oagvkmusm18320updkb0u.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

builder.Services.Configure<IdentityOptions>(opt =>
	{
		opt.Password.RequireDigit = false;
		opt.Password.RequireLowercase = false;
		opt.Password.RequireUppercase = false;
		opt.Password.RequireNonAlphanumeric = false;
		opt.Password.RequiredLength = 0;
		opt.Password.RequiredUniqueChars = 1;
	});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseDirectoryBrowser();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.MapControllers();

app.Run();
