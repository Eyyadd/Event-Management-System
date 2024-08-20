using BLL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Classes
{
    public static class RepositoryDependancyInjection
    {
        public static IServiceCollection AddRepositories (this IServiceCollection services)
        {
            services.AddScoped<IRepository<Event>, Repository<Event>>();
            services.AddScoped<IRepository<Tickets>, Repository<Tickets>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Feedback>, Repository<Feedback>>();
            services.AddScoped<IRepository<EventBooking>, Repository<EventBooking>>();
            services.AddScoped<IUniteOfWork, UnitOfWork>();
            return services;
        }
    }
}
