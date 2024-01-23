using EventBus.Base.Abstraction;
using EventBus.Base;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.EventHandlers;

namespace PaymentService.Api.Extension;

public static class ExtensionMethod
{
    public static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
        services.AddTransient<OrderStartedIntegrationEventHandler>();
        services.AddSingleton<IEventBus>(sp =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",
                SubscriberClientAppName = "PaymentService",
                EventBusType = EventBusType.RabbitMQ
            };
            return EventBusFactory.Create(config, sp);
        });

        return services;
    }
}