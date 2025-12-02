using System.Text;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Contracts;
using Infrastructure.MessageQueue;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "API Gateway";
    document.Version = "v1";

    document.AddSecurity("JWT", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Input: Bearer {JWT Token}"
    });
    
    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT")
    );
});

builder.Services.AddSingleton<IMessageBus>(sp =>
    new RabbitMQMessageBus(builder.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
);

builder.Services.AddHttpClient("SalesAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Microservices:Sales"));
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient("StockAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Microservices:Stock"));
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddDbContext<UsersContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
});

builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            RoleClaimType = "role",
            
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build()
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();