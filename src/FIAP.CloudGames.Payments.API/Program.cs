using FIAP.CloudGames.Payments.Application.Interfaces;
using FIAP.CloudGames.Payments.Application.Services;
using FIAP.CloudGames.Payments.Infrastructure.Messaging;
using FIAP.CloudGames.Payments.Infrastructure.Repositories;
using FIAP.CloudGames.Payments.API.Middlewares;
using FIAP.CloudGames.Payments.Infrastructure.Configuration.Auth;
using FIAP.CloudGames.Payments.Infrastructure.Context;
using FIAP.CloudGames.Payments.Infrastructure.Logging;
using System.Security.Claims;
using System.Text;
using FIAP.CloudGames.Payments.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using FIAP.CloudGames.Payments.Domain.Interfaces.Repositories;
using FIAP.CloudGames.Payments.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Amazon.SQS;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

// DbContext
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

#region Application Services Configuration

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentEventPublisher, PaymentEventPublisher>();
builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.AddControllers();

#endregion

#region JWT Authentication Configuration

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
    throw new InvalidOperationException("JWT settings are not configured properly. Please check your appsettings.json.");

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role
        };
    });

#endregion

var app = builder.Build();

app.UsePathBase("/users");
app.UseRouting();

// Middlewares
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapGet("/health", () => Results.Ok("OK")).AllowAnonymous();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Payments API v1");
    });
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();