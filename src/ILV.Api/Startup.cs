using System;
using ILV.Api.Data;
using ILV.Api.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ILV.Api
{
    public class Startup
    {
        private static IServiceProvider _serviceProvider;

        public Startup()
        {
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            _serviceProvider = new ServiceCollection()
                                    .AddScoped<IPersistenceService, PersistenceService>()
                                    .AddScoped<IMiningService, MiningService>()
                                    .BuildServiceProvider();
        }

        public static T GetService<T>()
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }
    }
}
