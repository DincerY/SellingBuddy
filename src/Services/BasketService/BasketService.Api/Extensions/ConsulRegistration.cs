using Consul;

namespace BasketService.Api.Extensions;

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


        var address = "http://localhost:5003";

        //Register service with consul
        var uri = new Uri(address);
        var registration = new AgentServiceRegistration()
        {
            ID = $"BasketService",
            Name = "BasketService",
            Address = $"{uri.Host}",
            Port = uri.Port,
            Tags = new[] { "BasketService" , "Basket" }
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