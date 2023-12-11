using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API.Errors;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Infrastructure.Services;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds the application service to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration){
            services

                .AddDbContext<StoreContext>(opt => {
                    opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                })
                .AddSingleton<IConnectionMultiplexer>(c => {
                    var configurationOptions = ConfigurationOptions
                        .Parse(configuration.GetConnectionString("Redis"), true);
                    return ConnectionMultiplexer.Connect(configurationOptions);
                })
                .AddScoped<IBasketRepository, BasketRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .Configure<ApiBehaviorOptions>(options => {
                    options.InvalidModelStateResponseFactory = actionOption => {
                        var errors = actionOption.ModelState
                            .Where(e => e.Value!.Errors.Count > 0)
                            .SelectMany(x => x.Value!.Errors)
                            .Select(x => x.ErrorMessage).ToArray();

                        var errorResponse = new ValidationErrorRespose
                        {
                            Errors = errors
                        };

                        return new BadRequestObjectResult(errorResponse);
                    };
                })
                .AddCors(opt => {
                    opt.AddPolicy("CorsPolicy", policy => {
                        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                    });
                });
            return services;
        }
    }
}