using AuctionHouse.Domain.Entities;
using AuctionHouse.Infrastructure.Data;
using AuctionHouse.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;
using Scalar.AspNetCore;

/**
  *  Builder
 **/
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Db connections
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379"));

// Authentication & Identity
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme);
builder.Services.AddAuthorization();

// Custom services
builder.Services.AddScoped<BidCacheService>();
builder.Services.AddScoped<AuctionHouse.Application.Services.JwtTokenGenerator>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var jwtSection = config.GetSection("Jwt");
    var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    var issuer = jwtSection["Issuer"] ?? "AuctionHouse";
    var audience = jwtSection["Audience"] ?? "AuctionHouseAudience";
    return new AuctionHouse.Application.Services.JwtTokenGenerator(secret, issuer, audience);
});
builder.Services.AddScoped<AuctionHouse.Application.Services.IAuthService, AuctionHouse.Application.Services.AuthService>();
builder.Services.AddScoped<AuctionHouse.Domain.Repositories.IUserRepository, AuctionHouse.Infrastructure.Repositories.UserRepository>();

/**
  *  App
 **/
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => 
        options.AddHttpAuthentication("BearerAuth", auth =>
        {
            auth.Token = "YOUR_AUTH_TOKEN";
        })
        .EnablePersistentAuthentication());
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();