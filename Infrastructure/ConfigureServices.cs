using Application.Interfaces;
using Infrastructure.Music;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IMusicService, SpotifyService>();

            return services;
        }
    }
}
