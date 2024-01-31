using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.IntegrationEvents;

namespace OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);
        var dbOrder = new Order(request.UserName, address, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration, null);

        request.OrderItems.ToList().ForEach(item =>
        {
            dbOrder.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.PictureUrl,item.Units);
        });

        await _orderRepository.AddAsync(dbOrder);
        await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserName);
        _eventBus.Publish(orderStartedIntegrationEvent);

        return true;
    }
}