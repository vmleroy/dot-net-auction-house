using AuctionHouse.Domain.Repositories;
using AuctionHouse.Infrastructure.Data;
using AuctionHouse.Infrastructure.Services;
using AuctionHouse.Infrastructure.Repositories;
using AuctionHouse.Infrastructure.Workers;
using AuctionHouse.Application.Interfaces;
using AuctionHouse.Application.Services;
using AuctionHouse.Application.Utils;
using Microsoft.EntityFrameworkCore;
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
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecret = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = jwtSection["Issuer"] ?? "AuctionHouse";
var jwtAudience = jwtSection["Audience"] ?? "AuctionHouseAudience";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSecret))
        };
    });
builder.Services.AddAuthorization();

// JWT Token Generator
builder.Services.AddScoped<JwtTokenGenerator>(sp =>
    new JwtTokenGenerator(jwtSecret, jwtIssuer, jwtAudience));

// Repositories & Application Services
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();

builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<AuctionClosingWorker>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ILockService, RedisLockService>();
builder.Services.AddScoped<IBidCacheService, BidCacheService>();
builder.Services.AddScoped<IBidService, BidService>();

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