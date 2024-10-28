using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationIdentity(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<DatabaseService>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}