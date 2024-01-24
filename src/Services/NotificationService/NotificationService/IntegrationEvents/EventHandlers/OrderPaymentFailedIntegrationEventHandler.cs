using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers;

public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> logger;

    public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
    {
        this.logger = logger;
    }

    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        //Send Fail Notification (Sms,Email,Call)
        logger.LogInformation($"Order Payment failed with OrderId : {@event.OrderId}, Error Message : {@event.ErrorMessage}");

        return Task.CompletedTask;
    }
}