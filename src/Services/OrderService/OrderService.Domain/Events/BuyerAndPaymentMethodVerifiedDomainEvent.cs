using MediatR;

namespace OrderService.Domain.Events;

public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification
{
    public BuyerAndPaymentMethodVerifiedDomainEvent(Guid orderId, Buyer buyer, PaymentMethod payment)
    {
        OrderId = orderId;
        Buyer = buyer;
        Payment = payment;
    }

    public Buyer Buyer { get; private set; }
    public PaymentMethod Payment { get; private set; }
    public Guid OrderId { get; private set; }



}