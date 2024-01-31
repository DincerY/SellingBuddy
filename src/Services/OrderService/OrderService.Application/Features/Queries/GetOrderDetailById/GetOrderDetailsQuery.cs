using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderService.Application.Features.Queries.ViewModels;

namespace OrderService.Application.Features.Queries.GetOrderDetailById;

public class GetOrderDetailsQuery : IRequest<OrderDetailViewModel>
{
    public GetOrderDetailsQuery(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; set; }
    
}