using BookReviewApp.Data.Repositories;
using BookReviewApp.Data.Repositories.Interfaces;
using BookReviewApp.Services;
using BookReviewApp.Services.Caching;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repository Pattern
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Business Services - Direct registration without AutoMapper
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IReviewService, ReviewService>();

            // Caching
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();

            return services;
        }

        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.SuppressMapClientErrors = false;
                options.SuppressConsumesConstraintForFormFileParameters = false;
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "Book Review API",
                    Version = "v1",
                    Description = "A comprehensive book review application API with user authentication, book management, and review system.",
                    Contact = new()
                    {
                        Name = "Book Review API Team",
                        Email = "api@bookreview.com",
                        Url = new Uri("https://github.com/yourusername/BookReviewApp")
                    }
                });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}