using CloudinaryDotNet;
using ECommerce.Core.Services.Implementations;
using ECommerce.Core.Services.Interfaces;
using ECommerce.Infrastructure.Repositories.Implementations;
using ECommerce.Infrastructure.Repositories.Interfaces;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ECommerce.API.Extensions;

public static class DependencyServices
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork<EcommerceDbContext>, UnitOfWork<EcommerceDbContext>>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("RedisConnection");
        services.AddDbContext<EcommerceDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurationOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("RedisConnection"));
            configurationOptions.Ssl = true;
            configurationOptions.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configurationOptions);
        });
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudinaryConfig = configuration.GetSection("Cloudinary").Get<Dictionary<string, string>>();
        var account = new Account(
            cloudinaryConfig["CloudName"],
            cloudinaryConfig["ApiKey"],
            cloudinaryConfig["ApiSecret"]
        );

        var cloudinary = new Cloudinary(account);
        services.AddSingleton(cloudinary);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<IImageService, CloudinaryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreProductService, StoreProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IMoMoService, MoMoService>();

        return services;
    }

    public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo() { Title = "ECommerce", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             new string[] { }
         }
     });
            options.MapType<IFormFile>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            });
        });
        return services;
    }

    public static IServiceCollection AddJwtValidation(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = "Issuer",
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("SuperStrongSecretKeyForJwtToken123!")
                ),
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role,
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/vinWalletHub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        message = "Invalid token"
                    });
                    return context.Response.WriteAsync(result);
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();

                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            message = "You are not authorized"
                        });
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
                        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true"); // tùy
                        context.Response.Headers.Add("Access-Control-Allow-Headers", "*"); // optional

                        return context.Response.WriteAsync(result);
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        code = StatusCodes.Status403Forbidden,
                        message = "You do not have access to this resource"
                    });
                    return context.Response.WriteAsync(result);
                }
            };
        });

        return services;
    }
}
