using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebPortal.Dll.Interfaces;
using WebPortal.Dll.Repositories;

namespace WebPortal.Bll.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddUnitOfWorkService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
