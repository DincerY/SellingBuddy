using System.Diagnostics.Eventing.Reader;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandlers;

public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    private readonly IConfiguration configuration;
    private readonly IEventBus eventBus;
    private readonly ILogger<OrderStartedIntegrationEventHandler> logger;


    public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
    {
        this.configuration = configuration;
        this.eventBus = eventBus;
        this.logger = logger;
    }

    //BaseEventBus içindeki ProcessEvent kısımını iyi anlamak lazım.
    public Task Handle(OrderStartedIntegrationEvent @event)
    {
        //Fake payment process
        string keyword = "PaymentSuccess";
        bool paymentSuccessFlag = configuration.GetValue<bool>(keyword);

        IntegrationEvent paymentEvent = paymentSuccessFlag
            ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
            : new OrderPaymentFailedIntegrationEvent(@event.OrderId, "This is a fake error message");

        logger.LogInformation($"OrderCreatedIntegrationEventHandler is PaymentService is fired with PaymentSuccess : {paymentSuccessFlag}, orderId : {@event.OrderId}");

        eventBus.Publish(paymentEvent);

        return Task.CompletedTask;
    }
}