using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

public class Order : BaseEntity, IAggregateRoot
{
    public DateTime OrderDate { get; private set; }
    public int Quantity { get; private set; }
    public string Description { get; private set; }
    public Guid? BuyerId { get; private set; }
    public Buyer Buyer { get; private set; }
    public Address Address { get; private set; }

    private int _orderStatusId;
    public OrderStatus OrderStatus { get; private set; }

    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public Guid? PaymentMethodId { get; set; }

    public Order()
    {
        BuyerId = buyerId;
        Id = Guid.NewGuid();
        _orderItems = new List<OrderItem>();
    }

    public Order(string userName, string cardHolderName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber, DateTime cardExpiration, Guid? paymentMethodId, Guid? buyerId = null) : this()
    {
        BuyerId = buyerId;
        Address = address;
        PaymentMethodId = paymentMethodId;
        _orderStatusId = OrderStatus.Submitted.Id;
        OrderDate = DateTime.UtcNow;

        AddOrderStartedDomainEvent(userName, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);
    }
}