using Films.Domain.Interfaces.Repositories;
using Films.Infrastructure.Data;
using Films.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Films.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<FilmsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<IFilmRepository, FilmRepository>();
        services.AddScoped<IActorRepository, ActorRepository>();
        services.AddScoped<IDirectedByRepository, DirectedByRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISexRepository, SexRepository>();
        services.AddScoped<ITypeUserRepository, TypeUserRepository>();

        return services;
    }
}
