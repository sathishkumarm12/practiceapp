using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.data;
using API.Data;
using API.helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddDbContext<DataContext>(options =>
                     {
                         options.UseSqlite(config.GetConnectionString("DefaultConnection"));
                     });

            services.AddScoped<Interfaces.ITokenService, Services.TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            return services;

        }
    }
}