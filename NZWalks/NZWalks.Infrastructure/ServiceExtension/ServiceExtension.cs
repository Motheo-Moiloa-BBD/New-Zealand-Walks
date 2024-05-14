using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NZWalks.Core.Interfaces;
using NZWalks.Infrastructure.Data;
using NZWalks.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Infrastructure.ServiceExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<NzWalksDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("NzWalksConnectionString"));
            });

            services.AddDbContext<NzWalksAuthDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("NzWalksConnectionString"));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IWalkRepository, WalkRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();

            return services;
        }
    }
}
