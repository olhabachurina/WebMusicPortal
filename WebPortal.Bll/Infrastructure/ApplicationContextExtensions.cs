using Microsoft.Extensions.DependencyInjection;
using WebPortal.Dll;

using Microsoft.EntityFrameworkCore;
namespace WebPortal.Bll.Infrastructure.Extensions
{
    public static class ApplicationContextExtensions
    {
        public static void AddApplicationContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}