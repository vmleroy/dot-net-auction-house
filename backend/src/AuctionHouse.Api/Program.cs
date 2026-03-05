using AuctionHouse.Domain.Repositories;
using AuctionHouse.Infrastructure.Data;
using AuctionHouse.Infrastructure.Services;
using AuctionHouse.Infrastructure.Repositories;
using AuctionHouse.Application.Interfaces;
using AuctionHouse.Application.Services;
using AuctionHouse.Application.Utils;
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
builder.Services.AddScoped<ILockService, RedisLockService>();
builder.Services.AddScoped<IBidCacheService, BidCacheService>();
builder.Services.AddScoped<IBidService, BidService>();

// JWT Token Generator
builder.Services.AddScoped<JwtTokenGenerator>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var jwtSection = config.GetSection("Jwt");
    var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    var issuer = jwtSection["Issuer"] ?? "AuctionHouse";
    var audience = jwtSection["Audience"] ?? "AuctionHouseAudience";
    return new JwtTokenGenerator(secret, issuer, audience);
});

// Repositories & Application Services
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

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