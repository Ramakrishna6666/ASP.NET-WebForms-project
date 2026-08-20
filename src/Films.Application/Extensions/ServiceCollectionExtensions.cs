using Microsoft.Extensions.DependencyInjection;
using Films.Application.Interfaces;
using Films.Application.Services;

namespace Films.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        // Register application services
        services.AddScoped<IFilmService, FilmService>();
        services.AddScoped<IActorService, ActorService>();
        services.AddScoped<IDirectedByService, DirectedByService>();

        return services;
    }
}
