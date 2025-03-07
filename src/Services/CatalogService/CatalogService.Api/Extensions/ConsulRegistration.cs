﻿using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace CatalogService.Api.Extensions;

public static class ConsulRegistration
{
    public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["ConsulConfig:Address"];
            consulConfig.Address = new Uri(address);
        }));

        return services;
    }



    public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();


        ////Get server IP address
        //var features = app.Properties["server.Features"] as FeatureCollection;
        ////.NET içinde, çalışan uygulamanın hangi address te olduğunu bu şekilde öğrenebiliyoruz 
        //var addresses = features.Get<IServerAddressesFeature>().Addresses;
     
        //var address = addresses.First();
        

        var address = "http://localhost:5004";

        //Register service with consul
        var uri = new Uri(address);
        var registration = new AgentServiceRegistration()
        {
            ID = $"CatalogService",
            Name = "CatalogService",
            Address = $"{uri.Host}",
            Port = uri.Port,
            Tags = new []{"CatalogService", "Catalog"}
        };

        logger.LogInformation("Registering with consul");
        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Deregistering from consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
    }
}